using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class GraphicsPathTest : TestLayer
    {

        private Pen _thickPen;

        private GraphicsPath _gpathf;
        private GraphicsPath _gpathr;
        private GraphicsPath _gpath2f;
        private GraphicsPath _gpath2r;

        public GraphicsPathTest()
        {
            _thickPen = new Pen(Microsoft.Xna.Framework.Color.Green, 15);

            List<CCVector2> path1 = new List<CCVector2>() {
                new CCVector2(50, 50), new CCVector2(100, 50), new CCVector2(100, 100), new CCVector2(50, 100),
            };

            _gpathf = new GraphicsPath(_thickPen, path1, PathType.Closed);

            path1.Reverse();
            for (int i = 0; i < path1.Count; i++)
                path1[i] = new CCVector2(path1[i].X + 100, path1[i].Y);

            _gpathr = new GraphicsPath(_thickPen, path1, PathType.Closed);

            for (int i = 0; i < path1.Count; i++)
                path1[i] = new CCVector2(path1[i].X, path1[i].Y + 100);

            _gpath2r = new GraphicsPath(_thickPen, path1);

            path1.Reverse();
            for (int i = 0; i < path1.Count; i++)
                path1[i] = new CCVector2(path1[i].X - 100, path1[i].Y);

            _gpath2f = new GraphicsPath(_thickPen, path1);
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
            drawBatch.DrawPath(_gpathf);
            drawBatch.DrawPath(_gpathr);
            drawBatch.DrawPath(_gpath2f);
            drawBatch.DrawPath(_gpath2r);
        }

    }
}
