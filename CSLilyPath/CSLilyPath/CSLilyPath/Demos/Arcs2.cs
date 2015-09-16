using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;

namespace CSLilyPath.Demos
{
    public class Arcs2 : TestLayer
    {
        private Pen _thickPen;

        public Arcs2 ()
        {

            _thickPen = new Pen(Microsoft.Xna.Framework.Color.Blue, 15);
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
            drawBatch.DrawArc(_thickPen, new CCVector2(50, 75), new CCVector2(150, 75), 25);
            drawBatch.DrawArc(_thickPen, new CCVector2(50, 125), new CCVector2(150, 125), 50);
            drawBatch.DrawArc(_thickPen, new CCVector2(50, 200), new CCVector2(150, 200), 75);

            drawBatch.DrawArc(_thickPen, new CCVector2(50, 225), new CCVector2(150, 225), -75);
            drawBatch.DrawArc(_thickPen, new CCVector2(50, 300), new CCVector2(150, 300), -50);
            drawBatch.DrawArc(_thickPen, new CCVector2(50, 350), new CCVector2(150, 350), -25);

            drawBatch.DrawArc(_thickPen, new CCVector2(175, 75), new CCVector2(275, 75), 25, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(175, 125), new CCVector2(275, 125), 50, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(175, 200), new CCVector2(275, 200), 75, 16);

            drawBatch.DrawArc(_thickPen, new CCVector2(175, 225), new CCVector2(275, 225), -75, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(175, 300), new CCVector2(275, 300), -50, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(175, 350), new CCVector2(275, 350), -25, 16);

            drawBatch.DrawArc(_thickPen, new CCVector2(325, 50), new CCVector2(325, 150), -25);
            drawBatch.DrawArc(_thickPen, new CCVector2(375, 50), new CCVector2(375, 150), -50);
            drawBatch.DrawArc(_thickPen, new CCVector2(450, 50), new CCVector2(450, 150), -75);

            drawBatch.DrawArc(_thickPen, new CCVector2(475, 50), new CCVector2(475, 150), 75);
            drawBatch.DrawArc(_thickPen, new CCVector2(550, 50), new CCVector2(550, 150), 50);
            drawBatch.DrawArc(_thickPen, new CCVector2(600, 50), new CCVector2(600, 150), 25);

            drawBatch.DrawArc(_thickPen, new CCVector2(325, 175), new CCVector2(325, 275), -25, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(375, 175), new CCVector2(375, 275), -50, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(450, 175), new CCVector2(450, 275), -75, 16);

            drawBatch.DrawArc(_thickPen, new CCVector2(475, 175), new CCVector2(475, 275), 75, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(550, 175), new CCVector2(550, 275), 50, 16);
            drawBatch.DrawArc(_thickPen, new CCVector2(600, 175), new CCVector2(600, 275), 25, 16);

            drawBatch.DrawArc(_thickPen, new CCVector2(325, 300), new CCVector2(325, 400), -25, 4);
            drawBatch.DrawArc(_thickPen, new CCVector2(375, 300), new CCVector2(375, 400), -50, 4);
            drawBatch.DrawArc(_thickPen, new CCVector2(450, 300), new CCVector2(450, 400), -75, 4);

            drawBatch.DrawArc(_thickPen, new CCVector2(475, 300), new CCVector2(475, 400), 75, 4);
            drawBatch.DrawArc(_thickPen, new CCVector2(550, 300), new CCVector2(550, 400), 50, 4);
            drawBatch.DrawArc(_thickPen, new CCVector2(600, 300), new CCVector2(600, 400), 25, 4);
        }
    }
}
