using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using CocosSharp;
namespace AirHockey.Windows
{

    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            CCApplication sharedApp = CCApplication.SharedApplication;
            sharedApp.ApplicationDelegate = new AppDelegate();
            CCApplication.SharedApplication.StartGame();
        }
    }


}

