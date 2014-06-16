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

        List<CCEventListenerCustom> listener_buttons;

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


        public void InitializeBear()
        {

            CCSpriteSheet spriteSheet = new CCSpriteSheet("magic-beard.plist");

            walkAnim = new CCAnimation(spriteSheet.Frames, 0.1f);
            action = new CCRepeatForever(new CCAnimate(walkAnim));

            bear = new CCSprite(spriteSheet.Frames.FirstOrDefault());
            bear.Position = winSize.Center;

            AddChild(bear);
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

            listener_buttons = new List<CCEventListenerCustom>();
            CCEventListenerCustom tmp;

            foreach (var item in JoyPanel.Buttons)
            {
                tmp = new CCEventListenerCustom(item.EVENT_END_PRESS_ID, (customEvent) =>
                {
                    Console.WriteLine("PRESSED: " + item.ID);

                    if (item.ID == 1)
                        CCSimpleAudioEngine.SharedEngine.PlayEffect("sound_oso");

                });
                EventDispatcher.AddEventListener(tmp, 1);
                listener_buttons.Add(tmp);
            }
        }

        public override void Update(float dt)
        {

            base.Update(dt);

            if (JoyPanel != null)
            {
                bear.Position = JoyPanel.GetPlayerPosition(dt);

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
            base.OnExit();

            this.EventDispatcher.RemoveEventListener(startMovement);
            this.EventDispatcher.RemoveEventListener(endMovement);

            foreach (var item in listener_buttons)
            {
                this.EventDispatcher.RemoveEventListener(item);
            }
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

