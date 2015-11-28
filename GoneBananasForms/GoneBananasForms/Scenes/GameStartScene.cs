using System;
using CocosSharp;

namespace GoneBananasForms
{
    public class GameStartScene : CCScene
    {
        CCLayer mainLayer;

        CCLabel goLabel, bananasLabel;
        CCMenu gameMenu;

        CCFiniteTimeAction scaleLabelAction;
        CCFiniteTimeAction tintLabelAction;

        #region Initialisation

        public GameStartScene (CCGameView gameView) : base(gameView)
        {
            CreateLayers ();

            LoadStartLabels ();

            LoadMenu ();
        }

        void CreateLayers ()
        {
            mainLayer = new CCLayerColor (CCColor4B.Black);

            this.AddLayer (mainLayer);
        }

        void LoadStartLabels ()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            goLabel = new CCLabel ( "Go", "pixelfont", 100, CCLabelFormat.SpriteFont) {
                Color = CCColor3B.Green,
                HorizontalAlignment = CCTextAlignment.Left,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            bananasLabel = new CCLabel ( "Bananas!", "pixelfont", 100, CCLabelFormat.SpriteFont) {
                Color = CCColor3B.Yellow,
                HorizontalAlignment = CCTextAlignment.Left,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            mainLayer.AddChild (goLabel);
            mainLayer.AddChild (bananasLabel);

            var goLabelSize = goLabel.ContentSize;
            var bananasLabelSize = bananasLabel.ContentSize;

            goLabel.Position = new CCPoint (center.X, bounds.Size.Height - goLabelSize.Height);
            bananasLabel.Position = new CCPoint (center.X, bounds.Size.Height - goLabelSize.Height / 2.0f - bananasLabelSize.Height);

            var scaleAction = new CCScaleBy (0.5f, 1.0f, 1.5f);
            var fadeAction = new CCFadeIn (0.5f);

            scaleLabelAction = new CCSequence (new CCSpawn (scaleAction, fadeAction), 
                new CCEaseElasticInOut (scaleAction.Reverse (), 1.0f));

            tintLabelAction = new CCSequence (new CCTintTo (1.0f, 100, 152, 219), 
                new CCTintTo (1.0f, 255, 255, 255));
        }

        void LoadMenu ()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            var startGameLabel = new CCLabel ( "Start game", "pixelfont", 60, CCLabelFormat.SpriteFont) {
                Color = CCColor3B.White,
                HorizontalAlignment = CCTextAlignment.Left,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            var startGameMenuItem = new CCMenuItemLabel (startGameLabel, StartGamePressed);

            gameMenu = new CCMenu (startGameMenuItem) {
                AnchorPoint = CCPoint.AnchorMiddle
            };

            gameMenu.Position = new CCPoint (center.X, bounds.Size.Height / 4.0f);

            mainLayer.AddChild (gameMenu);
        }

        #endregion Initialisation


        public override void OnEnter ()
        {
            base.OnEnter ();

            gameMenu.Opacity = 0;
            goLabel.Opacity = 0;
            bananasLabel.Opacity = 0;

            goLabel.RunActionsAsync (new CCDelayTime (1.5f), scaleLabelAction);
            bananasLabel.RunActionsAsync (new CCDelayTime (2.0f), scaleLabelAction);
            gameMenu.RunActionsAsync (new CCDelayTime (3.0f), new CCFadeIn (0.5f), tintLabelAction);

            PlayMusic ();
        }

        async void PlayMusic ()
        {
            // Wait a bit before we play the music -- adds to the drama of the scene
            await System.Threading.Tasks.Task.Delay (3000);

            if (!GameView.AudioEngine.BackgroundMusicPlaying)
                GameView.AudioEngine.PlayBackgroundMusic (GameScene.MusicPath, true);
        }

        #region Menu actions

        void StartGamePressed (object sender)
        {
            var scene = new GameScene (GameView);

            var transitionToGame = new CCTransitionRotoZoom (1.0f, scene);

            GameView.Director.ReplaceScene (transitionToGame);
        }

        #endregion Menu actions
    }
}

