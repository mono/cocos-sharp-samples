using System.Reflection;
using Microsoft.Xna.Framework;
using CocosSharp;
using CocosDenshion;

namespace CocosSharpBox2d.Shared
{
    public class AppDelegate : CCApplicationDelegate
    {

        public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
        {
            application.ContentRootDirectory = "Content";
            var windowSize = mainWindow.WindowSizeInPixels;

            var desiredWidth = 1024.0f;
            var desiredHeight = 768.0f;

            // This will set the world bounds to be (0,0, w, h)
            // CCSceneResolutionPolicy.ShowAll will ensure that the aspect ratio is preserved
            CCScene.SetDefaultDesignResolution(desiredWidth, desiredHeight, CCSceneResolutionPolicy.ShowAll);

            // Determine whether to use the high or low def versions of our images
            // Make sure the default texel to content size ratio is set correctly
            // Of course you're free to have a finer set of image resolutions e.g (ld, hd, super-hd)
            if (desiredWidth < windowSize.Width)
            {
                application.ContentSearchPaths.Add("hd");
            }
            else
            {
                application.ContentSearchPaths.Add("ld");
            }

            CCSprite.DefaultTexelToContentSizeRatio = 1.0f;

            var scene = new CCScene(mainWindow);
            var introLayer = new IntroLayer();

            mainWindow.DisplayStats = true;

            #if ANDROID
            var display = Game.Activity.WindowManager.DefaultDisplay;
            var metrics = new Android.Util.DisplayMetrics();
            display.GetMetrics(metrics);
            mainWindow.StatsScale = (int)metrics.ScaledDensity ;
            #endif

            scene.AddChild(introLayer);

            mainWindow.RunWithScene(scene);
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