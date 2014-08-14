using CocosSharp;
using System;
using System.Diagnostics;

#if MACOS
using MonoMac.AppKit;
using MonoMac;
#endif

namespace AngryNinjas
{

    #if WINDOWS
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			CCApplication application = new CCApplication(false, new CCSize(1024f, 768f));
			application.ApplicationDelegate = new AppDelegate();
			application.StartGame();
		}
	}
    #endif

    #if MACOS
    class Program : NSApplicationDelegate 
    {
        public override void FinishedLaunching(MonoMac.Foundation.NSObject notification)
        {
            CCApplication application = new CCApplication(false, new CCSize(1024f, 768f));
            application.ApplicationDelegate = new AppDelegate();

            application.StartGame();
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }

        // This is the main entry point of the application.
        static void Main (string[] args)
        {
            NSApplication.Init ();

            using (var p = new MonoMac.Foundation.NSAutoreleasePool ()) 
            {
                NSApplication.SharedApplication.Delegate = new Program();
                NSApplication.Main(args);
            }
        }
    }
    #endif

}

