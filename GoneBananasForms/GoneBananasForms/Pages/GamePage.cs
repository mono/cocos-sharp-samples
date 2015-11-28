using System;
using System.Collections.Generic;
using Xamarin.Forms;
using CocosSharp;

namespace GoneBananasForms
{
    public class GamePage : ContentPage
    {
        CCGameView nativeGameView;


        #region Properties

        public CocosSharpView GameView { get; private set; }

        public float MusicVolume
        {
            get 
            {
                return nativeGameView != null ? nativeGameView.AudioEngine.BackgroundMusicVolume : 0.0f; 
            }

            set 
            {
                if (nativeGameView != null)
                    nativeGameView.AudioEngine.BackgroundMusicVolume = value;
            }
        }

        public float EffectsVolume
        {
            get 
            {
                return nativeGameView != null ? nativeGameView.AudioEngine.EffectsVolume : 0.0f; 
            }

            set 
            {
                if (nativeGameView != null)
                    nativeGameView.AudioEngine.EffectsVolume = value;
            }
        }

        #endregion Properties


        public GamePage ()
        {
            GameView = new CocosSharpView () {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                MinimumHeightRequest = 300,
                MinimumWidthRequest = 300,
                VerticalOptions = LayoutOptions.FillAndExpand,
                // Set the game world dimensions
                DesignResolution = new Size (1024, 768),
                // Set the method to call once the view has been initialised
                ViewCreated = LoadGame
            };

            Content = GameView;
        }

        protected override void OnDisappearing ()
        {
            if (GameView != null) {
                GameView.Paused = true;
            }

            base.OnDisappearing ();
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing ();

            if (GameView != null)
                GameView.Paused = false;
        }

        void LoadGame (object sender, EventArgs e)
        {
            nativeGameView = sender as CCGameView;

            if (nativeGameView != null) {
                var contentSearchPaths = new List<string> () { "Fonts", "Sounds", "Images" , "Animations" };
                CCSizeI viewSize = nativeGameView.ViewSize;
                CCSizeI designResolution = nativeGameView.DesignResolution;

                nativeGameView.ContentManager.SearchPaths = contentSearchPaths;
               
                nativeGameView.AudioEngine.PreloadBackgroundMusic (GameScene.MusicPath);
                nativeGameView.AudioEngine.PreloadEffect (GameScene.EatingEffectPath);
                nativeGameView.AudioEngine.PreloadEffect (GameScene.HurtEffectPath);

                // Retro game, so we don't want our textures to be antialiased
                CCTexture2D.DefaultIsAntialiased = false;

                CCScene gameScene = new GameStartScene (nativeGameView);

                nativeGameView.RunWithScene (gameScene);
            }
        }
    }
}
