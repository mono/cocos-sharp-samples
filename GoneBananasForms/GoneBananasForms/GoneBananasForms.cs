using System;
using System.Collections.Generic;
using Xamarin.Forms;
using CocosSharp;

namespace GoneBananasForms
{
    public class App : Application
    {
        public App ()
        {
            var gamePage = new GamePage ();
            var navPage = new NavigationPage (gamePage);

            var mainPage = new MasterDetailPage () {
                Detail = navPage,
                Master = new GameSettingsPage (gamePage),
                MasterBehavior = MasterBehavior.Popover
            };
            // The root page of your application
            MainPage = mainPage;
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}

