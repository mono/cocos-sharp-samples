using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.IO.SneakyJoystick
{
    public delegate void SneakyStartEndActionDelegate();

    public class SneakyButtonControl : CCNode
    {
        public bool IsDebug { get; set; }

        public event SneakyStartEndActionDelegate StartPress;
        public event SneakyStartEndActionDelegate EndPress;

        //CCPoint center;

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

        public SneakyButtonControl(CCRect rect)
        {
            //TouchEnabled = true;
            //bounds = new CCRect(0, 0, rect.Size.Width, rect.Size.Height);
            //center = new CCPoint(rect.Size.Width / 2, rect.Size.Height / 2);
            status = true; //defaults to enabled
            value = false;
            active = false;
            isHoldable = true;
            isToggleable = false;
            radius = 32.0f;
            rateLimit = 1.0f / 120.0f;
            Position = rect.Origin;
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

            CCPoint location = Director.ConvertToGl(touch.LocationInView);
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

        public virtual void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            //base.TouchMoved(touch, touchEvent);
            CCTouch touch = touches.FirstOrDefault();

            if (!active) return;

            CCPoint location = Director.ConvertToGl(touch.LocationInView);
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

        public virtual void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            CCTouch touch = touches.FirstOrDefault();

            if (!active) return;
            if (isHoldable) value = false;
            if (isHoldable || isToggleable) active = false;

            CheckSelf();

            if (EndPress != null)
                EndPress();

        }

        public virtual void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {

            OnTouchesEnded(touches, touchEvent);
        }


        public override void OnExit()
        {
            base.OnExit();



        }

        public void Draw()
        {
            base.Draw();
            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawRect(new CCRect(0, 0, this.ContentSize.Width, this.ContentSize.Height), CCColor4B.Blue);
            CCDrawingPrimitives.End();
        }


    }
}
