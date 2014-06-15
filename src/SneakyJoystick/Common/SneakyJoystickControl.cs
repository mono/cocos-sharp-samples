using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.IO.SneakyJoystick
{


    public class SneakyJoystickControl : CCLayer
    {

        #region Delegates

        public event SneakyStartEndActionDelegate StartMovement;
        public event SneakyStartEndActionDelegate EndMovement;

        #endregion

        #region Private properties

		private bool isDPad;
		private float joystickRadius;
		private float thumbRadius;
		private float deadRadius; //Size of deadzone in joystick (how far you must move before input starts). Automatically set if isDpad == YES

		private bool isMoving;

		private CCRect ControlSize { get; set; }

        #endregion

        #region Public properties

        public CCPoint StickPosition { get; set; }

        public CCPoint Center { get; set; }

        public CCPoint StickPreviousPosition { get; set; }
        public CCPoint Velocity { get; set; }

        public float Degrees { get; set; }
        public bool AutoCenter { get; set; }

        public float JoystickRadiusSq { get; set; } //Optimizations (keep Squared values of all radii for faster calculations) (updated internally when changing joy/thumb radii)

        public float JoystickRadius
        {
            get
            {
                return joystickRadius;
            }
            set
            {
                JoystickRadiusSq = value * value;
                joystickRadius = value;

            }
        }

        public float ThumbRadius
        {
            get
            {
                return thumbRadius;
            }
            set
            {
                ThumbRadiusSq = value * value;
                thumbRadius = value;
            }
        }
        public float ThumbRadiusSq { get; set; }

        public float DeadRadius
        {
            get
            {
                return deadRadius;
            }
            set
            {
                DeadRadiusSq = value * value;
                deadRadius = value;
            }
        }
        public float DeadRadiusSq { get; set; }


        public bool IsDebug;

        //DPAD =====================================================

        public bool IsDPad
        {
            get
            {
                return isDPad;
            }
            set
            {

                isDPad = value;
                if (isDPad)
                {
                    HasDeadzone = true;
                    deadRadius = 10.0f;
                }
            }
        }

        public bool HasDeadzone { get; set; } //Turns Deadzone on/off for joystick, always YES if ifDpad == YES
        public int NumberOfDirections { get; set; } //Used only when isDpad == YES

        //DIRECTIONS ===============================================

        public bool HasAnyDirection
        {
            get
            {
                return (IsDown || IsLeft || IsUp || IsRight);

            }
        }

        public bool IsUp { get; set; }
        public bool IsRight { get; set; }
        public bool IsLeft { get; set; }
        public bool IsDown { get; set; }
        public bool IsUpLeft { get; set; }
        public bool IsUpRight { get; set; }
        public bool IsDownLeft { get; set; }
        public bool IsDownRight { get; set; }

        #endregion

        public SneakyJoystickControl(CCRect rect)
        {

            Degrees = 0.0f;
            Velocity = CCPoint.Zero;
            AutoCenter = true;

            HasDeadzone = false;
            NumberOfDirections = 4;

            isDPad = false;
            joystickRadius = rect.Size.Width / 2;

            ThumbRadius = 32.0f;
            DeadRadius = 0.0f;

			AnchorPoint = CCPoint.AnchorMiddle;

			ControlSize = rect;

        }

		protected override void RunningOnNewWindow(CCSize windowSize)
		{
			base.RunningOnNewWindow(windowSize);

			var rect = ControlSize.PixelsToPoints(Director.ContentScaleFactor);

			ContentSize = rect.Size;
			Position = rect.Center;

			StickPosition = new CCPoint(ContentSize.Width / 2, ContentSize.Height / 2);
			Center = ContentSize.Center;
		}


        public virtual void UpdateVelocity(CCPoint point)
        {
            // Calculate distance and angle from the center.
            float dx = point.X - ContentSize.Width / 2;
            float dy = point.Y - ContentSize.Height / 2;
            float dSq = dx * dx + dy * dy;

            if (dSq <= DeadRadiusSq)
            {
                Velocity = CCPoint.Zero;
                Degrees = 0.0f;
                StickPosition = point;
                return;
            }

            float angle = CCMathExHelper.atan2(dy, dx); // in radians
            if (angle < 0)
            {
                angle += CCMathExHelper.PI_X_2;
            }
            float cosAngle;
            float sinAngle;

            if (isDPad)
            {
                float anglePerSector = 360.0f / NumberOfDirections * CCMathExHelper.SJ_DEG2RAD;
                angle = CCMathExHelper.round(angle / anglePerSector) * anglePerSector;
            }

            cosAngle = CCMathHelper.Cos(angle);
            sinAngle = CCMathHelper.Sin(angle);

            // NOTE: Velocity goes from -1.0 to 1.0.
            if (dSq > JoystickRadiusSq || isDPad)
            {
                dx = cosAngle * joystickRadius;
                dy = sinAngle * joystickRadius;
            }

            Velocity = new CCPoint(dx / joystickRadius, dy / joystickRadius);
            Degrees = angle * CCMathExHelper.SJ_RAD2DEG;

            // Update the thumb's position
            StickPosition = new CCPoint(dx + ContentSize.Width / 2, dy + ContentSize.Height / 2);

        }

        public virtual void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {

            //base.TouchesBegan(touches, touchEvent);

            CCTouch touch = touches.First();

            CCPoint location = Director.ConvertToGl(touch.LocationInView);

            //if([background containsPoint:[background convertToNodeSpace:location]]){
            location = ConvertToNodeSpace(location);
            //Do a fast rect check before doing a circle hit check:
            if (location.X - joystickRadius < -joystickRadius || location.X - joystickRadius > joystickRadius || location.Y - joystickRadius < -joystickRadius || location.Y - joystickRadius > joystickRadius)
            {
                return;
            }
            else
            {
                float dSq = (location.X - joystickRadius) * (location.X - joystickRadius) + (location.Y - joystickRadius) * (location.Y - joystickRadius);
                if (JoystickRadiusSq > dSq)
                {
                    isMoving = true;

                    //[self updateVelocity:location];
                    UpdateVelocity(location);

                    if (StartMovement != null)
                        StartMovement();

                    return;
                }
            }

        }

        public virtual void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            //base.TouchesMoved(touches, touchEvent);

            if (!isMoving)
                return;

            CCTouch touch = touches.First();

            CCPoint location = Director.ConvertToGl(touch.LocationInView);
            location = ConvertToNodeSpace(location);

            //Check direction
            IsRight = (location.X > Center.X);
            IsLeft = !IsRight;
            IsUp = (location.Y > Center.Y);
            IsDown = !IsUp;

            IsDownLeft = IsDown && IsLeft;
            IsDownRight = IsDown && IsRight;
            IsUpLeft = IsUp && IsLeft;
            IsUpRight = IsUp && IsRight;

            StickPreviousPosition = location;

            UpdateVelocity(location);

        }

        public virtual void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {
            //base.TouchesCancelled(touches, touchEvent);

            OnTouchesEnded(touches, touchEvent);
        }

        public virtual void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            //base.TouchesEnded(touches, touchEvent);

            if (!isMoving)
                return;

            ResetDirections();

            isMoving = false;

            CCTouch touch = touches.First();

            CCPoint location = new CCPoint(ContentSize.Width / 2, ContentSize.Height / 2);
            if (!AutoCenter)
            {
                location = Director.ConvertToGl(touch.LocationInView);
                location = ConvertToNodeSpace(location);
            }

            UpdateVelocity(location);

            if (EndMovement != null)
                EndMovement();

        }

        public void ResetDirections()
        {
            IsLeft = IsRight = IsUp = IsDown = IsDownLeft = IsDownRight = IsUpLeft = IsUpRight = false;
        }

        public void ForceCenter()
        {
            StickPosition = new CCPoint(Center.X, Center.Y);
        }

        public CCPoint GetNextPositionFromImage(CCNode node, float dt)
        {
            return GetPositionFromVelocity(Velocity, node, dt, Director.WindowSizeInPixels);
        }

        public void RefreshImagePosition(CCNode node, float dt)
        {
            if (node != null)
                node.Position = GetPositionFromVelocity(Velocity, node, dt, Director.WindowSizeInPixels);
        }


        #region Static methods

        public static CCPoint GetPositionFromVelocity(CCPoint velocity, CCNode node, float dt, CCSize winSize)
        {
            return GetPositionFromVelocity(velocity, node.Position, node.ContentSize, winSize, dt);
        }

        public static CCPoint GetPositionFromVelocity(CCPoint velocity, CCPoint actualPosition, CCSize size, CCSize winSize, float dt)
        {
            return GetPositionFromVelocity(velocity, actualPosition, size.Width, size.Height, winSize.Width, winSize.Height, dt);
        }

        public static CCPoint GetPositionFromVelocity(CCPoint velocity, CCPoint actualPosition, float Width, float Height, float maxWindowWidth, float maxWindowHeight, float dt)
        {

            CCPoint scaledVelocity = CCPointExHelper.b2Mul(velocity, 240);
            CCPoint newPosition = new CCPoint(actualPosition.X + scaledVelocity.X * dt, actualPosition.Y + scaledVelocity.Y * dt);

            if (newPosition.Y > maxWindowHeight - Height / 2)
            {
                newPosition.Y = maxWindowHeight - Height / 2;
            }
            if (newPosition.Y < (0 + Height / 2))
            {
                newPosition.Y = (0 + Height / 2);
            }

            if (newPosition.X > maxWindowWidth - Width / 2)
            {
                newPosition.X = maxWindowWidth - Width / 2;
            }
            if (newPosition.X < (0 + Width / 2))
            {
                newPosition.X = (0 + Width / 2);
            }

            return newPosition;

        }


        protected override void Draw()
        {
            base.Draw();

            if (!IsDebug)
                return;

            CCRect rect = new CCRect(0, 0, ContentSize.Width, ContentSize.Height);

            CCPoint[] vertices = new CCPoint[] {
        new CCPoint(rect.Origin.X,rect.Origin.Y),
        new CCPoint(rect.Origin.X+rect.Size.Width,rect.Origin.Y),
        new CCPoint(rect.Origin.X+rect.Size.Width,rect.Origin.Y+rect.Size.Height),
        new CCPoint(rect.Origin.X,rect.Origin.Y+rect.Size.Height),
    };

            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawCircle(AnchorPointInPoints, 10, 0, 8, true, CCColor4B.Blue);
            CCDrawingPrimitives.DrawSolidPoly(vertices, 4, CCColor4B.Red, true);
            CCDrawingPrimitives.End();

        }

        #endregion

    }
}
