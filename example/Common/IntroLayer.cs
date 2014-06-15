using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CocosDenshion;
using System.Linq;
using CocosSharp;
using CocosSharp.IO.SneakyJoystick;

namespace SneakyJoystickExample.Common
{
    public class IntroLayer : CCLayerColor
    {
	
        SneakyPanelControl JoyPanel;

        CCSize winSize;
        CCAnimation walkAnim;
        CCAction action;

        CCSprite bear;

		private bool IsWalking = false;

        public IntroLayer()
        {

            winSize = Director.WindowSizeInPoints;

            InitializeJoyPanel();

            InitializeBear();

            JoyPanel.SetPlayer(bear);

            Schedule();
        }

        public void ReadKeys(float f)
        {

        }

        public void InitializeBear()
        {

            CCSpriteSheet spriteSheet = new CCSpriteSheet("magic-beard.plist");

            walkAnim = new CCAnimation(spriteSheet.Frames, 0.1f);
            action = new CCRepeatForever(new CCAnimate(walkAnim));

            bear = new CCSprite(spriteSheet.Frames.FirstOrDefault());
			bear.Position = winSize.Center;

            AddChild(bear);
        }

		CCEventListenerCustom startMovement;
		CCEventListenerCustom endMovement;

		CCEventListenerCustom button1StartPress;
		CCEventListenerCustom button1EndPress;
		CCEventListenerCustom button2StartPress;
		CCEventListenerCustom button2EndPress;
		CCEventListenerCustom button3StartPress;
		CCEventListenerCustom button3EndPress;
		CCEventListenerCustom button4StartPress;
		CCEventListenerCustom button4EndPress;

        public void InitializeJoyPanel()
        {

            JoyPanel = new SneakyPanelControl();

			startMovement = new CCEventListenerCustom(SneakyPanelControl.START_MOVEMENT, (customEvent) =>
				{
					IsWalking = true;
				});

			EventDispatcher.AddEventListener(startMovement, 1);

			endMovement = new CCEventListenerCustom(SneakyPanelControl.END_MOVEMENT, (customEvent) =>
				{
					IsWalking = false;
				});

			EventDispatcher.AddEventListener(endMovement, 1);


			button1StartPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_1_START_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 1 Start Press");
					//CCSimpleAudioEngine.SharedEngine.PlayEffect("sound_oso");
				});


			button1EndPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_1_END_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 1 End Press");
				});


			button2StartPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_2_START_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 2 Start Press");
				});


			button2EndPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_2_END_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 2 End Press");
				});


			button3StartPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_3_START_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 3 Start Press");
				});


			button3EndPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_3_END_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 3 End Press");
				});


			button4StartPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_4_START_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 4 Start Press");
				});


			button4EndPress = new CCEventListenerCustom(SneakyPanelControl.BUTTON_4_END_PRESS, (customEvent) =>
				{
					Console.WriteLine("Button 4 End Press");
				});

			EventDispatcher.AddEventListener(button1StartPress, 1);
			EventDispatcher.AddEventListener(button1EndPress, 1);
			EventDispatcher.AddEventListener(button2StartPress, 1);
			EventDispatcher.AddEventListener(button2EndPress, 1);
			EventDispatcher.AddEventListener(button3StartPress, 1);
			EventDispatcher.AddEventListener(button3EndPress, 1);
			EventDispatcher.AddEventListener(button4StartPress, 1);
			EventDispatcher.AddEventListener(button4EndPress, 1);


			JoyPanel.IsDebug = true;

			AddChild(JoyPanel, 9999);

        }

        public override void Update(float dt)
        {

            base.Update(dt);

            if (JoyPanel != null)
            {
                bear.Position = JoyPanel.GetPlayerPosition(bear, dt);

                if (JoyPanel.HasAnyDirection)
                {
                    bear.FlipX = (JoyPanel.JoyControl.IsRight);
                }

                if (IsWalking && bear.NumberOfRunningActions == 0)
                    bear.RunAction(action);

                if (!IsWalking && bear.NumberOfRunningActions > 0)
                    bear.StopAllActions();


            }

        }

		public override void OnExit()
		{
			// Don't forget to remove the fixed priority Event listeners yourself.
			this.EventDispatcher.RemoveEventListener(startMovement);
			this.EventDispatcher.RemoveEventListener(endMovement);

			this.EventDispatcher.RemoveEventListener(button1StartPress);
			this.EventDispatcher.RemoveEventListener(button1EndPress);
			this.EventDispatcher.RemoveEventListener(button2StartPress);
			this.EventDispatcher.RemoveEventListener(button2EndPress);
			this.EventDispatcher.RemoveEventListener(button3StartPress);
			this.EventDispatcher.RemoveEventListener(button3EndPress);
			this.EventDispatcher.RemoveEventListener(button4StartPress);
			this.EventDispatcher.RemoveEventListener(button4EndPress);


			base.OnExit();
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

