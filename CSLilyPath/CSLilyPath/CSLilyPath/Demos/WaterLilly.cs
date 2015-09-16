using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class WaterLilly : TestLayer
    {

        private CCVector2 _origin = new CCVector2(200, 200);
        private float _startAngle = (float)(Math.PI / 16) * 25; // 11:20
        private float _arcLength = (float)(Math.PI / 16) * 30;

        private GraphicsPath _lilyOuterFlower;
        private GraphicsPath _lilyInnerFlower;

        public WaterLilly()
        {
            Pen penOuterFlower = new Pen(Microsoft.Xna.Framework.Color.White * 0.75f, 15) { Alignment = PenAlignment.Outset };
            _lilyOuterFlower = CreateFlowerGP(penOuterFlower, _origin, 8, 120, 100, (float)(Math.PI / 8));

            Pen penInnerFlower = new Pen(Microsoft.Xna.Framework.Color.MediumPurple * 0.5f, 10) { Alignment = PenAlignment.Outset };
            _lilyInnerFlower = CreateFlowerGP(penInnerFlower, _origin, 16, 105, 60, 0);
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
            drawBatch.FillCircle(new SolidColorBrush(Microsoft.Xna.Framework.Color.SkyBlue), _origin, 175);
            drawBatch.FillArc(new SolidColorBrush(Microsoft.Xna.Framework.Color.LimeGreen), _origin, 150, _startAngle, _arcLength, ArcType.Sector);
            drawBatch.DrawClosedArc(new Pen(Microsoft.Xna.Framework.Color.Green, 15), _origin, 150, _startAngle, _arcLength, ArcType.Sector);
            drawBatch.DrawPath(_lilyOuterFlower);
            drawBatch.DrawPath(_lilyInnerFlower);
        }

        private GraphicsPath CreateFlowerGP(Pen pen, CCVector2 center, int petalCount, float petalLength, float petalWidth, float rotation)
        {
            List<CCVector2> points = StarPoints(center, petalCount / 2, petalLength, petalLength, rotation, false);

            PathBuilder builder = new PathBuilder();
            builder.AddPoint(center);

            foreach (CCVector2 point in points)
            {
                builder.AddArcByPoint(point, petalWidth / 2);
                builder.AddArcByPoint(center, petalWidth / 2);
            }

            return builder.Stroke(pen, PathType.Closed);
        }


    }
}
