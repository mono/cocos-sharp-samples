using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{
    public delegate void SneakyStartEndActionDelegate();

    public class SneakyButtonControl : CCNode
    {
        #region Custom Events
        CCEventCustom buttonEvent;
        #endregion

        public int ID { get; set; }
        public bool IsDebug { get; set; }
        public float radiusSq;

        //public CCRect bounds;
        public bool active;
        public bool status;
        public bool value;
        public bool isHoldable;
        public bool isToggleable;
        public float rateLimit;

        //Optimizations (keep Squared values of all radii for faster calculations) (updated internally when changing radii)
        float radius;

        public SneakyButtonControl(CCRect rect, int id)
        {
            status = true; //defaults to enabled
            value = false;
            active = false;
            isHoldable = true;
            isToggleable = false;
            radius = 32.0f;
            rateLimit = 1.0f / 120.0f;
            Position = rect.Origin;
            ID = id;

            buttonEvent = new CCEventCustom(SneakyPanelControl.BUTTON_LISTENER_ID);

        }

        void limiter(float delta)
        {
            value = false;
            active = false;
            // Unschedule(limiter);
        }

        public void SetRadius(float r)
        {
            radius = r;
            radiusSq = r * r;
        }

        public virtual bool OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            CCTouch touch = touches.FirstOrDefault();

            CCPoint location = Layer.ScreenToWorldspace(touch.LocationOnScreen);
            location = WorldToParentspace(location);

            // Adjust the location to be relative to the button's origin
            location -= BoundingBox.Origin;
			//Console.WriteLine(location + " radius: " + radius);
            //Do a fast rect check before doing a circle hit check:
            if (location.X < -radius || location.X > radius || location.Y < -radius || location.Y > radius)
            {
                return false;
            }
            else
            {

                float dSq = location.X * location.X + location.Y * location.Y;
				Console.WriteLine(location + " radius: " + radius + " distanceSquared: " + dSq);

                if (radiusSq > dSq)
                {
                    active = true;
                    if (!isHoldable && !isToggleable)
                    {
                        value = true;
                        //Schedule(limiter, rateLimit);
                    }
                    if (isHoldable) value = true;
                    if (isToggleable) value = !value;

                    CheckSelf();


                    // Fire off our event to notify that movement was started
                    buttonEvent.UserData = new SneakyButtonEventResponse(SneakyButtonStatus.Press, ID, this);
                    DispatchEvent(buttonEvent);

                    return true;
                }
            }
            return false;
        }

        public virtual void CheckSelf()
        {
        }

        public virtual void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            //base.TouchMoved(touch, touchEvent);
            CCTouch touch = touches.FirstOrDefault();

            if (!active) return;

            CCPoint location = Layer.ScreenToWorldspace(touch.LocationOnScreen);
            location = WorldToParentspace(location);
			Console.WriteLine("Moved: " + location + " radius: " + radius);
			//Console.WriteLine(location);
            //Do a fast rect check before doing a circle hit check:
            if (location.X < -radius || location.X > radius || location.Y < -radius || location.Y > radius)
            {
                return;
            }
            else
            {
                float dSq = location.X * location.X + location.Y * location.Y;
                if (radiusSq > dSq)
                {
                    if (isHoldable) value = true;
                }
                else
                {
                    if (isHoldable) value = false; active = false;
                }
            }

            CheckSelf();
        }

        public virtual void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            CCTouch touch = touches.FirstOrDefault();

            if (!active) return;
            if (isHoldable) value = false;
            if (isHoldable || isToggleable) active = false;

            CheckSelf();

            // Fire off our event to notify that movement was started
            buttonEvent.UserData = new SneakyButtonEventResponse(SneakyButtonStatus.Release, ID, this);
            DispatchEvent(buttonEvent);
        }

        public virtual void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {

            OnTouchesEnded(touches, touchEvent);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        protected override void Draw()
        {
            base.Draw();
            CCDrawingPrimitives.Begin();
			CCDrawingPrimitives.DrawRect(new CCRect(-this.ContentSize.Width*.5f, -this.ContentSize.Height*.5f, this.ContentSize.Width*.5f, this.ContentSize.Height*.5f), CCColor4B.Blue);
            CCDrawingPrimitives.End();
        }
    }
}
