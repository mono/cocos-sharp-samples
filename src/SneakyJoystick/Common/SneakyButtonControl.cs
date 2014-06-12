using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.IO.SneakyJoystick
{
    public delegate void SneakyStartEndActionDelegate();

    public class SneakyButtonControl : CCNode
    {

        public event SneakyStartEndActionDelegate StartPress;
        public event SneakyStartEndActionDelegate EndPress;

        CCPoint center;

        public float radiusSq;

        public CCRect bounds;
        public bool active;
        public bool status;
        public bool value;
        public bool isHoldable;
        public bool isToggleable;
        public float rateLimit;

        //Optimizations (keep Squared values of all radii for faster calculations) (updated internally when changing radii)
        float radius;

        public SneakyButtonControl(CCRect rect)
        {
            //TouchEnabled = true;
            bounds = new CCRect(0, 0, rect.Size.Width, rect.Size.Height);
            center = new CCPoint(rect.Size.Width / 2, rect.Size.Height / 2);
            status = true; //defaults to enabled
            value = false;
            active = false;
            isHoldable = true;
            isToggleable = false;
            radius = 32.0f;
            rateLimit = 1.0f / 120.0f;
            Position = rect.Origin;

            var listener1 = new CCEventListenerTouchOneByOne();
            //listener1.IsSwallowTouches = true;
            listener1.OnTouchBegan = TouchBegan;
            listener1.OnTouchMoved = TouchMoved;
            listener1.OnTouchCancelled = TouchCancelled;
            listener1.OnTouchEnded = TouchEnded;

            EventDispatcher.AddEventListener(listener1, this);

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


        bool TouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            CCPoint location = CCDirector.SharedDirector.ConvertToGl(touch.LocationInView);
            location = ConvertToNodeSpace(location);
            //Do a fast rect check before doing a circle hit check:
            if (location.X < -radius || location.X > radius || location.Y < -radius || location.Y > radius)
            {
                return false;
            }
            else
            {

                float dSq = location.X * location.X + location.Y * location.Y;
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

                    if (StartPress != null)
                        StartPress();

                    return true;
                }
            }
            return false;
        }


        public virtual void CheckSelf()
        {

        }

        void TouchMoved(CCTouch touch, CCEvent touchEvent)
        {
            //base.TouchMoved(touch, touchEvent);

            if (!active) return;

            CCPoint location = CCDirector.SharedDirector.ConvertToGl(touch.LocationInView);
            location = ConvertToNodeSpace(location);

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



        void TouchEnded(CCTouch touch, CCEvent touchEvent)
        {
            //base.TouchEnded(touch, touchEvent);

            if (!active) return;
            if (isHoldable) value = false;
            if (isHoldable || isToggleable) active = false;

            CheckSelf();

            if (EndPress != null)
                EndPress();

        }

        void TouchCancelled(CCTouch touch, CCEvent touchEvent)
        {
            //base.TouchCancelled(touch, touchEvent);

            TouchEnded(touch, touchEvent);
        }


        public override void OnExit()
        {
            base.OnExit();



        }

    }
}
