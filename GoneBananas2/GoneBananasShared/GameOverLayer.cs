using System;
using CocosSharp;

namespace GoneBananas
{
	public class GameOverLayer : CCLayerColor
	{

		string scoreMessage = string.Empty;
		float sx, sy;

		public GameOverLayer (int score, float sx, float sy)
		{
			this.sx = sx;
			this.sy = sy;

			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

			AddEventListener (touchListener, this);

			scoreMessage = String.Format ("Game Over.\r\nYou collected {0} bananas!", score);

			Color = CCColor3B.Black;
			Opacity = 255;
		}

		public void AddMonkey ()
		{
			var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
			var frame = spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("frame"));
           
			var monkey = new CCSprite (frame) {
				Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X + 20, VisibleBoundsWorldspace.Size.Center.Y + 300 * sy),
				Scale = 0.5f * (sx > sy ? sx : sy),
			};

			AddChild (monkey);
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

			var scoreLabel = new CCLabel (scoreMessage, "HelveticaNeue-Bold", 32 * (sx > sy ? sx : sy)) {
				Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X, VisibleBoundsWorldspace.Size.Center.Y + 50),
				Color = new CCColor3B (CCColor4B.Yellow),
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			AddChild (scoreLabel);

			var playAgainLabel = new CCLabel ("Tap to Play Again", "HelveticaNeue-Bold", 32 * (sx > sy ? sx : sy)) {
				Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X, VisibleBoundsWorldspace.Size.Center.Y - 50 * sy),
				Color = new CCColor3B (CCColor4B.Green),
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			AddChild (playAgainLabel);

			AddMonkey ();
		}

		public static CCScene SceneWithScore (CCWindow mainWindow, int score)
		{
			var scene = new CCScene (mainWindow);

			var winSize = mainWindow.WindowSizeInPixels;
			const float wbase = 640.0f;
			const float hbase = 1136.0f;

			var layer = new GameOverLayer (score, winSize.Width / wbase, winSize.Height / hbase);

			scene.AddChild (layer);

			return scene;
		}
	}
}