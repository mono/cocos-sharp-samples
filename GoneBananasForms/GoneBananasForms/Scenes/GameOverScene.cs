using System;
using CocosSharp;

namespace GoneBananasForms
{
    public class GameOverScene : CCScene
    {
        CCLayer mainLayer;


        #region Initialisation

        public GameOverScene (int bananasCollected, CCGameView gameView) : base(gameView)
        {
            CreateLayers ();

            CreateMenu (bananasCollected);
        }

        void CreateLayers ()
        {
            mainLayer = new CCLayerColor (CCColor4B.Black);

            this.AddLayer (mainLayer);
        }

        void CreateMenu (int bananasCollected)
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            var gameOverLabel = new CCLabel ( String.Format("Game\nOver\n{0} collected", bananasCollected), "pixelfont", 60, CCLabelFormat.SpriteFont) {
                Color = CCColor3B.Yellow,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            var restartGameLabel = new CCLabel ( "Try again", "pixelfont", 60, CCLabelFormat.SpriteFont) {
                Color = CCColor3B.White,
                HorizontalAlignment = CCTextAlignment.Left,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            var restartGameMenuItem = new CCMenuItemLabel (restartGameLabel, RestartGamePressed);

            var gameMenu = new CCMenu (restartGameMenuItem) {
                AnchorPoint = CCPoint.AnchorMiddle
            };

            mainLayer.AddChild (gameOverLabel);
            mainLayer.AddChild (gameMenu);

            gameOverLabel.Position = new CCPoint (center.X, bounds.Size.Height - gameOverLabel.ContentSize.Height);
            gameMenu.Position = new CCPoint (center.X, bounds.Size.Height / 4.0f);
        }

        #endregion Initialisation


        #region Menu actions

        void RestartGamePressed (object sender)
        {
            var scene = new GameScene (GameView);

            var transitionToGame = new CCTransitionRotoZoom (1.0f, scene);

            GameView.Director.ReplaceScene (transitionToGame);
        }

        #endregion Menu actions
    }
}

