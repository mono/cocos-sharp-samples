using System;
using System.Diagnostics;
using CocosSharp;

namespace GoneBananas
{

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			CCApplication application = new CCApplication(false, new CCSize(1024f, 768f));
            application.ApplicationDelegate = new GoneBananasApplicationDelegate();
			application.StartGame();
		}
	}


}

