using System;
using MonoMac;
using MonoMac.AppKit;
using MonoMac.Foundation;
using CocosSharp;

namespace SneakyJoystickExample.Windows
{
	class Program : NSApplicationDelegate 
	{
		//Game1 game;

		static void Main (string[] args)
		{
			NSApplication.Init ();

			using (var p = new NSAutoreleasePool()) 
			{
				NSApplication.SharedApplication.Delegate = new Program();
				NSApplication.Main(args);
			}
		}

		public override void FinishedLaunching (NSObject notification)
		{
            CCApplication application = new CCApplication(false, new CCSize(1024f, 768f));
            application.ApplicationDelegate = new AppDelegate();

            application.StartGame();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}

