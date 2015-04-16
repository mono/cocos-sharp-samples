using System;
using System.Collections.Generic;
using CocosSharp;

namespace RotateAroundCustomAction
{
    public class GameLayer : CCLayer
    {
        CCSprite monkey;

        public GameLayer()
        {
            // Load and instantate your assets here

            // Make any renderable node objects (e.g. sprites) children of this layer
            monkey = new CCSprite("monkey");
            AddChild(monkey);

        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            // point to rotate around
            origin = bounds.Center;

            // Draw a magenta circle at our origin
            var originNode = new CCDrawNode();
            originNode.DrawDot(origin, 10, new CCColor4F(CCColor4B.Magenta));
            AddChild(originNode);

            PositionMonkey();

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        CCPoint origin; // point to rotate around
        CCPoint startPosition; // Starting position of monkey

        float elapsed = 0;  // our elapsed time accumulator
        float duration = 2.0f;  // time that the animation should take to finish
        const float angle = 360;  // degrees to pass through in duration time.

        const float rotationDirection = -1;  // -1 = ClockWise / 1 = CounterClockWise

        // Convert to radians and take into account our rotationDirection
        float theta = CCMathHelper.ToRadians(angle) * rotationDirection;   

        void Rotate (float delta)
        {
            // calculate our elapsed time
            elapsed += delta;
            var time = elapsed / duration;

            // Check if we are done and if so unschedule ourselves from more updates.
            if (elapsed > duration)
                this.Unschedule(Rotate);

            var thetaOverTime = theta * time;

            monkey.Position = CCPoint.RotateByAngle(startPosition, origin, thetaOverTime);
        }

        void PositionMonkey()
        {

            // We start our rotation with an offset of 230,0 from the origin 0 degrees
            // and randomly position the monkey
            var startingAngle = CCRandom.GetRandomInt(-360, 360);
            monkey.Position = CCPoint.RotateByAngle(origin + new CCPoint(230, 0), origin, CCMathHelper.ToRadians(startingAngle));
            startPosition = monkey.Position;
            elapsed = 0;
            positioned = true;

        }

        bool positioned = false;

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
                if (this.NumberOfRunningActions == 0 && !positioned)
                {
                    PositionMonkey();
                }
                else
                {
                    // schedule the Rotate method to be called on each update cycle.
                    Schedule(Rotate);
                    positioned = false;
                }
            }
        }
    }
}
