using System.Reflection;
using Microsoft.Xna.Framework;
using CocosDenshion;
using AirHockey.Common;
using CocosSharp;

namespace AirHockey.Store
{
    public class AppDelegate : CCApplicationDelegate
    {

		static CCWindow sharedWindow;

		public static CCWindow SharedWindow
		{
			get { return sharedWindow; }
		}

		public static CCSize DefaultResolution;

		public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
		{
			application.ContentRootDirectory = "Content";

			sharedWindow = mainWindow;

			DefaultResolution = new CCSize(
		application.MainWindow.WindowSizeInPixels.Width,
		application.MainWindow.WindowSizeInPixels.Height);

			CCScene scene = new CCScene(sharedWindow);
			CCLayer layer = new IntroLayer(DefaultResolution);

			scene.AddChild(layer);
			sharedWindow.RunWithScene(scene);
		}

		public override void ApplicationDidEnterBackground(CCApplication application)
		{
			application.Paused = true;
		}

		public override void ApplicationWillEnterForeground(CCApplication application)
		{
			application.Paused = false;
		}

    }
}