using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class PrimitiveShapes : TestLayer
    {

        private List<CCVector2> _wavy = new List<CCVector2>();

        public PrimitiveShapes()
        {
            for (int i = 0; i < 20; i++)
            {
                if (i % 2 == 0)
                    _wavy.Add(new CCVector2(50 + i * 10, 100));
                else
                    _wavy.Add(new CCVector2(50 + i * 10, 110));
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
            drawBatch.DrawPrimitiveLine(Pen.Blue, new CCVector2(50, 50), new CCVector2(250, 50));
            drawBatch.DrawPrimitivePath(Pen.Red, _wavy);
            drawBatch.DrawPrimitiveRectangle(Pen.Magenta, new CCRect(50, 160, 200, 100));
            drawBatch.DrawPrimitiveCircle(Pen.Black, new CCVector2(350, 100), 50);
            drawBatch.DrawPrimitiveCircle(Pen.DarkGray, new CCVector2(350, 225), 50, 16);
            drawBatch.DrawPrimitiveRectangle(Pen.Green, new CCRect(50, 350, 200, 100), (float)Math.PI / 4f);
        }

    }
}
