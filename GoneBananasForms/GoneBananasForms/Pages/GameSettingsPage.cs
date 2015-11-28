using System;

using Xamarin.Forms;

namespace GoneBananasForms
{
    public class GameSettingsPage : ContentPage
    {
        GamePage gamePage;

        Slider musicVolumeSlider;
        Slider effectsVolumeSlider;

        public GameSettingsPage (GamePage gamePageIn)
        {
            gamePage = gamePageIn;

            Title = "Settings";

            musicVolumeSlider = new Slider { Minimum = 0, Maximum = 1, Value = 1 };
            effectsVolumeSlider = new Slider { Minimum = 0, Maximum = 1, Value = 1 };

            musicVolumeSlider.ValueChanged += MusicVolumeChanged;
            effectsVolumeSlider.ValueChanged += EffectsVolumeChanged;

            Content = new StackLayout () {
                Padding = 20,
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    new Label { Text = "Music volume" },
                    musicVolumeSlider,
                    new Label { Text = "Effects volume" },
                    effectsVolumeSlider
                }
            };
        }

        void MusicVolumeChanged (object sender, ValueChangedEventArgs e)
        {
            if (gamePage != null)
                gamePage.MusicVolume = (float)musicVolumeSlider.Value;
        }

        void EffectsVolumeChanged (object sender, ValueChangedEventArgs e)
        {
            if (gamePage != null)
                gamePage.EffectsVolume = (float)effectsVolumeSlider.Value;
        }
    }
}


