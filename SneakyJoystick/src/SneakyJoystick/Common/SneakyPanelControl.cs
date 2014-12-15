using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{
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
        const int JOY_Z = 100;
        const int BUTTON_Z = 101;
        const int DEFAULT_TRANSPARENCY = 90;

        public const string JOY_LISTENER_ID = "SneakyJoystick_JOY_LISTENER";
        public const string BUTTON_LISTENER_ID = "SneakyJoystick_BUTTON_LISTENER";

        private byte _opacity;

        public SneakyJoystickControlSkinnedBase JoyControl { get; set; }
        public List<SneakyButtonControlSkinnedBase> Buttons { get; set; }

        public CCNode Player { get; set; }

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
            get { return _opacity; }
            set
            {
                if (JoyControl != null)
                    JoyControl.Opacity = value;

                foreach (var item in Buttons)
                    item.Opacity = value;

                _opacity = value;
            }
        }

		#region Directions

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

		#endregion

        public SneakyPanelControl(CCSize visibleBoundsSize, int buttons) : base(visibleBoundsSize)
        {
            Buttons = new List<SneakyButtonControlSkinnedBase>(buttons);
			ContentSize = visibleBoundsSize;
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();
            Opacity = DEFAULT_TRANSPARENCY;

			//Joystick initialization
			JoyControl = new SneakyJoystickControlSkinnedBase();
			AddChild(JoyControl, JOY_Z);
			JoyControl.Position = new CCPoint (ContentSize.Width * 0.09f, ContentSize.Width * 0.09f);

			//Buttons initialization
			SneakyButtonControlSkinnedBase tmp = null;
			for (int i = 0; i < Buttons.Capacity; i++)
			{
				tmp = new SneakyButtonControlSkinnedBase(i);
				AddChild(tmp, JOY_Z);
				Buttons.Add(tmp);
			}

			Orientation = ButtonsOrientation.Horizontal;
			//Listeners

            if (!IsListenerDisabled)
            {
                tListener = new CCEventListenerTouchAllAtOnce();
                tListener.OnTouchesBegan = OnTouchesBegan;
                tListener.OnTouchesMoved = OnTouchesMoved;
                tListener.OnTouchesCancelled = OnTouchesCancelled;
                tListener.OnTouchesEnded = OnTouchesEnded;
                AddEventListener(tListener, this);
            }

			#if DEBUG
			IsDebug = true;
			#endif
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
            CCSize visibleBoundsSize = VisibleBoundsWorldspace.Size;

            if (Orientation == ButtonsOrientation.Vertical)
            {
                y = 0.03f;
                for (int i = 0; i < Buttons.Count; i++)
                {
                    x = (i % 2 == 0) ? 0.8f : 0.9f;

                    if (i % 2 == 0)
                        y += 0.12f;

                    Buttons[i].Position = new CCPoint(visibleBoundsSize.Width * x, visibleBoundsSize.Height * y);
					Buttons[i].AnchorPoint = CCPoint.AnchorLowerLeft;
                }
            }
            else
            {
                y = .15f;
                x = visibleBoundsSize.Width;
                for (int i = 0; i < Buttons.Count; i++)
                {
                    Buttons[i].Position = new CCPoint(x, visibleBoundsSize.Height * y);
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
			if (JoyControl != null && Player != null)
                return JoyControl.GetNextPositionFromImage(Player, dt, wSize);
            Console.WriteLine("SNEAKYCONTROL > GETPLAYERPOSITION() : ERROR. NOT PLAYER ASSIGNED");
            return CCPoint.Zero;
        }

        protected override void Draw()
        {
            CCSize visibleBoundsSize = VisibleBoundsWorldspace.Size;
            if (IsDebug)
            {
                CCDrawingPrimitives.Begin();
                CCDrawingPrimitives.DrawRect(new CCRect(0, 0, visibleBoundsSize.Width, visibleBoundsSize.Height), CCColor4B.Blue);
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
}