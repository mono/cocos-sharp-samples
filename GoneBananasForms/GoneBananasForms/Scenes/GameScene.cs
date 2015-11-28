using System;
using System.IO;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework.Graphics;

namespace GoneBananasForms
{
    public class GameScene : CCScene
    {
        // There's a bug with iOS that requires us to specify the full-path of music assets (relative to Content)
        // even if we've added the Sounds folder to the ContentSearchPaths
        // This will be fixed in subsequent verions of CocosSharp
        public readonly static string MusicPath = Path.Combine("Sounds", "backgroundMusic");
        public readonly static string EatingEffectPath = Path.Combine("Sounds", "eating_effect");
        public readonly static string HurtEffectPath = Path.Combine("Sounds", "hurt_effect");

        static readonly CCColor4B backgroundColor = new CCColor4B (88, 140, 238);

        const int backgroundZOrder = 0;
        const int groundZOrder = 2;
        const int monkeyZOrder = 3;
        const int treesZOrder = 4;

        // Game state
        bool moveEnded;
        bool crouched;
        bool gameStopped;
        float scheduledMoveDistance;
        float timeElapsed;
        int bananasCollected;

        // Layers
        CCLayer mainLayer;
        CCLayer hudLayer;

        // Sprite sheets
        CCSpriteSheet backgroundSpriteSheet;
        CCSpriteSheet contentSpriteSheet;

        // Background content
        CCSprite ground;
        CCParallaxNode cloudsParallax;

        // Dynamic content
        float bananaScale;
        CCSpriteFrame bananaSpriteFrame;
        List<CCSprite> bananas;
        List<CCSprite> bananasToRemove;

        CCSpriteFrame coconutSpriteFrame;
        List<CCSprite> coconuts;
        List<CCSprite> coconutsToRemove;

        CCFiniteTimeAction bananaRotateAction;
        CCFiniteTimeAction bananaDropAction;
        CCFiniteTimeAction bananaEatenAction;
        CCFiniteTimeAction bananaFinishAction;

        CCFiniteTimeAction coconutRotateAction;
        CCFiniteTimeAction coconutDropAction;

        // Monkey content
        bool monkeyRightFacing;
        CCSpriteFrame restingMonkeyFrame;
        CCSpriteFrame hurtMonkeyFrame;
        CCSpriteFrame duckingMonkeyFrame;
        CCSprite monkey;
        CCAction monkeyHurt;
        CCAction monkeyEating;
        CCAction monkeyWalking;
        CCActionState currentMonkeyMovingState;
        CCActionState currentMonkeyWalkingState;

        // Hud content
        CCLabel bananaCounterLabel;
        CCSprite rightButton;
        CCSprite leftButton;
        CCSprite duckButton;


        #region Initialisation

        public GameScene (CCGameView gameView) : base(gameView)
        {
            CreateLayers ();

            SetupInput ();

            LoadSpriteSheets ();
            LoadClouds ();
            LoadBackground ();
            LoadMonkey ();
            LoadBananas ();
            LoadCoconuts ();

            LoadScoreboard ();

            Schedule (UpdateInput, 0.1f);
            Schedule (GenerateDynamicContent, 1.0f);
            Schedule (CheckCollision, 0.2f);
        }

        void SetupInput ()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;

            var spriteSheet = new CCSpriteSheet ("controls_spritesheet.plist");
            rightButton = new CCSprite (spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("right")));
            leftButton = new CCSprite (spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("left")));
            duckButton = new CCSprite (spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("duck")));

            float scale = bounds.Size.Height / (6 * rightButton.ContentSize.Height);
            rightButton.Scale = leftButton.Scale = duckButton.Scale = scale;
            rightButton.Opacity = leftButton.Opacity = duckButton.Opacity = 175;

            leftButton.Position = new CCPoint (bounds.Origin.X + 0.1f * bounds.Size.Width, 
                bounds.Origin.Y + 0.1f * bounds.Size.Height);

            rightButton.Position = leftButton.Position + new CCPoint (1.5f * leftButton.Position.X, 0.0f);

            duckButton.Position = new CCPoint (bounds.Origin.X + 0.90f * bounds.Size.Width, 
                bounds.Origin.Y + 0.1f * bounds.Size.Height);

            hudLayer.AddChild (leftButton);
            hudLayer.AddChild (rightButton);
            hudLayer.AddChild (duckButton);

            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            touchListener.OnTouchesMoved = OnTouchesBegan;
            touchListener.OnTouchesEnded = OnTouchesEnded;

            hudLayer.AddEventListener (touchListener);
        }

        void CreateLayers ()
        {
            mainLayer = new CCLayerColor (backgroundColor);
            hudLayer = new CCLayer ();

            this.AddLayer (mainLayer);
            this.AddLayer (hudLayer);
        }

        void LoadSpriteSheets ()
        {
            backgroundSpriteSheet = new CCSpriteSheet ("background_spritesheet.plist");
            contentSpriteSheet = new CCSpriteSheet ("monkey_spritesheet.plist");
        }

        void LoadClouds ()
        {
            var cloudSpriteFrame = backgroundSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("clouds"));

            var clouds = new CCSprite (cloudSpriteFrame);

            var bounds = mainLayer.VisibleBoundsWorldspace;

            clouds.ContentSize = new CCSize (bounds.Size.Width * 1.5f, bounds.Size.Height / 4.0f);
            clouds.Position = bounds.Center;

            cloudsParallax = new CCParallaxNode ();
            cloudsParallax.AddChild (clouds, backgroundZOrder, new CCPoint (0.4f, 0.0f), 
                new CCPoint (bounds.Size.Width * 0.5f, 2 * bounds.Size.Height / 3.0f));

            cloudsParallax.Position = bounds.Center;

            mainLayer.AddChild (cloudsParallax, backgroundZOrder);
        }

        void LoadBackground ()
        {
            var treesSpriteFrame = backgroundSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("trees"));
            var backgroundSpriteFrame = backgroundSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("background"));

            var trees = new CCSprite (treesSpriteFrame);
            trees.AnchorPoint = CCPoint.AnchorUpperLeft;

            var background = new CCSprite (backgroundSpriteFrame);
            background.AnchorPoint = CCPoint.AnchorLowerLeft;

            ground = new CCSprite ("block");
            ground.AnchorPoint = CCPoint.AnchorLowerLeft;

            // Here, we're repeating the block texture to create the ground
            // Use point wrap to maintain the sharp retro look
            // i.e. A linear wrap will perform some interpolation and smoothen out edges which we don't want 
            ground.Texture.SamplerState = SamplerState.PointWrap;

            var texRect = ground.TextureRectInPixels;
            texRect.Origin.X -= texRect.Size.Width / 2;
            texRect.Size.Height += 1;

            // We roughly want the block texture to be repeated 15 times horizontally
            texRect.Size.Width *= 15;
            ground.TextureRectInPixels = texRect;

            mainLayer.AddChild (background, backgroundZOrder);
            mainLayer.AddChild (trees, treesZOrder);
            mainLayer.AddChild (ground, groundZOrder);

            var bounds = mainLayer.VisibleBoundsWorldspace;

            trees.Position = new CCPoint (bounds.Origin.X, bounds.Origin.Y + bounds.Size.Height);
            trees.ContentSize = new CCSize (bounds.Size.Width, bounds.Size.Height / 3.0f);

            background.Position = bounds.Origin;
            background.ContentSize = new CCSize (bounds.Size.Width, 0.9f * bounds.Size.Height);

            ground.Position = bounds.Origin;
            ground.ContentSize = new CCSize (bounds.Size.Width, bounds.Size.Height / 11.0f);
        }

        void LoadMonkey ()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;

            var eatingAnimationFrames = contentSpriteSheet.Frames.FindAll ((x) => x.TextureFilename.StartsWith ("eating"));
            var walkingAnimationFrames = contentSpriteSheet.Frames.FindAll ((x) => x.TextureFilename.StartsWith ("walking"));

            restingMonkeyFrame = eatingAnimationFrames[0];
            hurtMonkeyFrame = contentSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("hurt"));
            duckingMonkeyFrame = contentSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("duck"));

            eatingAnimationFrames.Sort(
                delegate(CCSpriteFrame p1, CCSpriteFrame p2)
                {
                    return p1.TextureFilename.CompareTo(p2.TextureFilename);
                }
            );

            walkingAnimationFrames.Sort(
                delegate(CCSpriteFrame p1, CCSpriteFrame p2)
                {
                    return p1.TextureFilename.CompareTo(p2.TextureFilename);
                }
            );

            monkeyEating = new CCSequence(new CCAnimate(new CCAnimation (eatingAnimationFrames, 0.2f)), 
                new CCCallFuncN(node => { var sprite = node as CCSprite; sprite.SpriteFrame = restingMonkeyFrame; }));

            monkeyWalking = new CCRepeatForever(new CCAnimate(new CCAnimation (walkingAnimationFrames, 0.1f)));

            var shakeAction = new CCMoveBy (0.1f, new CCPoint(bounds.Size.Width / 15.0f, 0.0f));

            monkeyHurt = new CCSequence(shakeAction, shakeAction.Reverse(), 
                new CCMoveBy (0.3f, new CCPoint(0.0f, bounds.Size.Height / 8.0f)), 
                new CCMoveBy (0.5f, new CCPoint(0.0f, -bounds.Size.Height / 2.0f)), new CCCallFuncN (TransitionOut));

            monkey = new CCSprite (restingMonkeyFrame);
            monkey.AnchorPoint = CCPoint.AnchorMiddleBottom;

            mainLayer.AddChild (monkey, monkeyZOrder);

            monkey.Scale =  bounds.Size.Width / (6 * monkey.ContentSize.Width); 
            monkey.Position = new CCPoint (ground.ContentSize.Width / 2, ground.Position.Y + ground.ContentSize.Height);

            bananaScale = bounds.Size.Width / (6 * monkey.ContentSize.Width);
        }

        void LoadBananas ()
        {
            bananas = new List<CCSprite> ();
            bananasToRemove = new List<CCSprite> ();
            bananaSpriteFrame = contentSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("bananas"));
            bananaRotateAction = new CCRotateBy (0.8f, 360);
            bananaFinishAction = new CCCallFuncN (node => node.RemoveFromParent ());
            bananaEatenAction = new CCSequence (new CCDelayTime (0.3f), bananaFinishAction);
        }

        void LoadCoconuts ()
        {
            coconuts = new List<CCSprite> ();
            coconutsToRemove = new List<CCSprite> ();
            coconutSpriteFrame = contentSpriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("coconut"));
            coconutRotateAction = new CCRotateBy (2.0f, 360);
        }

        void LoadScoreboard ()
        {
            var bounds = hudLayer.VisibleBoundsWorldspace;

            var bananaIcon = new CCSprite (bananaSpriteFrame);
            bananaIcon.AnchorPoint = CCPoint.AnchorMiddleLeft;
            bananaIcon.Scale = bounds.Size.Height / (10 * bananaIcon.ContentSize.Height);
            bananaIcon.Position = new CCPoint (bounds.Origin.X + 0.05f * bounds.Size.Width, 
                bounds.Origin.Y + 0.90f * bounds.Size.Height);

            bananaCounterLabel = new CCLabel ( "x0", "pixelfont", 60, CCLabelFormat.SpriteFont) {
                Color = CCColor3B.Yellow,
                HorizontalAlignment = CCTextAlignment.Left,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddleLeft
            };

            var bananaIconSize = bananaIcon.ScaledContentSize;

            bananaCounterLabel.Position = new CCPoint (bananaIcon.Position.X + bananaIconSize.Width * 1.2f, bananaIcon.Position.Y);

            hudLayer.AddChild (bananaIcon);
            hudLayer.AddChild (bananaCounterLabel);
        }

        #endregion Initialisation


        #region Dynamic content

        CCSprite AddBanana ()
        {
            var banana = new CCSprite (bananaSpriteFrame);
            banana.AnchorPoint = CCPoint.AnchorMiddle;
            banana.Scale = bananaScale;

            var boundingBox = banana.BoundingBox;

            var p = GetRandomPosition (boundingBox.Size);
            banana.Position = p;

            mainLayer.AddChild (banana, monkeyZOrder);

            float dropTime = 3.0f;

            bananaDropAction = new CCMoveTo (dropTime, new CCPoint (banana.Position.X, - 2 * boundingBox.Size.Height));
            banana.RunActions (bananaDropAction, bananaFinishAction);
            banana.RepeatForever (bananaRotateAction);

            return banana;
        }

        CCSprite AddCoconut ()
        {
            var coconut = new CCSprite (coconutSpriteFrame);
            coconut.AnchorPoint = CCPoint.AnchorMiddle;
            coconut.Scale = bananaScale;

            var boundingBox = coconut.BoundingBox;

            var p = GetRandomPosition (boundingBox.Size);
            coconut.Position = p;

            mainLayer.AddChild (coconut, monkeyZOrder);

            float dropTime = Math.Max (3.0f / (0.1f * timeElapsed + 1), 1.0f);

            coconutDropAction = new CCMoveTo (dropTime, new CCPoint (coconut.Position.X, - 2 * boundingBox.Size.Height));
            coconut.RunActions (coconutDropAction, bananaFinishAction);
            coconut.RepeatForever (coconutRotateAction);

            return coconut;
        }

        CCPoint GetRandomPosition (CCSize spriteSize)
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;

            double rnd = CCRandom.NextDouble ();
            double randomX = (rnd > 0) 
                ? rnd * bounds.Size.Width - spriteSize.Width / 2 
                : spriteSize.Width / 2;

            return new CCPoint ((float)randomX, bounds.Size.Height - spriteSize.Height / 2);
        }

        void UpdateScore ()
        {
            bananasCollected += 1;
            bananaCounterLabel.Text = String.Format ("x{0}", bananasCollected);
        }

        void PlayEatingSoundEffect ()
        {
            GameView.AudioEngine.StopAllEffects ();
            GameView.AudioEngine.PlayEffect (EatingEffectPath);
        }

        void PlayHurtSoundEffect ()
        {
            GameView.AudioEngine.StopAllEffects ();
            GameView.AudioEngine.PlayEffect (HurtEffectPath);
        }

        #endregion Dynamic content



        #region Run loop

        void UpdateInput (float time)
        {
            if (gameStopped)
                return;

            if (moveEnded) 
            {
                if (currentMonkeyWalkingState != null) 
                {
                    monkey.StopAction (currentMonkeyWalkingState);
                    currentMonkeyWalkingState = null;
                }

                monkey.SpriteFrame = crouched ? duckingMonkeyFrame : restingMonkeyFrame;

                scheduledMoveDistance = 0.0f;
                moveEnded = false;
            }

            if (scheduledMoveDistance != 0.0f) 
            {
                if (currentMonkeyWalkingState == null)
                    currentMonkeyWalkingState = monkey.RunAction (monkeyWalking);
                
                MoveMonkey (scheduledMoveDistance);
            }
        }

        void GenerateDynamicContent (float time)
        {
            if (gameStopped)
                return;

            timeElapsed += time;

            bananas.Add (AddBanana ());
            coconuts.Add (AddCoconut ());
        }

        void CheckCollision (float time)
        {
            if (gameStopped || crouched)
                return;

            var monkeyBounds = monkey.BoundingBoxTransformedToParent;
            var reducedMonkeyBounds = monkeyBounds;
            reducedMonkeyBounds.Origin.Y += 0.8f * reducedMonkeyBounds.Size.Height;
            reducedMonkeyBounds.Size.Height = 0.3f * reducedMonkeyBounds.Size.Height;

            float xOffset = monkeyRightFacing ? 0.3f : 0.0f;

            reducedMonkeyBounds.Origin.X += reducedMonkeyBounds.Size.Width * xOffset;
            reducedMonkeyBounds.Size.Width *= 0.5f;


            // Check if coconut has hit monkey
            foreach (var coconut in coconuts) {
                if (coconut.BoundingBoxTransformedToParent.IntersectsRect (reducedMonkeyBounds)) 
                {
                    coconut.RunAction (bananaFinishAction);
                    coconutsToRemove.Add (coconut);
                    monkey.StopAllActions ();
                    monkey.SpriteFrame = hurtMonkeyFrame;
                    monkey.RunAction (monkeyHurt);
                    PlayHurtSoundEffect ();

                    StopGame ();
                    break;
                }
            }


            // Otherwise, check if banana has hit monkey

            if (!gameStopped) 
            {
                foreach (var banana in bananas) {
                    if (banana.BoundingBoxTransformedToParent.IntersectsRect (reducedMonkeyBounds)) {
                        UpdateScore ();
                        PlayEatingSoundEffect ();
                        banana.RunAction (bananaEatenAction);
                        bananasToRemove.Add (banana);
                        monkey.StopAllActions ();
                        monkey.RunAction (monkeyEating);
                    }
                }
            }

            foreach (var banana in bananasToRemove) 
            {
                bananas.Remove (banana);
            }

            foreach (var coconut in coconutsToRemove) 
            {
                coconuts.Remove (coconut);
            }

            coconutsToRemove.Clear ();
            bananasToRemove.Clear ();
        }

        void StopGame ()
        {
            if (gameStopped)
                return;

            gameStopped = true;
            Unschedule (GenerateDynamicContent);
            Unschedule (CheckCollision);
            mainLayer.RemoveAllListeners ();
        }

        void TransitionOut (object sender)
        {
            var scene = new GameOverScene (bananasCollected, GameView);

            var transitionToGame = new CCTransitionSlideInT (1.0f, scene);

            GameView.Director.ReplaceScene (transitionToGame);
        }

        #endregion Run loop


        #region Touch handling

        static CCRect EnlargedPoint (CCPoint point, float factor)
        {
            float halfFactor = factor / 2.0f;
            return new CCRect (point.X - halfFactor, point.Y - halfFactor, point.X + factor, point.Y + factor);
        }

        void OnTouchesBegan (List<CCTouch> touches, CCEvent touchEvent)
        {
            if (crouched)
                return;

            foreach (CCTouch touch in touches) 
            {
                var location = touches [0].LocationOnScreen;
                location = hudLayer.ScreenToWorldspace (location);

                moveEnded = false;
                scheduledMoveDistance = ProcessMoveDistance (location);

                if (crouched) { 
                    scheduledMoveDistance = 0.0f;
                    moveEnded = true;
                    break;
                } else if (scheduledMoveDistance > 0.0f)
                    break;
            }
        }

        void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
        {
            moveEnded = true;
            crouched = false;
        }
            
        float ProcessMoveDistance (CCPoint location)
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;

            CCRect locationRect = EnlargedPoint (location, bounds.Size.Height / 12.0f);

            float moveDistance = 0.0f;

            if (locationRect.IntersectsRect (duckButton.BoundingBoxTransformedToWorld))
                crouched = true;
            else if (locationRect.IntersectsRect (leftButton.BoundingBoxTransformedToWorld))
                moveDistance = -bounds.Size.Width / 10.0f;
            else if (locationRect.IntersectsRect (rightButton.BoundingBoxTransformedToWorld))
                moveDistance = bounds.Size.Width / 10.0f;

            return moveDistance;
        }

        void MoveMonkey (float ds)
        {

            if (currentMonkeyMovingState != null) {
                monkey.StopAction (currentMonkeyMovingState);
            }

            var bounds = mainLayer.VisibleBoundsWorldspace;

            var monkeyBounds = monkey.BoundingBox;
            monkeyBounds.Origin += new CCPoint (ds, 0.0f);

            if (Math.Abs (bounds.Intersection (monkeyBounds).Size.Width - monkeyBounds.Size.Width) > 1.0f)
                return;

            var dt =  Math.Max(Math.Min(bounds.Size.Width / (ds), 0.3f), 0.2f);

            var moveMonkey = new CCMoveBy (dt, new CCPoint (ds, 0.0f));

            var moveClouds = new CCMoveBy (dt, new CCPoint (-ds / 20.0f, 0.0f));

            monkeyRightFacing = ds > 0.0f;
            monkey.FlipX = monkeyRightFacing;

            currentMonkeyMovingState = monkey.RunAction (moveMonkey);

            cloudsParallax.RunAction (moveClouds);
        }

        #endregion Touch handling
    }
}

