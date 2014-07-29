using System.Reflection;
using Microsoft.Xna.Framework;
using SneakyJoystickExample.Common;
using CocosSharp;

namespace SneakyJoystickExample
{
    public class AppDelegate : CCApplicationDelegate
    {
        public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
        {
            application.ContentRootDirectory = "Content";
            application.ContentSearchPaths.Add("SD");

            CCSize visibleBoundsDimension = new CCSize(960, 640);

            CCScene scene = new CCScene(mainWindow);
            CCLayer layer = new IntroLayer(visibleBoundsDimension);

            scene.AddChild(layer);

            mainWindow.RunWithScene(scene);
        }
    }
}