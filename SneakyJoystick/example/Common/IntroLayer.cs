using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CocosDenshion;
using System.Linq;
using CocosSharp;
using CocosSharp.Extensions.SneakyJoystick;
using SneakyJoystickExample.Windows;

namespace SneakyJoystickExample.Common
{
    public class IntroLayer : CCLayerColor
    {

        CCEventListenerCustom joystickListener;
        CCEventListenerCustom buttonListener;


        SneakyPanelControl JoyPanel;

        CCSize winSize;
        CCAnimation walkAnim;
        CCAction action;

        CCSprite avitar;

        private bool IsWalking = false;

        public IntroLayer()
        {

            InitializeJoyPanel();

            InitializeMonkey();

            JoyPanel.SetPlayer(avitar);

        }

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            CCSimpleAudioEngine.SharedEngine.PreloadEffect("sound_oso");

            winSize = windowSize;

            avitar.Position = winSize.Center;

            joystickListener = new CCEventListenerCustom(SneakyPanelControl.JOY_LISTENER_ID, (customEvent) =>
                {
                    var response = customEvent.UserData as SneakyJoystickEventResponse;
                    if (response != null)
                    {

                        switch (response.ResponseType)
                        {
                            case SneakyJoystickMovementStatus.Start:
                                IsWalking = true;
                                Console.WriteLine("Start walk.");
                                break;
                            case SneakyJoystickMovementStatus.End:
                                IsWalking = false;
                                Console.WriteLine("Stop walk.");
                                break;
                            default:
                                break;
                        }

                    }

                });
            EventDispatcher.AddEventListener(joystickListener, 1);

            buttonListener = new CCEventListenerCustom(SneakyPanelControl.BUTTON_LISTENER_ID, (customEvent) =>
                {
                    var response = customEvent.UserData as SneakyButtonEventResponse;
                    if (response != null)
                    {
                        if (response.ID == 1)
                            CCSimpleAudioEngine.SharedEngine.PlayEffect("sound_oso");

                        Console.WriteLine("BUTTON {0} {1}", response.ID, response.ResponseType == SneakyButtonStatus.Press ? "PRESED" : "UNPRESSED");
                    }
                });

            EventDispatcher.AddEventListener(buttonListener, 1);

            Schedule();
        }


        public void InitializeBear()
        {
            //var spriteSheet = new CCSpriteSheet("magic-beard.plist");
            var spriteSheet = new CCSpriteSheet("animations/AnimBear.plist");

            walkAnim = new CCAnimation(spriteSheet.Frames, 0.1f);
            action = new CCRepeatForever(new CCAnimate(walkAnim));
            avitar = new CCSprite(spriteSheet.Frames.First());

            AddChild(avitar);
        }

        public void InitializeMonkey()
        {

            var spriteSheet = new CCSpriteSheet("animations/monkey.plist");

            // Load the frames using the Frames property which
            var animationFrames = spriteSheet.Frames.FindAll((x) =>
                {
                    return x.TextureFilename.StartsWith("frame");
                });

            walkAnim = new CCAnimation(animationFrames, 0.1f);
            action = new CCRepeatForever(new CCAnimate(walkAnim));
            avitar = new CCSprite(animationFrames.First());
            avitar.Scale = 0.5f;

            AddChild(avitar);
        }


        public void InitializeJoyPanel()
        {

            JoyPanel = new SneakyPanelControl(6);
            JoyPanel.IsDebug = true;
            AddChild(JoyPanel, 9999);

        }

        public override void Update(float dt)
        {

            base.Update(dt);

            if (JoyPanel != null)
            {
                avitar.Position = JoyPanel.GetPlayerPosition(dt);

                if (JoyPanel.HasAnyDirection)
                {
                    avitar.FlipX = (JoyPanel.JoyControl.IsRight);
                }

                if (IsWalking && avitar.NumberOfRunningActions == 0)
                    avitar.RunAction(action);

                if (!IsWalking && avitar.NumberOfRunningActions > 0)
                    avitar.StopAllActions();


            }

        }

        public override void OnExit()
        {
            base.OnExit();

            this.EventDispatcher.RemoveEventListener(joystickListener);
            this.EventDispatcher.RemoveEventListener(buttonListener);

        }

        public static CCScene Scene
        {
            get
            {
                // 'scene' is an autorelease object.
                var scene = new CCScene();


                // 'layer' is an autorelease object.
                var layer = new IntroLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;

            }

        }

    }
}

