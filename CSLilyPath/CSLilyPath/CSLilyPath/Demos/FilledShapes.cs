using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;

namespace CSLilyPath.Demos
{
    public class FilledShapes : TestLayer
    {

        public FilledShapes()
        {

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
            drawBatch.FillRectangle(Brush.Green, new CCRect(50, 50, 200, 100));
            drawBatch.FillCircle(Brush.Blue, new CCVector2(350, 100), 50);
            drawBatch.FillCircle(Brush.Blue, new CCVector2(500, 100), 50, 16);
            drawBatch.FillPath(Brush.Gray, StarPoints(new CCVector2(150, 300), 8, 100, 50, 0, false));
            drawBatch.FillRectangle(Brush.Green, new CCRect(300, 250, 200, 100), (float)Math.PI / 4f);
        }
    }
}
