using System.Reflection;
using Microsoft.Xna.Framework;
using SneakyJoystickExample.Common;
using CocosSharp;

namespace SneakyJoystickExample
{
	public class AppDelegate : CCApplicationDelegate
	{
		float preferredWidth;
		float preferredHeight;

		/// <summary>
		///  Implement CCDirector and CCScene init code here.
		/// </summary>
		/// <returns>
		///  true  Initialize success, app continue.
		///  false Initialize failed, app terminate.
		/// </returns>
		public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
		{

			preferredWidth = application.MainWindow.WindowSizeInPixels.Width;
			preferredHeight = application.MainWindow.WindowSizeInPixels.Height;

			application.ContentRootDirectory = "Content";
			application.ContentSearchPaths.Add("SD");

			CCScene scene = new CCScene(mainWindow);
			CCLayer layer = new IntroLayer(new CCSize(preferredWidth, preferredHeight));

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