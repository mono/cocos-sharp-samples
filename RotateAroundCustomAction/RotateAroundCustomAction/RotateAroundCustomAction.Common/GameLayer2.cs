using System;
using System.Collections.Generic;
using CocosSharp;

namespace RotateAroundCustomAction
{
    public class GameLayer2 : CCLayer
    {
        CCSprite monkey;

        CCRotateAroundTo rotateAround;
        CCPoint origin;

        public GameLayer2()
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

            var originNode = new CCDrawNode();
            originNode.DrawDot(origin, 10, new CCColor4F(CCColor4B.Magenta));
            AddChild(originNode);

            PositionMonkey();

            rotateAround = new CCRotateAroundTo(2.0f, origin, 45, -1);

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        void PositionMonkey()
        {
            // We start our rotation with an offset of 230,0 from the origin 0 degrees
            // and randomly position the monkey
            var startingAngle = CCRandom.GetRandomInt(-360, 360);
            monkey.Position = CCPoint.RotateByAngle(origin + new CCPoint(230, 0), origin, CCMathHelper.ToRadians(startingAngle));
            positioned = true;

        }

        bool positioned = true;

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if (monkey.NumberOfRunningActions == 0 && !positioned)
                {
                    PositionMonkey();
                }
                else
                {
                    // Perform touch handling here
                    monkey.RunAction(rotateAround);
                    positioned = false;
                }
            }
        }
    }
}
