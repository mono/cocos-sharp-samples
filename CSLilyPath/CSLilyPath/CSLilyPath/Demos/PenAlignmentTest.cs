using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class PenAlignmentTest : TestLayer
    {

        private List<CCVector2> _starPoints1;
        private List<CCVector2> _starPoints2;
        private List<CCVector2> _starPoints3;

        private Pen _insetPen;
        private Pen _centerPen;
        private Pen _outsetPen;

        private GraphicsPath _insetPath;
        private GraphicsPath _centerPath;
        private GraphicsPath _outsetPath;

        public PenAlignmentTest()
        {

            _starPoints1 = StarPoints(new CCVector2(125, 150), 5, 100, 50, 0, false);
            _starPoints2 = StarPoints(new CCVector2(350, 275), 5, 100, 50, 0, false);
            _starPoints3 = StarPoints(new CCVector2(125, 400), 5, 100, 50, 0, false);

            _insetPen = new Pen(Microsoft.Xna.Framework.Color.MediumTurquoise, 10)
            {
                Alignment = PenAlignment.Inset
            };
            _centerPen = new Pen(Microsoft.Xna.Framework.Color.MediumTurquoise, 10)
            {
                Alignment = PenAlignment.Center
            };
            _outsetPen = new Pen(Microsoft.Xna.Framework.Color.MediumTurquoise, 10)
            {
                Alignment = PenAlignment.Outset
            };

            _insetPath = new GraphicsPath(_insetPen, _starPoints1, PathType.Closed);
            _centerPath = new GraphicsPath(_centerPen, _starPoints2, PathType.Closed);
            _outsetPath = new GraphicsPath(_outsetPen, _starPoints3, PathType.Closed);
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
            drawBatch.DrawPath(_insetPath);
            drawBatch.DrawPrimitivePath(new Pen(Microsoft.Xna.Framework.Color.OrangeRed), _starPoints1, PathType.Closed);
            drawBatch.DrawPath(_centerPath);
            drawBatch.DrawPrimitivePath(new Pen(Microsoft.Xna.Framework.Color.OrangeRed), _starPoints2, PathType.Closed);
            drawBatch.DrawPath(_outsetPath);
            drawBatch.DrawPrimitivePath(new Pen(Microsoft.Xna.Framework.Color.OrangeRed), _starPoints3, PathType.Closed);
        }

    }
}
