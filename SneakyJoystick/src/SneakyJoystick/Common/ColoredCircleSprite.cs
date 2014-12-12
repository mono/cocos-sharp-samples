using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{
    public class ColoredCircleSprite : CCSprite
    {

        public float radius_;

        public int numberOfSegments;
        public CCPoint[] circleVertices;

        public ColoredCircleSprite(CCColor4B color, float r)
        {
            radius_ = r;
            Opacity = color.A;
            Color = new CCColor3B(color.R, color.G, color.B);
            PreInit();
        }

        public ColoredCircleSprite()
        {
            PreInit();
        }

        public void PreInit()
        {
            Radius = 10.0f;
            numberOfSegments = 36;

            //self.shaderProgram = [[CCShaderCache sharedShaderCache] programForKey:kCCShader_PositionColor];
            // default blend function
            //blendFunc = new CCBlendFunc( CCOGLES. CCDrawManager.BlendState.ColorSourceBlend, ;  //new CCBlendFunc( CC_BLEND_SRC, CC_BLEND_DST };
            //var tmpColor = ;
            Color = new CCColor3B(CCColor4B.White);
            //Opacity = tmpColor.A;

            circleVertices = new CCPoint[numberOfSegments]; // new CCPoint( malloc(sizeof(CGPoint)*(numberOfSegments));

            if (circleVertices.Count() == 0)
                Console.WriteLine(@"Ack!! malloc in colored circle failed");

        }

        public float Radius
        {
            get { return radius_; }
            set
            {
                radius_ = value;
                float theta_inc = 2.0f * 3.14159265359f / numberOfSegments;
                float theta = 0.0f;

                for (int i = 0; i < numberOfSegments; i++)
                {
                    //#ifdef __IPHONE_OS_VERSION_MAX_ALLOWED
                    //        float j = radius_ * cosf(theta) + self.position.x;
                    //        float k = radius_ * sinf(theta) + self.position.y;
                    //#elif defined(__MAC_OS_X_VERSION_MAX_ALLOWED)
                    //        float j = radius_ * cosf(theta) + position_.x;
                    //        float k = radius_ * sinf(theta) + position_.y;
                    //#endif				

                    float j = radius_ * CCMathHelper.Cos(theta) + Position.X;
                    float k = radius_ * CCMathHelper.Sin(theta) + Position.Y;
                    //        float k = radius_ * sinf(theta) + self.position.y;
                    circleVertices[i] = new CCPoint(j, k);
                    theta += theta_inc;
                }

                updateContentSize();
            }
        }

        public override CCSize ContentSize
        {
            get { return base.ContentSize; }
            set
            {
                radius_ = value.Width / 2;
                updateContentSize();
            }
        }

        public void updateContentSize()
        {
            base.ContentSize = new CCSize(radius_ * 2, radius_ * 2);
        }

        protected override void Draw()
        {
            base.Draw();
            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawSolidPoly(circleVertices, numberOfSegments, new CCColor4B(Color.R, Color.G, Color.B));
            CCDrawingPrimitives.End();
        }

        public bool isCascadeColorEnabled
        {
            get
            {
                return true;
            }
        }

        public bool containsPoint(CCPoint point)
        {
            float dSq = point.X * point.X + point.Y * point.Y;
            float rSq = radius_ * radius_;
            return (dSq <= rSq);
        }

        public override string ToString()
        {
            return String.Format(@"<%@ = {0} | Tag = {3} | Color = {1} | Radius ={2}>", GetType().ToString(), Color.ToString(), Opacity, radius_);
        }
    }
}
