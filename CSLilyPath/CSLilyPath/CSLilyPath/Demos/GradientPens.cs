using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class GradientPens : TestLayer
    {

        private Pen _gradWidth;
        private Pen _gradLength;

        private GraphicsPath _widthStar;
        private GraphicsPath _lengthStar;

        public GradientPens()
        {

            _gradWidth = new GradientPen(Microsoft.Xna.Framework.Color.Lime, Microsoft.Xna.Framework.Color.Blue, 15);
            _gradLength = new PathGradientPen(Microsoft.Xna.Framework.Color.Lime, Microsoft.Xna.Framework.Color.Blue, 15);

            PathBuilder pathBuilder = new PathBuilder() { CalculateLengths = true };
            pathBuilder.AddPath(StarPoints(new CCVector2(325, 75), 5, 50, 25, 0, false));

            _widthStar = pathBuilder.Stroke(_gradWidth, PathType.Open);
            _lengthStar = pathBuilder.Stroke(_gradLength, 
                CCAffineTransform.Translate(CCAffineTransform.Identity, 0, 125, 0), 
                PathType.Open);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();


            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
            }
        }

        public override void Draw(DrawBatch drawBatch)
        {
            drawBatch.DrawLine(_gradWidth, new CCVector2(25, 25), new CCVector2(125, 125));
            drawBatch.DrawCircle(_gradWidth, new CCVector2(200, 75), 50);
            drawBatch.DrawPath(_widthStar);

            drawBatch.DrawLine(_gradLength, new CCVector2(25, 150), new CCVector2(125, 250));
            drawBatch.DrawCircle(_gradLength, new CCVector2(200, 200), 50);
            drawBatch.DrawPath(_lengthStar);
        }
    }
}
