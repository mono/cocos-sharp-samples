using System;
using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;

namespace CocosSharpBox2d.Shared
{
    internal class CCPhysicsSprite : CCSprite
    {
        readonly float ptmRatio;

        public CCPhysicsSprite (CCTexture2D f, CCRect r, float ptmRatio) : base (f, r)
        {
            this.ptmRatio = ptmRatio;
        }

        public CCPhysicsSprite(string fileName, float ptmRatio) : base(fileName)
        {
            this.ptmRatio = ptmRatio;
        }

        public b2Body PhysicsBody { get; set; }

        public void UpdateBodyTransform()
        {
            if (PhysicsBody != null)
            {
                b2Vec2 pos = PhysicsBody.Position;

                float x = pos.x * ptmRatio;
                float y = pos.y * ptmRatio;

                if (IgnoreAnchorPointForPosition) 
                {
                    x += AnchorPointInPoints.X * ptmRatio;
                    y += AnchorPointInPoints.Y * ptmRatio;
                }

                // Make matrix
                float radians = PhysicsBody.Angle;
                if (radians != 0)
                {
                    Rotation = CCMathHelper.ToDegrees(-radians);
                }

                PositionX = x;
                PositionY = y;
            }
        }
    }
}