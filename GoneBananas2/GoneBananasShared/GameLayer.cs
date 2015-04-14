using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

#if IOS
using WormHoleSharp;
#endif

namespace GoneBananas
{
	public class GameLayer : CCLayerColor
	{
		// game ends after 60 seconds or when the monkey hits a ball, whichever comes first
		const float GAME_DURATION = 60.0f;
		const int MAX_NUM_BALLS = 10;

		// point to meter ratio for physics
		const int PTM_RATIO = 32;

		//speed factor will be adjusted based on screen size;
		float monkeySpeed = 350.0f;

		float elapsedTime;
		CCSprite monkey;
		List<CCSprite> visibleBananas;
		List<CCSprite> hitBananas;

		// monkey walking animation
		CCAnimation walkAnim;
		CCRepeatForever walkRepeat;
		CCCallFuncN walkAnimStop = new CCCallFuncN (node => node.StopAllActions ());

		// background sprite
		CCSprite grass;

		// particles
		CCParticleSun sun;

		// circle layered behind sun
		CCDrawNode circleNode;

		// parallax node for clouds
		CCParallaxNode parallaxClouds;
            
		// define our banana rotation action
		CCRotateBy rotateBanana = new CCRotateBy (0.8f, 360);

		// define our completion action to remove the banana once it hits the bottom of the screen
		CCCallFuncN moveBananaComplete = new CCCallFuncN (node => node.RemoveFromParent ());

		// physics world
		b2World world;
        
		// balls sprite batch
		CCSpriteBatchNode ballsBatch;
		CCTexture2D ballTexture;

		#if IOS
		Wormhole wormhole;
		#endif

		// particle system for exploding banana
		CCParticleSystemQuad explosion;

		// rect to constrain where monkey can move to
		CCRect allowableMovementRect;

		float contentScaleFactorY;
		float contentScaleFactorX;

		public GameLayer (float scaleX, float scaleY)
		{
			contentScaleFactorY = scaleY;
			contentScaleFactorX = scaleX;

			var speedFactor = contentScaleFactorX < contentScaleFactorY ? contentScaleFactorX : contentScaleFactorY;

			monkeySpeed *= (float)speedFactor;

			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = OnTouchesEnded;
			       
			AddEventListener (touchListener, this);
			Color = new CCColor3B (CCColor4B.White);
			Opacity = 255;

			visibleBananas = new List<CCSprite> ();
			hitBananas = new List<CCSprite> ();

			// batch node for physics balls
			ballsBatch = new CCSpriteBatchNode ("balls", 100);
			ballTexture = ballsBatch.Texture;
			AddChild (ballsBatch, 1, 1);
	
			AddGrass ();
			AddSun ();

			StartScheduling ();

			CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic ("sounds/backgroundMusic", true);

#if IOS
			wormhole = new Wormhole ("group.com.mikebluestein.GoneBananas", "messageDir");
			wormhole.ListenForMessage<int> ("dpad", (message) => {

				CCPoint ds;

				float delta = 25.0f;

				switch (message) {
				case 1: //right

					float dx0 = allowableMovementRect.Size.Width - monkey.Position.X;
					delta = dx0 < delta ? dx0 : delta;
					ds = new CCPoint (delta, 0);
					break;
				case 2: //left

					delta =   monkey.Position.X - delta < 0 ? 0 : delta;
					ds = new CCPoint (-delta, 0);
					break;
				case 3: //up

					float dy0 = allowableMovementRect.Size.Height - monkey.Position.Y;
					delta = dy0 < delta ? dy0 : delta;
					ds = new CCPoint (0, delta);
					break;
				case 4: //down
					
					delta =   monkey.Position.Y - delta < 0 ? 0 : delta;
					ds = new CCPoint (0, -delta);
					break;
				default:
					ds = new CCPoint ();
					break;
				}
			
				var dt = delta / monkeySpeed;

				var moveMonkey = new CCMoveBy (dt, ds);

				monkey.RunAction (walkRepeat);
				monkey.RunActions (moveMonkey, new CCDelayTime (0.2f), walkAnimStop);

			});
#endif
		}

		void StartScheduling ()
		{
			Schedule (t => {
				visibleBananas.Add (AddBanana ());
				elapsedTime += t;
				if (ShouldEndGame ()) {
					EndGame ();
				}
				AddBall ();
			}, 1.0f);

			Schedule (t => CheckCollision ());

			Schedule (t => {
				world.Step (t, 8, 1);

				foreach (CCPhysicsSprite sprite in ballsBatch.Children) {
					if (sprite.Visible && sprite.PhysicsBody.Position.x < 0f || sprite.PhysicsBody.Position.x * PTM_RATIO > ContentSize.Width) {
						world.DestroyBody (sprite.PhysicsBody);
						sprite.Visible = false;
						sprite.RemoveFromParent ();
					} else {
						sprite.UpdateBallTransform ();
					}
				}
			});
		}

		void AddGrass ()
		{
			grass = new CCSprite ("grass");
			allowableMovementRect = CreateAllowableMovementRect (grass);
			grass.ScaleY = contentScaleFactorY;
			grass.ScaleX = contentScaleFactorX;
			AddChild (grass);
		}

		CCRect CreateAllowableMovementRect (CCSprite grass)
		{
			CCRect rect = grass.BoundingBox;
			float h = (rect.Size.Height - 300.0f) * contentScaleFactorY;
			float w = rect.Size.Width * contentScaleFactorX;
			CCRect rect2 = new CCRect (0, 0, w, h);

			return rect2;
		}

		void AddSun ()
		{
			circleNode = new CCDrawNode ();
			circleNode.DrawSolidCircle (CCPoint.Zero, 30.0f, CCColor4B.Yellow);
			AddChild (circleNode);

			sun = new CCParticleSun (CCPoint.Zero);
			sun.StartColor = new CCColor4F (CCColor3B.Red);
			sun.EndColor = new CCColor4F (CCColor3B.Yellow);
			AddChild (sun);
		}

		void AddMonkey ()
		{
			var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
			var animationFrames = spriteSheet.Frames.FindAll ((x) => x.TextureFilename.StartsWith ("frame"));

			walkAnim = new CCAnimation (animationFrames, 0.1f);
			walkRepeat = new CCRepeatForever (new CCAnimate (walkAnim));
			monkey = new CCSprite (animationFrames.First ()) { Name = "Monkey" };

			var baselineScale = 0.25f; //monkey image actual size is 4x what looks good at baseline, so scale by 0.25 for baseline
			var contentScale = contentScaleFactorX > contentScaleFactorY ? contentScaleFactorX : contentScaleFactorY; //use the larger scale if they are not equal

			monkey.Scale = baselineScale * contentScale;

			AddChild (monkey);
		}

		CCSprite AddBanana ()
		{
			var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
			var banana = new CCSprite (spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("Banana")));

			var p = GetRandomPosition (banana.ContentSize);
			banana.Position = p;

			var baselineScale = 0.5f;
			var contentScale = contentScaleFactorX > contentScaleFactorY ? contentScaleFactorX : contentScaleFactorY;

			banana.Scale = baselineScale * contentScale;

			AddChild (banana);

			var moveBanana = new CCMoveTo (5.0f, new CCPoint (banana.Position.X, 0));
			banana.RunActions (moveBanana, moveBananaComplete);
			banana.RepeatForever (rotateBanana);

			return banana;
		}

		CCPoint GetRandomPosition (CCSize spriteSize)
		{
			double rnd = CCRandom.NextDouble ();
			double randomX = (rnd > 0) 
                ? rnd * VisibleBoundsWorldspace.Size.Width - spriteSize.Width / 2 
                : spriteSize.Width / 2;

			return new CCPoint ((float)randomX, VisibleBoundsWorldspace.Size.Height - spriteSize.Height / 2);
		}

		void AddClouds ()
		{
			float h = VisibleBoundsWorldspace.Size.Height;

			parallaxClouds = new CCParallaxNode {
				Position = new CCPoint (0, h)
			};
             
			AddChild (parallaxClouds);

			var cloud1 = new CCSprite ("cloud");
			var cloud2 = new CCSprite ("cloud");
			var cloud3 = new CCSprite ("cloud");

			float yRatio1 = 1.0f;
			float yRatio2 = 0.15f;
			float yRatio3 = 0.5f;

			parallaxClouds.AddChild (cloud1, 0, new CCPoint (1.0f, yRatio1), new CCPoint (100 * contentScaleFactorX, -100 + h - (h * yRatio1)));
			parallaxClouds.AddChild (cloud2, 0, new CCPoint (1.0f, yRatio2), new CCPoint (250 * contentScaleFactorX, -200 + h - (h * yRatio2)));
			parallaxClouds.AddChild (cloud3, 0, new CCPoint (1.0f, yRatio3), new CCPoint (400 * contentScaleFactorX, -150 + h - (h * yRatio3)));
		}

		void MoveClouds (float dy)
		{
			parallaxClouds.StopAllActions ();
			var moveClouds = new CCMoveBy (1.0f, new CCPoint (0, dy * 0.1f));
			parallaxClouds.RunAction (moveClouds);
		}

		void CheckCollision ()
		{
			//TODO: split up collision rects for monkey into several smaller rects

			float fudgeFactor = 0.9f; //do the collision check on a slightly smaller rect to avoid hits in alpha space

			var rect = monkey.BoundingBoxTransformedToParent;
			var smRect = new CCRect (rect.LowerLeft.X, rect.LowerLeft.Y, rect.Size.Width * fudgeFactor, rect.Size.Height * fudgeFactor);

			foreach (var banana in visibleBananas) {

				bool hit = banana.BoundingBoxTransformedToParent.IntersectsRect (smRect);
				if (hit) {
					hitBananas.Add (banana);
					CCSimpleAudioEngine.SharedEngine.PlayEffect ("sounds/tap");
					Explode (banana.Position);
					banana.RemoveFromParent ();
				}
			}

			foreach (var banana in hitBananas) {
				visibleBananas.Remove (banana);
			}

			int ballHitCount = ballsBatch.Children.Count (ball => ball.BoundingBoxTransformedToParent.IntersectsRect (smRect));

			if (ballHitCount > 0) {
				EndGame ();
			}
		}

		void EndGame ()
		{
			// Stop scheduled events as we transition to game over scene
			UnscheduleAll ();

			var gameOverScene = GameOverLayer.SceneWithScore (Window, hitBananas.Count);
			var transitionToGameOver = new CCTransitionMoveInR (0.3f, gameOverScene);

			Director.ReplaceScene (transitionToGameOver);
		}

		void InitExplosionParticles ()
		{
			//BUG: the texture used in the particle system doesn't look correct on Android (probably due to premultiplied setting)
			explosion = new CCParticleSystemQuad ("ExplodingBanana.plist");
			explosion.AutoRemoveOnFinish = false;
			explosion.StopSystem ();
			explosion.Visible = false;
			AddChild (explosion);
		}

		void Explode (CCPoint pt)
		{
			explosion.Visible = true;
			explosion.Position = pt;
			explosion.ResetSystem ();
		}

		bool ShouldEndGame ()
		{
			return elapsedTime > GAME_DURATION;
		}

		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			monkey.StopAllActions ();

			var location = touches [0].LocationOnScreen;
			location = WorldToScreenspace (location); 

			if (location.Y >= allowableMovementRect.Size.Height) {
				location.Y = allowableMovementRect.Size.Height;
			}

			float ds = CCPoint.Distance (monkey.Position, location);
	
			var dt = ds / monkeySpeed;

			var moveMonkey = new CCMoveTo (dt, location);

			//BUG: calling walkRepeat separately as it doesn't run when called in RunActions or CCSpawn
			monkey.RunAction (walkRepeat);
			monkey.RunActions (moveMonkey, walkAnimStop);

			// move the clouds relative to the monkey's movement
			MoveClouds (location.Y - monkey.Position.Y);
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.NoBorder;

			grass.Position = VisibleBoundsWorldspace.Center;

			var b = VisibleBoundsWorldspace;
			sun.Position = b.UpperRight.Offset (-100, -100);

			circleNode.Position = sun.Position;

			AddClouds ();

			AddMonkey ();
			monkey.Position = VisibleBoundsWorldspace.Center;

			InitExplosionParticles ();
		}

		void InitPhysics ()
		{
			CCSize s = Layer.VisibleBoundsWorldspace.Size;

			var gravity = new b2Vec2 (0.0f, -10.0f);
			world = new b2World (gravity);

			world.SetAllowSleeping (true);
			world.SetContinuousPhysics (true);

			var def = new b2BodyDef ();
			def.allowSleep = true;
			def.position = b2Vec2.Zero;
			def.type = b2BodyType.b2_staticBody;
			b2Body groundBody = world.CreateBody (def);
			groundBody.SetActive (true);

			b2EdgeShape groundBox = new b2EdgeShape ();
			groundBox.Set (b2Vec2.Zero, new b2Vec2 (s.Width / PTM_RATIO, 0));
			b2FixtureDef fd = new b2FixtureDef ();
			fd.shape = groundBox;
			groundBody.CreateFixture (fd);
		}

		void AddBall ()
		{
			var contentScale = contentScaleFactorX < contentScaleFactorY ? contentScaleFactorX : contentScaleFactorY; //use the smaller scale if they are not equal

			if (ballsBatch.ChildrenCount < MAX_NUM_BALLS) {
				int idx = (CCRandom.Float_0_1 () > .5 ? 0 : 1);
				int idy = (CCRandom.Float_0_1 () > .5 ? 0 : 1);
				var ballSprite = new CCPhysicsSprite (ballTexture, new CCRect (32 * idx, 32 * idy, 32, 32), PTM_RATIO);
				ballSprite.Scale = contentScale;

				ballsBatch.AddChild (ballSprite);

				CCPoint p = GetRandomPosition (ballSprite.ContentSize);

				ballSprite.Position = new CCPoint (p.X, p.Y);

				var def = new b2BodyDef ();
				def.position = new b2Vec2 (p.X / PTM_RATIO, p.Y / PTM_RATIO);
				def.linearVelocity = new b2Vec2 (0.0f, -1.0f);
				def.type = b2BodyType.b2_dynamicBody;
				b2Body body = world.CreateBody (def);

				var circle = new b2CircleShape ();
				circle.Radius = 0.5f * contentScale;

				var fd = new b2FixtureDef ();
				fd.shape = circle;
				fd.density = 1f;
				fd.restitution = 0.85f;
				fd.friction = 0f;
				body.CreateFixture (fd);

				ballSprite.PhysicsBody = body;
			}
		}

		public override void OnEnter ()
		{
			base.OnEnter ();

			InitPhysics ();
		}

		public static CCScene GameScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);

			float w_baseline = 640.0f;
			float h_baseline = 1136.0f;

			CCSize winSize = mainWindow.WindowSizeInPixels;

			var layer = new GameLayer (winSize.Width / w_baseline, winSize.Height / h_baseline);

			scene.AddChild (layer);

			return scene;
		}
	}
}