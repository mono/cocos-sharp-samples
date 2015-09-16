using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class PrimitiveArcs2 : TestLayer
    {

        public PrimitiveArcs2()
        {   }

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
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(50, 75), new CCVector2(150, 75), 25);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(50, 125), new CCVector2(150, 125), 50);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(50, 200), new CCVector2(150, 200), 75);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(50, 225), new CCVector2(150, 225), -75);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(50, 300), new CCVector2(150, 300), -50);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(50, 350), new CCVector2(150, 350), -25);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(275, 75), new CCVector2(175, 75), -25, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(275, 125), new CCVector2(175, 125), -50, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(275, 200), new CCVector2(175, 200), -75, 16);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(275, 225), new CCVector2(175, 225), 75, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(275, 300), new CCVector2(175, 300), 50, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(275, 350), new CCVector2(175, 350), 25, 16);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(325, 50), new CCVector2(325, 150), -25);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(375, 50), new CCVector2(375, 150), -50);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(450, 50), new CCVector2(450, 150), -75);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(475, 50), new CCVector2(475, 150), 75);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(550, 50), new CCVector2(550, 150), 50);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(600, 50), new CCVector2(600, 150), 25);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(325, 275), new CCVector2(325, 175), 25, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(375, 275), new CCVector2(375, 175), 50, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(450, 275), new CCVector2(450, 175), 75, 16);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(475, 275), new CCVector2(475, 175), -75, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(550, 275), new CCVector2(550, 175), -50, 16);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(600, 275), new CCVector2(600, 175), -25, 16);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(325, 300), new CCVector2(325, 400), -25, 4);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(375, 300), new CCVector2(375, 400), -50, 4);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(450, 300), new CCVector2(450, 400), -75, 4);

            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(475, 300), new CCVector2(475, 400), 75, 4);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(550, 300), new CCVector2(550, 400), 50, 4);
            drawBatch.DrawPrimitiveArc(Pen.Blue, new CCVector2(600, 300), new CCVector2(600, 400), 25, 4);
        }

    }
}
