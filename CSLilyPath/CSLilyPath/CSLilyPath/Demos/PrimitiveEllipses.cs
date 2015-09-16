using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class PrimitiveEllipses : TestLayer
    {

        public PrimitiveEllipses()
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
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(50, 50, 50, 50));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(125, 50, 100, 50));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(250, 50, 150, 50));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(50, 125, 50, 100));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(125, 125, 100, 100));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(250, 125, 150, 100));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(50, 250, 50, 150));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(125, 250, 100, 150));
            drawBatch.DrawPrimitiveEllipse(Pen.Blue, new CCRect(250, 250, 150, 150));

            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(425, 50, 100, 50), 0);
            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(425, 150, 100, 50), (float)Math.PI / 8);
            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(425, 250, 100, 50), (float)Math.PI / 4);
            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(425, 350, 100, 50), (float)Math.PI / 8 * 3);

            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(50, 425, 50, 100), 0);
            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(150, 425, 50, 100), (float)-Math.PI / 8);
            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(250, 425, 50, 100), (float)-Math.PI / 4);
            drawBatch.DrawPrimitiveEllipse(Pen.Red, new CCRect(350, 425, 50, 100), (float)-Math.PI / 8 * 3);
        }

    }
}
