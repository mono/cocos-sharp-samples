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

        //SneakyPanelControl JoyPanel;
        SneakyPanelControl JoyPanel;

        CCSize winSize;
        CCAnimation walkAnim;
        CCAction action;

        CCSprite bear;

        private bool IsWalking = false;

        public IntroLayer()
        {

            winSize = Director.WindowSizeInPixels;

            InitializeJoyPanel();

            InitializeBear();

            JoyPanel.SetPlayer(bear);

            Schedule(Update);
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
            bear.Position = new CCPoint(winSize.Width / 2, winSize.Height / 2);

            AddChild(bear);
        }

        public void InitializeJoyPanel()
        {

            JoyPanel = new SneakyPanelControl();

            JoyPanel.StartMovement += () =>
            {
                IsWalking = true;
            };

            JoyPanel.EndMovement += () =>
            {
                IsWalking = false;
            };

            JoyPanel.Button1StartPress += () =>
            {
                CCSimpleAudioEngine.SharedEngine.PlayEffect("sound_oso");
            };

            JoyPanel.IsDebug = true;

            AddChild(JoyPanel);

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

