using System.Reflection;
using Microsoft.Xna.Framework;
using CocosDenshion;
using RocketThrought.Common;
using CocosSharp;

namespace RocketThrought.Windows
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



		/// <summary>
		/// The function be called when the application enters the background
		/// </summary>
		//		public override void ApplicationDidEnterBackground()
		//		{
		//			// stop all of the animation actions that are running.
		//			CCDirector.SharedDirector.Pause();
		//
		//			// if you use SimpleAudioEngine, your music must be paused
		//			//CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic = true;
		//		}

		/// <summary>
		/// The function be called when the application enter foreground  
		/// </summary>
		//		public override void ApplicationWillEnterForeground()
		//		{
		//			CCDirector.SharedDirector.Resume();
		//
		//			// if you use SimpleAudioEngine, your background music track must resume here. 
		//			//CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic = false;
		//
		//		}
	}
}