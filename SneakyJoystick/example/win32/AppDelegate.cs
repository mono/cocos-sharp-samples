using System.Reflection;
using Microsoft.Xna.Framework;
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

            application.ContentRootDirectory = "Content";
            application.ContentSearchPaths.Add("SD");

            CCRect boundsRect = new CCRect(0.0f, 0.0f, 960, 640);

            CCViewport viewport = new CCViewport(new CCRect (0.0f, 0.0f, 1.0f, 1.0f));
            CCWindow window = application.MainWindow;
            CCCamera camera = new CCCamera(boundsRect.Size, new CCPoint3(boundsRect.Center, 100.0f), new CCPoint3(boundsRect.Center, 0.0f));
            CCDirector director = new CCDirector();

            window.AddSceneDirector(director);

            CCScene scene = new CCScene(window, viewport, director);
            CCLayer layer = new IntroLayer();
            layer.Camera = camera;

            scene.AddChild(layer);

            director.RunWithScene(scene);
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