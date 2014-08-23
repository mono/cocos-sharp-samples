using System.Reflection;
using Microsoft.Xna.Framework;
using CocosDenshion;
using CocosSharp;

namespace AngryNinjas
{
	public class AppDelegate : CCApplicationDelegate
	{

		public static CCWindow SharedWindow { get; set; }

		public static CCSize DefaultResolution;


		/// <summary>
		///  Implement CCDirector and CCScene init code here.
		/// </summary>
		/// <returns>
		///  true  Initialize success, app continue.
		///  false Initialize failed, app terminate.
		/// </returns>
		public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
		{

			SharedWindow = mainWindow;

			DefaultResolution = new CCSize(
				application.MainWindow.WindowSizeInPixels.Width,
				application.MainWindow.WindowSizeInPixels.Height);

			application.ContentRootDirectory = "Content";
			application.ContentSearchPaths.Add("SD");

			CCScene scene = new CCScene(mainWindow);
			CCLayer layer = new IntroLayer(DefaultResolution);

			scene.AddChild(layer);

			mainWindow.RunWithScene(scene);
		}

	}

	
}
