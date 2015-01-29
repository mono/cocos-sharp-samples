using System;
using CocosSharp;


namespace MultipleViewports
{
    public class GameAppDelegate : CCApplicationDelegate
    {
        public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
        {
            application.PreferMultiSampling = false;
            application.ContentRootDirectory = "Content";
            application.ContentSearchPaths.Add("animations");
            application.ContentSearchPaths.Add("fonts");
            application.ContentSearchPaths.Add("sounds");

            CCSize windowSize = mainWindow.WindowSizeInPixels;

            float desiredWidth = 1024.0f;
            float desiredHeight = 768.0f;
            
            // This will set the world bounds to be (0,0, w, h)
            // Use CCSceneResolutionPolicy.Custom as we're manually setting our viewports
            CCScene.SetDefaultDesignResolution(desiredWidth, desiredHeight, CCSceneResolutionPolicy.Custom);
            
            // Determine whether to use the high or low def versions of our images
            // Make sure the default texel to content size ratio is set correctly
            // Of course you're free to have a finer set of image resolutions e.g (ld, hd, super-hd)
            if (desiredWidth < windowSize.Width)
            {
                application.ContentSearchPaths.Add("images/hd");
                CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
            }
            else
            {
                application.ContentSearchPaths.Add("images/ld");
                CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
            }
            
            var topViewport = new CCViewport(new CCRect(0.0f, 0.2f, .5f, 0.6f));
            var bottomViewport = new CCViewport(new CCRect(0.5f, 0.0f, .5f, 1.0f));

            // Each viewport needs its own director
            var sceneDirector1 = new CCDirector();
            var sceneDirector2 = new CCDirector();

            var scene1 = new CCScene(mainWindow, topViewport, sceneDirector1);
            var scene2 = new CCScene(mainWindow, bottomViewport, sceneDirector2);

            var layer1 = new GameLayer();
            var layer2 = new GameLayer();

            scene1.AddChild(layer1);
            scene2.AddChild(layer2);

            sceneDirector1.RunWithScene(scene1);
            sceneDirector2.RunWithScene(scene2);
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
