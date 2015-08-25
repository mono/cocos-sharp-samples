using System;
using System.Collections.Generic;
using CocosSharp;
using CocosDenshion;

namespace AudioSample
{
    public class GameLayer : CCLayerColor
    {

        CCPoint beginPos;
        int soundId;

        #region Button
        class Button : CCNode
        {

            CCNode child;

            public event TriggeredHandler Triggered;
            // A delegate type for hooking up button triggered events
            public delegate void TriggeredHandler(object sender,EventArgs e);


            private Button()
            {
                AttachListener();
            }

            public Button(CCSprite sprite)
                : this()
            {
                child = sprite;
                AddChild(sprite);
            }


            public Button(string text)
                : this()
            {
                child = new CCLabel(text, "arial", 16, CCLabelFormat.SystemFont);
                AddChild(child);

            }

            public string Text
            {
                get { return (child as CCLabel == null) ? string.Empty : ((CCLabel)child).Text; }
                set 
                {
                    if ((child as CCLabel) == null)
                        return;

                    ((CCLabel)child).Text = value;
                }
            }

            void AttachListener()
            {
                // Register Touch Event
                var listener = new CCEventListenerTouchOneByOne();
                listener.IsSwallowTouches = true;

                listener.OnTouchBegan = OnTouchBegan;
                listener.OnTouchEnded = OnTouchEnded;
                listener.OnTouchCancelled = OnTouchCancelled;

                AddEventListener(listener, this);
            }

            bool touchHits(CCTouch  touch)
            {
                var location = touch.Location;
                var area = child.BoundingBox;
                return area.ContainsPoint(child.WorldToParentspace(location));
            }

            bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
            {
                bool hits = touchHits(touch);
                if (hits)
                    scaleButtonTo(0.9f);

                return hits;
            }

            void OnTouchEnded(CCTouch  touch, CCEvent  touchEvent)
            {
                bool hits = touchHits(touch);
                if (hits && Triggered != null)
                    Triggered(this, EventArgs.Empty);
                scaleButtonTo(1);
            }

            void OnTouchCancelled(CCTouch touch, CCEvent  touchEvent)
            {
                scaleButtonTo(1);
            }

            void scaleButtonTo(float scale)
            {
                var action = new CCScaleTo(0.1f, scale);
                action.Tag = 900;
                StopAction(900);
                RunAction(action);
            }
        }
        #endregion

        #region AudioSlider
        class AudioSlider : CCNode
        {

            CCControlSlider slider;
            CCLabel lblMinValue, lblMaxValue;
            Direction direction;

            public enum Direction
            {
                Vertical,
                Horizontal
            }

            public AudioSlider(Direction direction = Direction.Horizontal)
            {
                slider = new CCControlSlider("images/sliderTrack.png", "images/sliderProgress.png", "images/sliderThumb.png");
                slider.Scale = 0.5f;
                this.direction = direction;
                if (direction == Direction.Vertical)
                    slider.Rotation = -90.0f;
                AddChild(slider);
                ContentSize = slider.ScaledContentSize;
            }

            public float Value
            {
                get { return slider.Value; }

                set
                {
                    SetValue(slider.MinimumValue, slider.MaximumValue, value);
                }
            }

            public void SetValue(float minValue, float maxValue, float value)
            {
                slider.MinimumValue = minValue;
                slider.MaximumValue = maxValue;
                slider.Value = value;

                var valueText = string.Format("{0,2:f2}", minValue);
                if (lblMinValue == null)
                {
                    lblMinValue = new CCLabel(valueText, "arial", 10, CCLabelFormat.SystemFont) { AnchorPoint = CCPoint.AnchorMiddleLeft };
                    AddChild(lblMinValue);

                    if (direction == Direction.Vertical)
                        lblMinValue.Position = new CCPoint(0, slider.ScaledContentSize.Height);
                    else
                        lblMinValue.Position = new CCPoint(0, slider.ScaledContentSize.Height * 1.5f);
                }
                else
                    lblMinValue.Text = valueText;

                valueText = string.Format("{0,2:f2}", maxValue);
                if (lblMaxValue == null)
                {
                    lblMaxValue = new CCLabel(valueText, "arial", 10, CCLabelFormat.SystemFont) { AnchorPoint = CCPoint.AnchorMiddleRight };
                    AddChild(lblMaxValue);

                    if (direction == Direction.Vertical)
                    {
                        lblMaxValue.Position = new CCPoint(slider.ScaledContentSize.Height * 1.75f, slider.ScaledContentSize.Width);
                        AnchorPoint = CCPoint.AnchorMiddleLeft;
                    }
                    else
                        lblMaxValue.Position = new CCPoint(slider.ScaledContentSize.Width, slider.ScaledContentSize.Height * 1.5f);
                }
                else
                    lblMaxValue.Text = valueText;
            }

        }

        #endregion

        float MusicVolume { get; set; }
        float EffectsVolume { get; set; }

        List<string> soundEffects = new List<string> { "effect1", "break1", "gong", "grunt1", "impact1", "whoosh", "bloop" };

        List<string> soundBackgrounds = new List<string> { "background","birds", "drums", "frogs" };

        #region Constructors

        public GameLayer()
        {

            MusicVolume = 1;
            EffectsVolume = 1;

            Color = CCColor3B.Blue;
            Opacity = 255;

            Schedule( (dt) => 

                {
                    var musicVolume = sliderMusicVolume.Value;
                    if ((float)Math.Abs(musicVolume - MusicVolume) > 0.001) 
                    {
                        MusicVolume = musicVolume;
                        CCSimpleAudioEngine.SharedEngine.BackgroundMusicVolume = musicVolume;
                    }

                    var effectsVolume = sliderEffectsVolume.Value;
                    if ((float)Math.Abs(effectsVolume - EffectsVolume) > 0.001) 
                    {
                        EffectsVolume = effectsVolume;
                        CCSimpleAudioEngine.SharedEngine.EffectsVolume = effectsVolume;
                    }
                }
            );

            // preload background music and effect

            foreach (var bg in soundBackgrounds)
                CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic("sounds/" + bg);

            foreach(var ef in soundEffects)
                CCSimpleAudioEngine.SharedEngine.PreloadEffect("sounds/" + ef);

            // set default volume
            CCSimpleAudioEngine.SharedEngine.EffectsVolume = 0.5f;
            CCSimpleAudioEngine.SharedEngine.BackgroundMusicVolume = 0.5f;

            CCLog.Log("Audio initialized to: " + CCSimpleAudioEngine.SharedEngine.BackgroundMusicVolume);

        }

        #endregion Constructors


        #region Setup content

        public override void OnEnter()
        {
            base.OnEnter(); 

            AddButtons();
            AddSliders();

            AddSoundSelections();
        }

        void AddButtons()
        {
            var audio = CCSimpleAudioEngine.SharedEngine;

            var lblMusic = new CCLabel("Control Music", "Arial", 48, CCLabelFormat.SystemFont);
            AddChildAt(lblMusic, 0.25f, 0.9f);

            var btnPlay = new Button("play");
            btnPlay.Triggered += (sender, e) =>
                {
                    var musicFile = btnCurrentBackground.Text;
                    musicFile = "sounds/"+ musicFile.Substring(2, musicFile.Length - 4).Trim();
                    audio.PlayBackgroundMusic(musicFile, true);
                };
            AddChildAt(btnPlay, 0.1f, 0.75f);

            var btnStop = new Button("stop");
            btnStop.Triggered += (sender, e) =>
                {
                    audio.StopBackgroundMusic();
                };

            AddChildAt(btnStop, 0.25f, 0.75f);

            var btnRewindMusic = new Button("rewind");
            btnRewindMusic.Triggered += (sender, e) =>
                {
                    audio.RewindBackgroundMusic();
                };

            AddChildAt(btnRewindMusic, 0.4f, 0.75f);

            var btnPause = new Button("pause");
            btnPause.Triggered += (sender, e) =>
                {
                    audio.PauseBackgroundMusic();
                };
            AddChildAt(btnPause, 0.1f, 0.65f);

            var btnResumeMusic = new Button("resume");
            btnResumeMusic.Triggered += (sender, e) =>
                {
                    audio.ResumeBackgroundMusic();
                };
            AddChildAt(btnResumeMusic, 0.25f, 0.65f);

            var btnIsPlayingMusic = new Button("is playing");
            btnIsPlayingMusic.Triggered += (sender, e) =>
                {
                    if (CCSimpleAudioEngine.SharedEngine.BackgroundMusicPlaying)
                        CCLog.Log("background music is playing");
                    else
                        CCLog.Log("background music is not playing");
                };
            AddChildAt(btnIsPlayingMusic, 0.4f, 0.65f);

            var lblSound = new CCLabel("Control Effects", "arial", 48, CCLabelFormat.SystemFont);
            AddChildAt(lblSound, 0.75f, 0.9f);

            var btnPlayEffect = new Button("play");
            btnPlayEffect.Triggered += (sender, e) =>
                {
                    var effectFile = btnCurrentEffect.Text;
                    effectFile = "sounds/"+ effectFile.Substring(2, effectFile.Length - 4).Trim();
                    var pitch = sliderPitch.Value;
                    var pan = sliderPan.Value;
                    var gain = sliderGain.Value;
                    soundId = audio.PlayEffect(effectFile, false);//, pitch, pan, gain);
                };
            AddChildAt(btnPlayEffect, 0.6f, 0.8f);

            var btnPlayEffectInLoop = new Button("play in loop");
            btnPlayEffectInLoop.Triggered += (sender, e) =>
                {
                    var effectFile = btnCurrentEffect.Text;
                    effectFile = "sounds/"+ effectFile.Substring(2, effectFile.Length - 4).Trim();
                    var pitch = sliderPitch.Value;
                    var pan = sliderPan.Value;
                    var gain = sliderGain.Value;
                    soundId = audio.PlayEffect(effectFile, true);//, pitch, pan, gain);
                };
            AddChildAt(btnPlayEffectInLoop, 0.75f, 0.8f);

            var btnStopEffect = new Button("stop");
            btnStopEffect.Triggered += (sender, e) =>
                {
                    audio.StopEffect(soundId);
                };
            AddChildAt(btnStopEffect, 0.9f, 0.8f);

            var btnUnloadEffect = new Button("unload");
            btnUnloadEffect.Triggered += (sender, e) =>
                {
                    var effectFile = btnCurrentEffect.Text;
                    effectFile = "sounds/"+ effectFile.Substring(2, effectFile.Length - 4).Trim();
                    audio.UnloadEffect(effectFile);
                };
            AddChildAt(btnUnloadEffect, 0.6f, 0.7f);

            var btnPauseEffect = new Button("pause");
            btnPauseEffect.Triggered += (sender, e) =>
                {
                    audio.PauseEffect(soundId);
                };
            AddChildAt(btnPauseEffect, 0.75f, 0.7f);

            var btnResumeEffect = new Button("resume");
            btnResumeEffect.Triggered += (sender, e) =>
                {
                    audio.ResumeEffect(soundId);
                };
            AddChildAt(btnResumeEffect, 0.9f, 0.7f);

            var btnPauseAll = new Button("pause all");
            btnPauseAll.Triggered += (sender, e) =>
                {
                    audio.PauseAllEffects();
                };
            AddChildAt(btnPauseAll, 0.6f, 0.6f);

            var btnResumeAll = new Button("resume all");
            btnResumeAll.Triggered += (sender, e) =>
                {
                    audio.ResumeAllEffects();
                };
            AddChildAt(btnResumeAll, 0.75f, 0.6f);

            var btnStopAll = new Button("stop all");
            btnStopAll.Triggered += (sender, e) =>
                {
                    audio.StopAllEffects();
                };
            AddChildAt(btnStopAll, 0.9f, 0.6f);
        }

        AudioSlider sliderPitch, sliderMusicVolume, sliderEffectsVolume, sliderPan, sliderGain;

        void AddSliders()
        {
            //            var lblPitch = new CCLabel("Pitch", "arial", 14, CCLabelFormat.SystemFont);
            //            AddChildAt(lblPitch, 0.67f, 0.4f);
            //
            sliderPitch = new AudioSlider(AudioSlider.Direction.Horizontal);
            sliderPitch.SetValue(0.5f, 2, 1);
            //            AddChildAt(sliderPitch, 0.72f, 0.39f);

            //            var lblPan = new CCLabel("Pan", "arial", 14, CCLabelFormat.SystemFont);
            //            AddChildAt(lblPan, 0.67f, 0.3f);
            sliderPan = new AudioSlider();
            sliderPan.SetValue(-1, 1, 0);
            //            AddChildAt(sliderPan, 0.72f, 0.29f);
            //
            //            var lblGain = new CCLabel("Gain", "arial", 14, CCLabelFormat.SystemFont);
            //            AddChildAt(lblGain, 0.67f, 0.2f);
            sliderGain = new AudioSlider();
            sliderGain.SetValue(0, 1, 1);
            //            AddChildAt(sliderGain, 0.72f, 0.19f);

            var lblEffectsVolume = new CCLabel("Effects Volume", "arial", 16, CCLabelFormat.SystemFont);
            AddChildAt(lblEffectsVolume, 0.62f, 0.5f);
            sliderEffectsVolume = new AudioSlider();
            sliderEffectsVolume.SetValue(0, 1, CCSimpleAudioEngine.SharedEngine.EffectsVolume);
            AddChildAt(sliderEffectsVolume, 0.71f, 0.49f);

            var lblMusicVolume = new CCLabel("Music Volume", "arial", 16, CCLabelFormat.SystemFont);
            AddChildAt(lblMusicVolume, 0.12f, 0.5f);

            sliderMusicVolume = new AudioSlider();
            sliderMusicVolume.SetValue(0, 1, CCSimpleAudioEngine.SharedEngine.BackgroundMusicVolume);
            AddChildAt(sliderMusicVolume, 0.21f, 0.49f);
        }

        Button btnCurrentEffect, btnCurrentBackground;

        void AddSoundSelections()
        {
            soundEffects.Sort();

            var lblEffectFiles = new CCLabel("Current Effect", "arial", 14, CCLabelFormat.SystemFont);
            AddChildAt(lblEffectFiles, 0.62f, 0.21f);

            var currentEffect = 0;
            var effectsCount = soundEffects.Count;

            btnCurrentEffect = new Button("< " + soundEffects[currentEffect] + " >");
            btnCurrentEffect.Triggered += (sender, e) =>
                {
                    currentEffect++;
                    currentEffect %= effectsCount;
                    btnCurrentEffect.Text = "< " + soundEffects[currentEffect] + " >";
                };
            AddChildAt(btnCurrentEffect, 0.73f, 0.21f);

            soundBackgrounds.Sort();

            var lblBackgroundFiles = new CCLabel("Current Background", "arial", 14, CCLabelFormat.SystemFont);
            AddChildAt(lblBackgroundFiles, 0.12f, 0.21f);

            var currentBackground = 0;
            var backgoundsCount = soundBackgrounds.Count;

            btnCurrentBackground = new Button("< " + soundBackgrounds[currentBackground] + " >");
            btnCurrentBackground.Triggered += (sender, e) =>
                {
                    currentBackground++;
                    currentBackground %= backgoundsCount;
                    btnCurrentBackground.Text = "< " + soundBackgrounds[currentBackground] + " >";
                };
            AddChildAt(btnCurrentBackground, 0.24f, 0.21f);
        }

        void AddChildAt(CCNode node, float percentageX, float percentageY)
        {
            var size = VisibleBoundsWorldspace.Size;
            node.PositionX = percentageX * size.Width;
            node.PositionY = percentageY * size.Height;
            AddChild(node);
        }

        #endregion Setup content

        public override void OnExit()
        {
            base.OnExit();

            CCSimpleAudioEngine.SharedEngine.End();
        }

    }
}
