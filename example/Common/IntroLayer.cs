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

        CCEventListenerCustom startMovement;
        CCEventListenerCustom endMovement;
        CCEventListenerCustom buttonEnded;


        SneakyPanelControl JoyPanel;

        CCSize winSize;
        CCAnimation walkAnim;
        CCAction action;

		CCSprite avitar;

        private bool IsWalking = false;

        public IntroLayer()
        {
            Schedule();
        }

		protected override void RunningOnNewWindow(CCSize windowSize)
		{
			base.RunningOnNewWindow(windowSize);

			winSize = windowSize;

			InitializeJoyPanel();

			//InitializeBear();
			InitializeMonkey();

			JoyPanel.SetPlayer(avitar);
		}


        public void InitializeBear()
        {

            //var spriteSheet = new CCSpriteSheet("magic-beard.plist");
			var spriteSheet = new CCSpriteSheet("animations/AnimBear.plist");

            walkAnim = new CCAnimation(spriteSheet.Frames, 0.1f);
            action = new CCRepeatForever(new CCAnimate(walkAnim));
			var frame = spriteSheet.Frames[0];
			avitar = new CCSprite(frame);
            avitar.Position = winSize.Center;

            AddChild(avitar);
        }

		public void InitializeMonkey()
		{

			var spriteSheet = new CCSpriteSheet("animations/monkey.plist");

			// Load the frames using the Frames property which
			var animationFrames = spriteSheet.Frames.FindAll ((x) =>
				{
					return x.TextureFilename.StartsWith ("frame");
				});

			walkAnim = new CCAnimation(animationFrames, 0.1f);
			action = new CCRepeatForever(new CCAnimate(walkAnim));
			avitar = new CCSprite(animationFrames.First());
			avitar.Scale = 0.5f;
			avitar.Position = winSize.Center;

			AddChild(avitar);
		}


        public void InitializeJoyPanel()
        {

            JoyPanel = new SneakyPanelControl();
            JoyPanel.IsDebug = true;
            AddChild(JoyPanel, 9999);

            startMovement = new CCEventListenerCustom(SneakyPanelControl.START_MOVEMENT, (customEvent) =>
            {
                IsWalking = true;
                Console.WriteLine("Walking: " + IsWalking);
            });
            EventDispatcher.AddEventListener(startMovement, 1);

            endMovement = new CCEventListenerCustom(SneakyPanelControl.END_MOVEMENT, (customEvent) =>
                {
                    IsWalking = false;
                    Console.WriteLine("Walking: " + IsWalking);
                });

            EventDispatcher.AddEventListener(endMovement, 1);

            buttonEnded = new CCEventListenerCustom(SneakyPanelControl.END_PRESS_BUTTON, (customEvent) =>
            {

                var button = customEvent.UserData as SneakyButtonControl;
                if (button != null)
                {
                    if (button.ID == 1)
                        CCSimpleAudioEngine.SharedEngine.PlayEffect("sound_oso");

                    Console.WriteLine("BUTTON {0} PRESSED", button.ID);
                }
            });

            EventDispatcher.AddEventListener(buttonEnded, 1);


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

            this.EventDispatcher.RemoveEventListener(startMovement);
            this.EventDispatcher.RemoveEventListener(endMovement);
            this.EventDispatcher.RemoveEventListener(buttonEnded);


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

