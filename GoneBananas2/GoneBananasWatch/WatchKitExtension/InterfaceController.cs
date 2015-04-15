using System;

using WatchKit;
using Foundation;
using WormHoleSharp;

namespace WatchKitExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		Wormhole wormHole;

		public InterfaceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			wormHole = new Wormhole ("group.com.mikebluestein.GoneBananas", "messageDir");
		}

		public override void WillActivate ()
		{
		}

		public override void DidDeactivate ()
		{
		}

		partial void OnRight ()
		{
			wormHole.PassMessage ("dpad", 1);
		}

		partial void OnLeft ()
		{
			wormHole.PassMessage ("dpad", 2);
		}

		partial void OnUp ()
		{
			wormHole.PassMessage ("dpad", 3);
		}

		partial void OnDown ()
		{
			wormHole.PassMessage ("dpad", 4);
		}
	}
}

