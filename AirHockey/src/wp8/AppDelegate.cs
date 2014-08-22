using System.Reflection;
using Microsoft.Xna.Framework;
using CocosDenshion;
using AirHockey.Common;
using CocosSharp;

namespace AirHockey.WP8
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

            //mainWindow.SetDesignResolutionSize(960, 640, CCSceneResolutionPolicy.ShowAll);
            //application.HandleMediaStateAutomatically = false;
            //mainWindow.DisplayStats = true;

            CCScene scene = new CCScene(sharedWindow);
            CCLayer layer = new IntroLayer(DefaultResolution);

            scene.AddChild(layer);
            sharedWindow.RunWithScene(scene);
        }

     
    }
}