using CocosSharp;
using CocosDenshion;

namespace TextField
{
    public class AppDelegate : CCApplicationDelegate
    {

        public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
        {

#if DEBUG
            // Example of using a custom log action as a Lambda.
            CCLog.Logger = (format, args) =>
            {
                System.Diagnostics.Debug.WriteLine("TextField: " + format, args);
            };
#endif

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
                CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
            }
            else
            {
                application.ContentSearchPaths.Add("ld");
                CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
            }

            var scene = new CCScene(mainWindow);
            var introLayer = new IntroLayer();

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

    public static class GeometryExtensionHelpers
    {
        public static CCPoint Left(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X, visibleRect.Origin.Y + visibleRect.Size.Height / 2);
        }

        public static CCPoint Right(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X + visibleRect.Size.Width,
                visibleRect.Origin.Y + visibleRect.Size.Height / 2);
        }

        public static CCPoint Top(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X + visibleRect.Size.Width / 2,
                visibleRect.Origin.Y + visibleRect.Size.Height);
        }

        public static CCPoint Bottom(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X + visibleRect.Size.Width / 2, visibleRect.Origin.Y);
        }

        public static CCPoint Center(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X + visibleRect.Size.Width / 2,
                visibleRect.Origin.Y + visibleRect.Size.Height / 2);
        }

        public static CCPoint LeftTop(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X, visibleRect.Origin.Y + visibleRect.Size.Height);
        }

        public static CCPoint RightTop(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X + visibleRect.Size.Width,
                visibleRect.Origin.Y + visibleRect.Size.Height);
        }

        public static CCPoint LeftBottom(this CCRect visibleRect)
        {
            return visibleRect.Origin;
        }

        public static CCPoint RightBottom(this CCRect visibleRect)
        {
            return new CCPoint(visibleRect.Origin.X + visibleRect.Size.Width, visibleRect.Origin.Y);
        }
    }

}