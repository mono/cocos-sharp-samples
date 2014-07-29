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
        const int JOYSTICK_Z_ORDER = 9999;

        CCEventListenerCustom joystickListener;
        CCEventListenerCustom buttonListener;

        bool isWalking = false;

        CCAction bearAction;
        CCAction monkeyAction;
        CCAction currentAction;

        CCSprite bearSprite;
        CCSprite monkeySprite;
        CCSprite currentSprite;

        SneakyPanelControl joystickPanel;

        #region Properties

        public CCSprite CurrentSprite
        {
            get { return currentSprite; }
            set
            {
                if (currentSprite != value)
                {
                    CCSprite previousSprite = currentSprite;

                    currentSprite = value;

                    if (currentSprite == monkeySprite)
                    {
                        currentAction = monkeyAction;
                    }
                    else if (currentSprite == bearSprite)
                    {
                        currentAction = bearAction;
                    }

                    RemoveChild(previousSprite);
                    AddChild(currentSprite);
                }
            }
        }

        #endregion Properties


        #region Initialization

        public IntroLayer(CCSize visibleBoundsDimension) 
            : base(visibleBoundsDimension, CCColor4B.AliceBlue)
        {
        }

        public void InitializeJoystick()
        {
            joystickPanel = new SneakyPanelControl(LayerSizeInPixels, 2);
            joystickPanel.Position = CCPoint.Zero;
            joystickPanel.Orientation = ButtonsOrientation.Vertical;

            AddChild(joystickPanel,  JOYSTICK_Z_ORDER);
        }

        public void InitializeBear()
        {
            var spriteSheet = new CCSpriteSheet("animations/AnimBear.plist");
            var walkAnimation = new CCAnimation(spriteSheet.Frames, 0.1f);
            bearAction = new CCRepeatForever(new CCAnimate(walkAnimation));

            bearSprite = new CCSprite(spriteSheet.Frames.First()) { Name = "Bear" };
        }

        public void InitializeMonkey()
        {
            var spriteSheet = new CCSpriteSheet("animations/monkey.plist");
            var walkAnimation = new CCAnimation(spriteSheet.Frames, 0.1f);
            monkeyAction = new CCRepeatForever(new CCAnimate(walkAnimation));

            // Load the frames using the Frames property which
            var animationFrames = spriteSheet.Frames.FindAll((x) =>
                {
                    return x.TextureFilename.StartsWith("frame");
                });

            monkeySprite = new CCSprite(animationFrames.First()) { Name = "Monkey" };
            monkeySprite.Scale = 0.5f;
        }

        #endregion Initialization


        protected override void AddedToNewScene()
        {
            base.AddedToNewScene();

			InitializeJoystick();
			InitializeMonkey();
            InitializeBear();

            CurrentSprite = monkeySprite;
            joystickPanel.Player = CurrentSprite;

            CurrentSprite.Position = VisibleBoundsWorldspace.Center;

            CCSimpleAudioEngine.SharedEngine.PreloadEffect("sound_oso");

            joystickListener = new CCEventListenerCustom(SneakyPanelControl.JOY_LISTENER_ID, (customEvent) =>
                {
                    var response = customEvent.UserData as SneakyJoystickEventResponse;
                    if (response != null)
                    {

                        switch (response.ResponseType)
                        {
                            case SneakyJoystickMovementStatus.Start:
                                isWalking = true;
                                Console.WriteLine("Start walk.");
                                break;
                            case SneakyJoystickMovementStatus.End:
                                isWalking = false;
                                Console.WriteLine("Stop walk.");
                                break;
                            default:
                                break;
                        }

                    }

                });
            AddEventListener(joystickListener, 1);

            buttonListener = new CCEventListenerCustom(SneakyPanelControl.BUTTON_LISTENER_ID, (customEvent) =>
                {
                    var response = customEvent.UserData as SneakyButtonEventResponse;
                    if (response != null)
                    {
                        if (response.ID == 1)
                            CCSimpleAudioEngine.SharedEngine.PlayEffect("sound_oso");

                        if (response.ID == 0 && response.ResponseType == SneakyButtonStatus.Release)
                            SwitchSprite();

                        Console.WriteLine("BUTTON {0} {1}", response.ID, response.ResponseType == SneakyButtonStatus.Press ? "PRESSED" : "UNPRESSED");
                    }
                });

            AddEventListener(buttonListener, 1);

            Schedule();
        }

        void SwitchSprite()
        {
            if (currentSprite != null)
            {
                var position = currentSprite.Position;
                var flipX = currentSprite.FlipX;

                if(CurrentSprite == monkeySprite)
                    CurrentSprite = bearSprite;
                else
                    CurrentSprite = monkeySprite;

                currentSprite.Position = position;
                currentSprite.FlipX = flipX;
                joystickPanel.Player = currentSprite;
            }
        }

        public override void Update(float dt)
        {

            base.Update(dt);

            if (joystickPanel != null)
            {
                currentSprite.Position = joystickPanel.GetPlayerPosition(dt, VisibleBoundsWorldspace.Size);

                if (joystickPanel.HasAnyDirection)
                {
                    currentSprite.FlipX = (joystickPanel.JoyControl.IsRight);
                }

                if (isWalking && currentSprite.NumberOfRunningActions == 0)
                    currentSprite.RunAction(currentAction);

                if (!isWalking && currentSprite.NumberOfRunningActions > 0)
                    currentSprite.StopAllActions();
            }

        }

        public override void OnExit()
        {
            base.OnExit();

            RemoveEventListener(joystickListener);
            RemoveEventListener(buttonListener);
        }
    }
}

