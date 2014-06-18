using System.Reflection;
using Microsoft.Xna.Framework;
using CocosDenshion;
using SneakyJoystickExample.Common;
using CocosSharp;

namespace SneakyJoystickExample.Windows
{
    public class AppDelegate : CCApplicationDelegate
    {



        int preferredWidth;
        int preferredHeight;

        /// <summary>
        ///  Implement CCDirector and CCScene init code here.
        /// </summary>
        /// <returns>
        ///  true  Initialize success, app continue.
        ///  false Initialize failed, app terminate.
        /// </returns>
        public override void ApplicationDidFinishLaunching(CCApplication application)
        {


            //1280 x 768
#if WINDOWS_PHONE
            preferredWidth = 1280;
            preferredHeight = 768;
#else
            preferredWidth = 1280;
            preferredHeight = 768;
#endif

            application.PreferredBackBufferWidth = preferredWidth;
            application.PreferredBackBufferHeight = preferredHeight;

            application.PreferMultiSampling = true;
            application.ContentRootDirectory = "Content";

            CCDirector director = CCApplication.SharedApplication.MainWindowDirector;
            director.DisplayStats = true;
            director.AnimationInterval = 1.0 / 60;

			var resPolicy = CCResolutionPolicy.ShowAll; // This will letterbox your game

			application.ContentSearchPaths.Add("SD");

			CCSize designSize = new CCSize(480, 320);

			if (CCDrawManager.FrameSize.Height > 320)
			{
				//CCSize resourceSize = new CCSize(960, 640);
				CCSize resourceSize = new CCSize(1280, 768);
				application.ContentSearchPaths.Insert(0,"HD");
				director.ContentScaleFactor = resourceSize.Height / designSize.Height;
			}

			CCDrawManager.SetDesignResolutionSize(designSize.Width, designSize.Height, resPolicy);



            // turn on display FPS
            director.DisplayStats = true;

            // set FPS. the default value is 1.0/60 if you don't call this
            director.AnimationInterval = 1.0 / 60;

            CCScene pScene = IntroLayer.Scene;

            director.RunWithScene(pScene);
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