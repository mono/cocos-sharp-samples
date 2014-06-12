using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.IO.SneakyJoystick
{

    public enum ButtonType
    {
        Button1 = 1, Button2 = 2, Button3 = 3, Button4 = 4
    }

    public class SneakyPanelControl
    {

        #region Constants

        const int JOY_Z = 100;
        const int BUTTON_Z = 101;
        const int DEFAULT_TRANSPARENCY = 90;

        #endregion

        #region Delegates

        public event SneakyStartEndActionDelegate Button1StartPress;
        public event SneakyStartEndActionDelegate Button1EndPress;

        public event SneakyStartEndActionDelegate Button2StartPress;
        public event SneakyStartEndActionDelegate Button2EndPress;

        public event SneakyStartEndActionDelegate Button3StartPress;
        public event SneakyStartEndActionDelegate Button3EndPress;

        public event SneakyStartEndActionDelegate Button4StartPress;
        public event SneakyStartEndActionDelegate Button4EndPress;

        public event SneakyStartEndActionDelegate StartMovement;
        public event SneakyStartEndActionDelegate EndMovement;

        #endregion

        #region Private properties

        private byte _opacity;

        #endregion

        public SneakyJoystickControlSkinnedBase JoyControl;

        public SneakyButtonControlSkinnedBase Button1; // new
        public SneakyButtonControlSkinnedBase Button2; // new
        public SneakyButtonControlSkinnedBase Button3; // new
        public SneakyButtonControlSkinnedBase Button4; // new

        public List<SneakyButtonControlSkinnedBase> Buttons;

        CCNode MasterLayer;
        CCNode Player;
        CCSize wSize;

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

                if (Button1 != null)
                    Button1.Opacity = value;

                if (Button2 != null)
                    Button2.Opacity = value;

                if (Button3 != null)
                    Button3.Opacity = value;

                if (Button4 != null)
                    Button4.Opacity = value;

                _opacity = value;
            }
        }

        //DIRECTION =====================================

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

        public SneakyPanelControl(CCNode masterLayer)
        {

            wSize = CCDirector.SharedDirector.WinSize;

            MasterLayer = masterLayer;

            Buttons = new List<SneakyButtonControlSkinnedBase>(6);

            //Inicializamos el joystick
            InitializeJoyStick();

            //Inicializamos los botones
            InitializeButtons();

            Opacity = DEFAULT_TRANSPARENCY;
        }

        public void InitializeJoyStick()
        {
            JoyControl = SneakyJoystickControlSkinnedBase.Create();

            //
            JoyControl.StartMovement += () =>
            {
                if (StartMovement != null)
                    StartMovement();
            };

            JoyControl.EndMovement += () =>
            {
                if (EndMovement != null)
                    EndMovement();
            };

            JoyControl.Position = new CCPoint(
               wSize.Width * 0.08f,
               wSize.Height * 0.08f
               );


            MasterLayer.AddChild(JoyControl, JOY_Z);
        }

        public void InitializeButtons()
        {
            InitializeButton(ButtonType.Button1);
            InitializeButton(ButtonType.Button2);
            //InitializeButton(ButtonType.Button3);
            //InitializeButton(ButtonType.Button4);
        }

        public void InitializeButton(ButtonType button)
        {

            SneakyButtonControlSkinnedBase tmp = SneakyButtonControlSkinnedBase.Create();

            Buttons.Add(tmp);

            MasterLayer.AddChild(tmp, JOY_Z);

            switch (button)
            {
                case ButtonType.Button1:
                    tmp.StartPress += Button1_StartPress;
                    tmp.EndPress += Button1_EndPress;

                    tmp.Position = new CCPoint(wSize.Width * 0.9f, wSize.Height * 0.18f);
                    Button1 = tmp;

                    break;
                case ButtonType.Button2:
                    tmp.StartPress += Button2_StartPress;
                    tmp.EndPress += Button2_EndPress;
                    tmp.Position = new CCPoint(wSize.Width * 0.8f, wSize.Height * 0.18f);
                    Button2 = tmp;
                    break;
                case ButtonType.Button3:
                    tmp.StartPress += Button3_StartPress;
                    tmp.EndPress += Button3_EndPress;
                    tmp.Position = new CCPoint(wSize.Width * 0.9f, wSize.Height * 0.30f);
                    Button3 = tmp;
                    break;
                case ButtonType.Button4:

                    tmp.StartPress += Button4_StartPress;
                    tmp.EndPress += Button4_EndPress;

                    tmp.Position = new CCPoint(wSize.Width * 0.8f, wSize.Height * 0.30f);
                    Button4 = tmp;
                    break;
                default:
                    break;
            }

            tmp = null;
        }

        public CCPoint GetPlayerPosition(float dt)
        {
            if (Player != null)
                return Player.Position = JoyControl.GetNextPositionFromImage(Player, dt);
            return CCPoint.Zero;
        }

        public void SetPlayer(CCNode user)
        {
            Player = user;
        }

        #region ButtonEvents

        void Button1_EndPress()
        {
            if (Button1EndPress != null)
                Button1EndPress();
        }

        void Button2_EndPress()
        {
            if (Button2EndPress != null)
                Button2EndPress();
        }

        void Button3_EndPress()
        {
            if (Button3EndPress != null)
                Button3EndPress();
        }

        void Button4_EndPress()
        {
            if (Button4EndPress != null)
                Button4EndPress();
        }

        void Button1_StartPress()
        {
            if (Button1StartPress != null)
                Button1StartPress();
        }

        void Button2_StartPress()
        {
            if (Button2StartPress != null)
                Button2StartPress();
        }

        void Button3_StartPress()
        {
            if (Button3StartPress != null)
                Button3StartPress();
        }

        void Button4_StartPress()
        {
            if (Button4StartPress != null)
                Button4StartPress();
        }


        #endregion


        public void Update(float dt)
        {
            if (JoyControl != null)
                JoyControl.RefreshImagePosition(Player, dt);

        }
    }
}

//public void Update(float dt)
//{
//    if (Player!=null)
//        Player.Position = DSJoyStickHelper.GetVelocity(leftJoystick.velocity, Player.Position, Player.ContentSize, CCDirector.SharedDirector.WinSize, dt);
//}
