using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{

    public enum ButtonType
    {
        Button1 = 1, Button2 = 2, Button3 = 3, Button4 = 4
    }

    public class SneakyPanelControl : CCLayer
    {


        #region Constants

        const int JOY_Z = 100;
        const int BUTTON_Z = 101;
        const int DEFAULT_TRANSPARENCY = 90;

        #endregion

		#region Custom Events

		public const string START_MOVEMENT = "SneakyJoystick_StartMovement";
		public const string END_MOVEMENT = "SneakyJoystick_EndMovement";

		public const string BUTTON_1_START_PRESS = "SneakyJoystick_Button1StartPress";
		public const string BUTTON_2_START_PRESS = "SneakyJoystick_Button3StartPress";
		public const string BUTTON_3_START_PRESS = "SneakyJoystick_Button4StartPress";
		public const string BUTTON_4_START_PRESS = "SneakyJoystick_Button5StartPress";

		public const string BUTTON_1_END_PRESS = "SneakyJoystick_Button1EndPress";
		public const string BUTTON_2_END_PRESS = "SneakyJoystick_Button2EndPress";
		public const string BUTTON_3_END_PRESS = "SneakyJoystick_Button3EndPress";
		public const string BUTTON_4_END_PRESS = "SneakyJoystick_Button4EndPress";


		#endregion

        #region Custom Events

		CCEventCustom button1StartPress = new CCEventCustom(BUTTON_1_START_PRESS);
		CCEventCustom button1EndPress = new CCEventCustom(BUTTON_1_END_PRESS);

		CCEventCustom button2StartPress = new CCEventCustom(BUTTON_2_START_PRESS);
		CCEventCustom button2EndPress = new CCEventCustom(BUTTON_2_END_PRESS);

		CCEventCustom button3StartPress = new CCEventCustom(BUTTON_3_START_PRESS);
		CCEventCustom button3EndPress = new CCEventCustom(BUTTON_3_END_PRESS);

		CCEventCustom button4StartPress = new CCEventCustom(BUTTON_4_START_PRESS);
		CCEventCustom button4EndPress = new CCEventCustom(BUTTON_4_END_PRESS);

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

        //CCNode MasterLayer;
        CCNode Player;
        CCSize wSize;

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

                if (Button1 != null)
                    Button1.IsDebug = value;
                if (Button2 != null)
                    Button2.IsDebug = value;
                if (Button3 != null)
                    Button3.IsDebug = value;
                if (Button4 != null)
                    Button4.IsDebug = value;

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

        public SneakyPanelControl()
        {

			wSize = Director.WindowSizeInPoints;

            Buttons = new List<SneakyButtonControlSkinnedBase>(6);

        }

        private void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {

            JoyControl.OnTouchesEnded(touches, touchEvent);

            if (Button1 != null)
                Button1.OnTouchesEnded(touches, touchEvent);
            if (Button2 != null)
                Button2.OnTouchesEnded(touches, touchEvent);
            if (Button3 != null)
                Button3.OnTouchesEnded(touches, touchEvent);
            if (Button4 != null)
                Button4.OnTouchesEnded(touches, touchEvent);
        }

        private void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {
            JoyControl.OnTouchesCancelled(touches, touchEvent);

            if (Button1 != null)
                Button1.OnTouchesCancelled(touches, touchEvent);
            if (Button2 != null)
                Button2.OnTouchesCancelled(touches, touchEvent);
            if (Button3 != null)
                Button3.OnTouchesCancelled(touches, touchEvent);
            if (Button4 != null)
                Button4.OnTouchesCancelled(touches, touchEvent);
        }

        private void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            JoyControl.OnTouchesMoved(touches, touchEvent);
            if (Button1 != null)
                Button1.OnTouchesMoved(touches, touchEvent);
            if (Button2 != null)
                Button2.OnTouchesMoved(touches, touchEvent);
            if (Button3 != null)
                Button3.OnTouchesMoved(touches, touchEvent);
            if (Button4 != null)
                Button4.OnTouchesMoved(touches, touchEvent);
        }

        private void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {

            if (JoyControl != null)
                JoyControl.OnTouchesBegan(touches, touchEvent);
            if (Button1 != null)
                Button1.OnTouchesBegan(touches, touchEvent);
            if (Button2 != null)
                Button2.OnTouchesBegan(touches, touchEvent);
            if (Button3 != null)
                Button3.OnTouchesBegan(touches, touchEvent);
            if (Button4 != null)
                Button4.OnTouchesBegan(touches, touchEvent);
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

		protected override void RunningOnNewWindow(CCSize windowSize)
		{
			base.RunningOnNewWindow(windowSize);

			//Inicializamos el joystick
			InitializeJoyStick();

			//Inicializamos los botones
			InitializeButtons();

			Opacity = DEFAULT_TRANSPARENCY;

			var listener1 = new CCEventListenerTouchAllAtOnce();
			//listener1.IsSwallowTouches = true;
			listener1.OnTouchesBegan = OnTouchesBegan;
			listener1.OnTouchesMoved = OnTouchesMoved;
			listener1.OnTouchesCancelled = OnTouchesCancelled;
			listener1.OnTouchesEnded = OnTouchesEnded;

			EventDispatcher.AddEventListener(listener1, this);

		}


        public void InitializeButtons()
        {
            InitializeButton(ButtonType.Button1);
            InitializeButton(ButtonType.Button2);
            InitializeButton(ButtonType.Button3);
            InitializeButton(ButtonType.Button4);
        }

        public void InitializeButton(ButtonType button)
        {

            SneakyButtonControlSkinnedBase tmp = new SneakyButtonControlSkinnedBase();

            Buttons.Add(tmp);

            AddChild(tmp, JOY_Z);

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


        public static CCPoint GetPlayerPosition(CCNode player, float dt, CCSize wSize)
        {
            if (player != null)
                return SneakyJoystickControl.GetPositionFromVelocity(SneakyJoystickControl.Velocity, player, dt, wSize);

            return CCPoint.Zero;
        }


        public CCPoint GetPlayerPosition(float dt)
        {
            if (Player != null)
                return JoyControl.GetNextPositionFromImage(Player, dt);

            Console.WriteLine("SNEAKYCONTROL > GETPLAYERPOSITION() : ERROR. NOT PLAYER ASSIGNED");
            return CCPoint.Zero;
        }



        public void SetPlayer(CCNode user)
        {
            Player = user;
        }

        #region ButtonEvents

        void Button1_EndPress()
        {
			EventDispatcher.DispatchEvent(button1EndPress);
        }

        void Button2_EndPress()
        {
			EventDispatcher.DispatchEvent(button2EndPress);
        }

        void Button3_EndPress()
        {
			EventDispatcher.DispatchEvent(button3EndPress);
        }

        void Button4_EndPress()
        {
			EventDispatcher.DispatchEvent(button4EndPress);
        }


        void Button1_StartPress()
        {
			EventDispatcher.DispatchEvent(button1StartPress);
        }

        void Button2_StartPress()
        {
			EventDispatcher.DispatchEvent(button2StartPress);
        }

        void Button3_StartPress()
        {
			EventDispatcher.DispatchEvent(button3StartPress);
        }

        void Button4_StartPress()
        {
			EventDispatcher.DispatchEvent(button4StartPress);
        }


        #endregion

        public override void Update(float dt)
        {
            base.Update(dt);

            if (JoyControl != null)
                JoyControl.RefreshImagePosition(Player, dt);
        }

        public void Draw()
        {

            if (IsDebug)
            {
                CCDrawingPrimitives.Begin();
                CCDrawingPrimitives.DrawRect(new CCRect(0, 0, this.ContentSize.Width, this.ContentSize.Height), CCColor4B.Blue);
                CCDrawingPrimitives.End();
            }
        }

        
    }
}

//public void Update(float dt)
//{
//    if (Player!=null)
//        Player.Position = DSJoyStickHelper.GetVelocity(leftJoystick.velocity, Player.Position, Player.ContentSize, CCDirector.SharedDirector.WinSize, dt);
//}
