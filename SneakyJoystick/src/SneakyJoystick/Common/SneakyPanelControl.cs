using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{



    //public enum ButtonType
    //{
    //    Button1 = 1, Button2 = 2, Button3 = 3, Button4 = 4
    //}

    public enum SneakyJoystickMovementStatus
    {
        Start = 1,
        OnMove = 2,
        End = 3,
    }

    public enum ButtonsOrientation
    {
        Horizontal = 1,
        Vertical = 2,
    }

    public enum SneakyButtonStatus
    {
        Press = 1,
        Release = 2,
    }

    public class SneakyPanelControl : CCLayer
    {

        #region Constants

        const int JOY_Z = 100;
        const int BUTTON_Z = 101;
        const int DEFAULT_TRANSPARENCY = 90;

        #endregion

        #region Custom Events

        public const string JOY_LISTENER_ID = "SneakyJoystick_JOY_LISTENER";
        public const string BUTTON_LISTENER_ID = "SneakyJoystick_BUTTON_LISTENER";

        #endregion

        #region Private properties

        private byte _opacity;

        #endregion

        public SneakyJoystickControlSkinnedBase JoyControl { get; set; }
        public List<SneakyButtonControlSkinnedBase> Buttons { get; set; }

        public CCNode Player { get; set; }
        CCSize wSize { get; set; }

        CCEventListenerTouchAllAtOnce tListener { get; set; }
        public bool IsListenerDisabled { get; set; }


        public bool IsDebug
        {
            get
            {
                if (JoyControl != null)
                    return JoyControl.IsDebug;
                return false;
            }

            set
            {

                if (JoyControl != null)
                    JoyControl.IsDebug = value;

                foreach (var item in Buttons)
                {
                    item.IsDebug = value;
                }

            }

        }

        public byte Opacity
        {
            get
            {
                return _opacity;
            }

            set
            {
                if (JoyControl != null)
                    JoyControl.Opacity = value;

                foreach (var item in Buttons)
                {
                    item.Opacity = value;
                }

                _opacity = value;
            }
        }

        //DIRECTION =====================================

        private ButtonsOrientation _orientation { get; set; }

        public ButtonsOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                ReorderButtons();
            }
        }


        public bool HasPlayer { get { return Player != null; } }

        public bool HasAnyDirection
        {
            get
            {
                return JoyControl.HasAnyDirection;
            }
        }

        public bool IsRight
        {
            get
            {
                return JoyControl.IsRight;
            }
        }
        public bool IsLeft
        {
            get
            {
                return JoyControl.IsLeft;
            }
        }
        public bool IsDown
        {
            get
            {
                return JoyControl.IsDown;
            }
        }
        public bool IsUp
        {
            get
            {
                return JoyControl.IsUp;
            }
        }
        public bool IsUpLeft
        {
            get
            {
                return JoyControl.IsUpLeft;
            }
        }
        public bool IsUpRight
        {
            get
            {
                return JoyControl.IsUpRight;
            }
        }
        public bool IsDownLeft
        {
            get
            {
                return JoyControl.IsDownLeft;
            }
        }
        public bool IsDownRight
        {
            get
            {
                return JoyControl.IsDownRight;
            }
        }


        public SneakyPanelControl(int buttons)
        {

            Buttons = new List<SneakyButtonControlSkinnedBase>(buttons);
        }


        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

			wSize = windowSize;

			//Joystick Init
            InitializeJoyStick();

            //Buttons init
            InitializeButtons(Buttons.Capacity);

            Opacity = DEFAULT_TRANSPARENCY;

            if (!IsListenerDisabled)
            {
                tListener = new CCEventListenerTouchAllAtOnce();
                tListener.OnTouchesBegan = OnTouchesBegan;
                tListener.OnTouchesMoved = OnTouchesMoved;
                tListener.OnTouchesCancelled = OnTouchesCancelled;
                tListener.OnTouchesEnded = OnTouchesEnded;
                AddEventListener(tListener, this);
            }
        }

        public void InitializeJoyStick()
        {

            JoyControl = new SneakyJoystickControlSkinnedBase();

            JoyControl.Position = new CCPoint(
               wSize.Width * 0.08f,
               wSize.Height * 0.08f
               );

            AddChild(JoyControl, JOY_Z);
        }

        public void InitializeButtons(int buttons)
        {
            SneakyButtonControlSkinnedBase tmp = null;
            for (int i = 0; i < buttons; i++)
            {
                tmp = new SneakyButtonControlSkinnedBase(i);
                AddChild(tmp, JOY_Z);
                Buttons.Add(tmp);
            }

            Orientation = ButtonsOrientation.Horizontal;
        }


        public void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            JoyControl.OnTouchesEnded(touches, touchEvent);

            foreach (var button in Buttons)
                button.OnTouchesEnded(touches, touchEvent);
        }

        public void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {
            JoyControl.OnTouchesCancelled(touches, touchEvent);

            foreach (var button in Buttons)
                button.OnTouchesCancelled(touches, touchEvent);
        }

        public void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            JoyControl.OnTouchesMoved(touches, touchEvent);

            foreach (var button in Buttons)
                button.OnTouchesMoved(touches, touchEvent);
        }

        public void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (JoyControl != null)
                JoyControl.OnTouchesBegan(touches, touchEvent);

            foreach (var button in Buttons)
                button.OnTouchesBegan(touches, touchEvent);
        }

        private void ReorderButtons()
        {
            float x, y;
            if (Orientation == ButtonsOrientation.Vertical)
            {
                y = 0.03f;
                for (int i = 0; i < Buttons.Count; i++)
                {

                    x = (i % 2 == 0) ? 0.8f : 0.9f;

                    if (i % 2 == 0)
                        y += 0.12f;

                    Buttons[i].Position = new CCPoint(wSize.Width * x, wSize.Height * y);
					Buttons[i].AnchorPoint = CCPoint.AnchorLowerLeft;
                }

            }
            else
            {
                y = .15f;
                x = wSize.Width;
                for (int i = 0; i < Buttons.Count; i++)
                {

                    Buttons[i].Position = new CCPoint(x, wSize.Height * y);
					Buttons[i].AnchorPoint = CCPoint.AnchorLowerRight;
					x -= (Buttons[i].DefaultSprite.BoundingBox.Size.Width + 20f);



                }

            }
        }

        public static CCPoint GetPlayerPosition(CCNode player, float dt, CCSize wSize)
        {
            if (player != null)
                return SneakyJoystickControl.GetPositionFromVelocity(SneakyJoystickControl.Velocity, player, dt, wSize);

            return CCPoint.Zero;
        }


        public CCPoint GetPlayerPosition(float dt, CCSize wSize)
        {
            if (Player != null)
                return JoyControl.GetNextPositionFromImage(Player, dt, wSize);

            Console.WriteLine("SNEAKYCONTROL > GETPLAYERPOSITION() : ERROR. NOT PLAYER ASSIGNED");
            return CCPoint.Zero;
        }

        protected override void Draw()
        {

            if (IsDebug)
            {
                CCDrawingPrimitives.Begin();
                CCDrawingPrimitives.DrawRect(new CCRect(0, 0, this.ContentSize.Width, this.ContentSize.Height), CCColor4B.Blue);
                CCDrawingPrimitives.End();
            }
        }


        public override void OnExit()
        {
            base.OnExit();

            if (!IsListenerDisabled)
                RemoveEventListener(tListener);

        }

    }



    #region RESPONSE CLASSES
    public class SneakyJoystickEventResponse
    {
        public SneakyJoystickMovementStatus ResponseType;
        public object UserData;

        public SneakyJoystickEventResponse(SneakyJoystickMovementStatus responseType, object userData)
        {
            ResponseType = responseType; UserData = userData;
        }
    }

    public class SneakyButtonEventResponse
    {
        public SneakyButtonStatus ResponseType;

        public int ID;

        public object UserData;

        public SneakyButtonEventResponse(SneakyButtonStatus responseType, int id, object userData)
        {
            ResponseType = responseType; UserData = userData; ID = id;
        }
    }

    #endregion



}

//public void Update(float dt)
//{
//    if (Player!=null)
//        Player.Position = DSJoyStickHelper.GetVelocity(leftJoystick.velocity, Player.Position, Player.ContentSize, CCDirector.SharedDirector.WinSize, dt);
//}

//public void InitializeButton(int id)
//{

//    SneakyButtonControlSkinnedBase tmp = null;
//    tmp = new SneakyButtonControlSkinnedBase(id);
//    tmp.Position = new CCPoint(wSize.Width * 0.9f, wSize.Height * 0.18f);
//    Button1 = tmp;

//    switch (button)
//    {
//        case ButtonType.Button1:



//            break;
//        case ButtonType.Button2:
//            tmp = new SneakyButtonControlSkinnedBase(2);
//            tmp.Position = new CCPoint(wSize.Width * 0.8f, wSize.Height * 0.18f);
//            Button2 = tmp;
//            break;
//        case ButtonType.Button3:
//            tmp = new SneakyButtonControlSkinnedBase(3);
//            tmp.Position = new CCPoint(wSize.Width * 0.9f, wSize.Height * 0.30f);
//            Button3 = tmp;
//            break;
//        case ButtonType.Button4:
//            tmp = new SneakyButtonControlSkinnedBase(4);
//            tmp.Position = new CCPoint(wSize.Width * 0.8f, wSize.Height * 0.30f);
//            Button4 = tmp;
//            break;
//        default:
//            break;
//    }

//    if (tmp != null)
//    {
//        AddChild(tmp, JOY_Z);
//        Buttons.Add(tmp);
//        tmp = null;
//    }


//}
