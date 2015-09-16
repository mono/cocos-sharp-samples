using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class GradientPensAlignment : TestLayer
    {

        private CCVector2[] _baseCoords = new CCVector2[] { new CCVector2(0, 0), new CCVector2(25, 50), new CCVector2(0, 100) };

        private Pen _gradWidthInner;
        private Pen _gradWidthCenter;
        private Pen _gradWidthOuter;

        private GraphicsPath[] _gPaths = new GraphicsPath[3];

        public GradientPensAlignment()
        {
            _gradWidthInner = new GradientPen(Microsoft.Xna.Framework.Color.Lime, Microsoft.Xna.Framework.Color.Cyan, 15) { Alignment = PenAlignment.Inset, StartCap = LineCap.Square, EndCap = LineCap.Square };
            _gradWidthCenter = new GradientPen(Microsoft.Xna.Framework.Color.Lime, Microsoft.Xna.Framework.Color.Cyan, 15) { StartCap = LineCap.Square, EndCap = LineCap.Square };
            _gradWidthOuter = new GradientPen(Microsoft.Xna.Framework.Color.Lime, Microsoft.Xna.Framework.Color.Cyan, 15) { Alignment = PenAlignment.Outset, StartCap = LineCap.Square, EndCap = LineCap.Square };

            Pen[] pens = new Pen[] { _gradWidthInner, _gradWidthCenter, _gradWidthOuter };
            for (int i = 0; i < _gPaths.Length; i++)
            {
                PathBuilder builder = new PathBuilder();
                foreach (CCVector2 v in _baseCoords)
                    builder.AddPoint(v + Offset(i));
                _gPaths[i] = builder.Stroke(pens[i]);
            }
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
            for (int i = 0; i < _gPaths.Length; i++)
            {
                GraphicsPath path = _gPaths[i];
                drawBatch.DrawPath(path);
                for (int j = 0; j < _baseCoords.Length - 1; j++)
                    drawBatch.DrawLine(PrimitivePen.Black, _baseCoords[j] + Offset(i), _baseCoords[j + 1] + Offset(i));
            }
        }

        private static CCVector2 Offset(int i)
        {
            return new CCVector2(100 + i * 50, 100);
        }
    }
}
