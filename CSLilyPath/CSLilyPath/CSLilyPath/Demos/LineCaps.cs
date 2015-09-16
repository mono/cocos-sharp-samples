using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class LineCaps : TestLayer
    {

        private Pen _flatPen;
        private Pen _squarePen;
        private Pen _trianglePen;
        private Pen _invTrianglePen;
        private Pen _arrowPen;

        public LineCaps()
        {

            _flatPen = new Pen(Microsoft.Xna.Framework.Color.Blue, 15) { StartCap = LineCap.Flat, EndCap = LineCap.Flat };
            _squarePen = new Pen(Microsoft.Xna.Framework.Color.Red, 15) { StartCap = LineCap.Square, EndCap = LineCap.Square };
            _trianglePen = new Pen(Microsoft.Xna.Framework.Color.Green, 15) { StartCap = LineCap.Triangle, EndCap = LineCap.Triangle };
            _invTrianglePen = new Pen(Microsoft.Xna.Framework.Color.Purple, 15) { StartCap = LineCap.InvTriangle, EndCap = LineCap.InvTriangle };
            _arrowPen = new Pen(Microsoft.Xna.Framework.Color.Orange, 15) { StartCap = LineCap.Arrow, EndCap = LineCap.Arrow };
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
            float space = 30;
            float macroSpace = 50;
            float length = 200;

            CCVector2 o1 = new CCVector2(macroSpace, macroSpace);
            CCVector2 o2 = new CCVector2(macroSpace * 2 + length, macroSpace);
            CCVector2 o3 = new CCVector2(macroSpace * 2 + length, macroSpace * 2 + length);
            CCVector2 o4 = new CCVector2(macroSpace, macroSpace * 2 + length);

            drawBatch.DrawPath(new GraphicsPath(_flatPen, new List<CCVector2> { o1 + new CCVector2(0, space * 0), o1 + new CCVector2(length, space * 0) }));
            drawBatch.DrawPath(new GraphicsPath(_squarePen, new List<CCVector2> { o1 + new CCVector2(0, space * 1), o1 + new CCVector2(length, space * 1) }));
            drawBatch.DrawPath(new GraphicsPath(_trianglePen, new List<CCVector2> { o1 + new CCVector2(0, space * 2), o1 + new CCVector2(length, space * 2) }));
            drawBatch.DrawPath(new GraphicsPath(_invTrianglePen, new List<CCVector2> { o1 + new CCVector2(0, space * 3), o1 + new CCVector2(length, space * 3) }));
            drawBatch.DrawPath(new GraphicsPath(_arrowPen, new List<CCVector2> { o1 + new CCVector2(0, space * 4), o1 + new CCVector2(length, space * 4) }));

            drawBatch.DrawPath(new GraphicsPath(_flatPen, new List<CCVector2> { o2 + new CCVector2(space * 0, 0), o2 + new CCVector2(space * 0, length) }));
            drawBatch.DrawPath(new GraphicsPath(_squarePen, new List<CCVector2> { o2 + new CCVector2(space * 1, 0), o2 + new CCVector2(space * 1, length) }));
            drawBatch.DrawPath(new GraphicsPath(_trianglePen, new List<CCVector2> { o2 + new CCVector2(space * 2, 0), o2 + new CCVector2(space * 2, length) }));
            drawBatch.DrawPath(new GraphicsPath(_invTrianglePen, new List<CCVector2> { o2 + new CCVector2(space * 3, 0), o2 + new CCVector2(space * 3, length) }));
            drawBatch.DrawPath(new GraphicsPath(_arrowPen, new List<CCVector2> { o2 + new CCVector2(space * 4, 0), o2 + new CCVector2(space * 4, length) }));

            drawBatch.DrawPath(new GraphicsPath(_flatPen, new List<CCVector2> { o3 + new CCVector2(length, space * 0), o3 + new CCVector2(0, space * 0) }));
            drawBatch.DrawPath(new GraphicsPath(_squarePen, new List<CCVector2> { o3 + new CCVector2(length, space * 1), o3 + new CCVector2(0, space * 1) }));
            drawBatch.DrawPath(new GraphicsPath(_trianglePen, new List<CCVector2> { o3 + new CCVector2(length, space * 2), o3 + new CCVector2(0, space * 2) }));
            drawBatch.DrawPath(new GraphicsPath(_invTrianglePen, new List<CCVector2> { o3 + new CCVector2(length, space * 3), o3 + new CCVector2(0, space * 3) }));
            drawBatch.DrawPath(new GraphicsPath(_arrowPen, new List<CCVector2> { o3 + new CCVector2(length, space * 4), o3 + new CCVector2(0, space * 4) }));

            drawBatch.DrawPath(new GraphicsPath(_flatPen, new List<CCVector2> { o4 + new CCVector2(space * 0, length), o4 + new CCVector2(space * 0, 0) }));
            drawBatch.DrawPath(new GraphicsPath(_squarePen, new List<CCVector2> { o4 + new CCVector2(space * 1, length), o4 + new CCVector2(space * 1, 0) }));
            drawBatch.DrawPath(new GraphicsPath(_trianglePen, new List<CCVector2> { o4 + new CCVector2(space * 2, length), o4 + new CCVector2(space * 2, 0) }));
            drawBatch.DrawPath(new GraphicsPath(_invTrianglePen, new List<CCVector2> { o4 + new CCVector2(space * 3, length), o4 + new CCVector2(space * 3, 0) }));
            drawBatch.DrawPath(new GraphicsPath(_arrowPen, new List<CCVector2> { o4 + new CCVector2(space * 4, length), o4 + new CCVector2(space * 4, 0) }));
        }

    }
}
