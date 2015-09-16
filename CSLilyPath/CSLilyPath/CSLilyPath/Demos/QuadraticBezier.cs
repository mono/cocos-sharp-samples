using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class QuadraticBezier : TestLayer
    {

        Pen _bluePen;
        Pen _pointPen;

        CCVector2[] _wavePoints1;
        CCVector2[] _wavePoints2;
        CCVector2[] _loopPoints;

        public QuadraticBezier()
        {
            _bluePen = new Pen(Microsoft.Xna.Framework.Color.Blue, 15);
            _pointPen = new Pen(Microsoft.Xna.Framework.Color.Gray, 4);

            _wavePoints1 = new CCVector2[] {
                new CCVector2(150, 100), new CCVector2(200, 150), new CCVector2(250, 100), new CCVector2(300, 50), new CCVector2(350, 100),
                new CCVector2(400, 150), new CCVector2(450, 100), new CCVector2(500, 50), new CCVector2(550, 100),
            };

            _wavePoints2 = new CCVector2[] {
                new CCVector2(150, 200), new CCVector2(200, 300), new CCVector2(250, 200), new CCVector2(300, 100), new CCVector2(350, 200),
                new CCVector2(400, 300), new CCVector2(450, 200), new CCVector2(500, 100), new CCVector2(550, 200),
            };

            _loopPoints = new CCVector2[] {
                new CCVector2(250, 300), new CCVector2(350, 300), new CCVector2(350, 400), new CCVector2(350, 500),
                new CCVector2(250, 500), new CCVector2(150, 500), new CCVector2(150, 400), new CCVector2(150, 300),
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
            drawBatch.DrawBezier(_bluePen, new CCVector2(50, 100), new CCVector2(50, 50), new CCVector2(100, 50));
            drawBatch.DrawBezier(_bluePen, new CCVector2(50, 250), new CCVector2(50, 150), new CCVector2(100, 150));

            drawBatch.DrawBeziers(_bluePen, _wavePoints1, BezierType.Quadratic);
            drawBatch.DrawPrimitivePath(Pen.Gray, _wavePoints1);

            for (int i = 0; i < _wavePoints1.Length; i++)
                drawBatch.DrawPoint(_pointPen, _wavePoints1[i]);

            drawBatch.DrawBeziers(_bluePen, _wavePoints2, BezierType.Quadratic);
            drawBatch.DrawPrimitivePath(Pen.Gray, _wavePoints2);

            for (int i = 0; i < _wavePoints2.Length; i++)
                drawBatch.DrawPoint(_pointPen, _wavePoints2[i]);

            drawBatch.DrawBeziers(_bluePen, _loopPoints, BezierType.Quadratic, PathType.Closed);
            drawBatch.DrawPrimitivePath(Pen.Gray, _loopPoints, PathType.Closed);

            for (int i = 0; i < _loopPoints.Length; i++)
                drawBatch.DrawPoint(_pointPen, _loopPoints[i]);
        }

    }
}
