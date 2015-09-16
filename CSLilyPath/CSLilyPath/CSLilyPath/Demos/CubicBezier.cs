using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;

namespace CSLilyPath.Demos
{
    public class CubicBezier : TestLayer
    {
        Pen _bluePen;
        Pen _pointPen;

        CCVector2[] _wavePoints;
        CCVector2[] _loopPoints;

        public CubicBezier()
        {

            _bluePen = new Pen(Microsoft.Xna.Framework.Color.Blue, 15);
            _pointPen = new Pen(Microsoft.Xna.Framework.Color.Gray, 4);

            _wavePoints = new CCVector2[] {
                new CCVector2(50, 350), new CCVector2(50, 400), new CCVector2(150, 400), new CCVector2(150, 350), new CCVector2(150, 300),
                new CCVector2(250, 300), new CCVector2(250, 350), new CCVector2(250, 400), new CCVector2(350, 400), new CCVector2(350, 350),
                new CCVector2(350, 300), new CCVector2(450, 300), new CCVector2(450, 350),
            };

            _loopPoints = new CCVector2[] {
                new CCVector2(225, 75), new CCVector2(250, 50), new CCVector2(275, 50), new CCVector2(300, 75),
                new CCVector2(325, 100), new CCVector2(325, 125), new CCVector2(300, 150), new CCVector2(275, 175),
                new CCVector2(250, 175), new CCVector2(225, 150), new CCVector2(200, 125), new CCVector2(200, 100),
            };
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
            drawBatch.DrawBezier(_bluePen, new CCVector2(50, 100), new CCVector2(50, 50), new CCVector2(150, 50), new CCVector2(150, 100));
            drawBatch.DrawBezier(_bluePen, new CCVector2(50, 250), new CCVector2(50, 150), new CCVector2(150, 150), new CCVector2(150, 250));

            drawBatch.DrawBeziers(_bluePen, _wavePoints, BezierType.Cubic);
            drawBatch.DrawPrimitivePath(Pen.Gray, _wavePoints);

            for (int i = 0; i < _wavePoints.Length; i++)
                drawBatch.DrawPoint(_pointPen, _wavePoints[i]);

            drawBatch.DrawBeziers(_bluePen, _loopPoints, BezierType.Cubic, PathType.Closed);
            drawBatch.DrawPrimitivePath(Pen.Gray, _loopPoints, PathType.Closed);

            for (int i = 0; i < _loopPoints.Length; i++)
                drawBatch.DrawPoint(_pointPen, _loopPoints[i]);
        }
    }
}
