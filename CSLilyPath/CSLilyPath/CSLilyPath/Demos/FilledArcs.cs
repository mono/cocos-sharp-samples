using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;

namespace CSLilyPath.Demos
{
    public class FilledArcs : TestLayer
    {

        public FilledArcs()
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
            drawBatch.FillArc(Brush.Blue, new CCVector2(100, 100), 75, -(float)(Math.PI * 0.25), -(float)(Math.PI * 0.5), ArcType.Segment);
            drawBatch.FillArc(Brush.Blue, new CCVector2(100, 125), 50, 0, -(float)Math.PI, ArcType.Segment);
            drawBatch.FillArc(Brush.Blue, new CCVector2(100, 200), 50, (float)(Math.PI * 0.25), -(float)(Math.PI * 1.5), ArcType.Segment);

            drawBatch.FillArc(Brush.Blue, new CCVector2(250, 100), 75, -(float)(Math.PI * 0.25), -(float)(Math.PI * 0.5), ArcType.Segment, 16);
            drawBatch.FillArc(Brush.Blue, new CCVector2(250, 125), 50, 0, -(float)Math.PI, ArcType.Segment, 16);
            drawBatch.FillArc(Brush.Blue, new CCVector2(250, 200), 50, (float)(Math.PI * 0.25), -(float)(Math.PI * 1.5), ArcType.Segment, 16);

            drawBatch.FillArc(Brush.Blue, new CCVector2(400, 100), 75, -(float)(Math.PI * 0.25), -(float)(Math.PI * 0.5), ArcType.Segment, 4);
            drawBatch.FillArc(Brush.Blue, new CCVector2(400, 125), 50, 0, -(float)Math.PI, ArcType.Segment, 4);
            drawBatch.FillArc(Brush.Blue, new CCVector2(400, 200), 50, (float)(Math.PI * 0.25), -(float)(Math.PI * 1.5), ArcType.Segment, 4);

            drawBatch.FillArc(Brush.Blue, new CCVector2(100, 335), 75, -(float)(Math.PI * 0.25), -(float)(Math.PI * 0.5), ArcType.Sector);
            drawBatch.FillArc(Brush.Blue, new CCVector2(100, 410), 50, 0, -(float)Math.PI, ArcType.Sector);
            drawBatch.FillArc(Brush.Blue, new CCVector2(100, 480), 50, (float)(Math.PI * 0.25), -(float)(Math.PI * 1.5), ArcType.Sector);

            drawBatch.FillArc(Brush.Blue, new CCVector2(250, 335), 75, -(float)(Math.PI * 0.25), -(float)(Math.PI * 0.5), ArcType.Sector, 16);
            drawBatch.FillArc(Brush.Blue, new CCVector2(250, 410), 50, 0, -(float)Math.PI, ArcType.Sector, 16);
            drawBatch.FillArc(Brush.Blue, new CCVector2(250, 480), 50, (float)(Math.PI * 0.25), -(float)(Math.PI * 1.5), ArcType.Sector, 16);

            drawBatch.FillArc(Brush.Blue, new CCVector2(400, 335), 75, -(float)(Math.PI * 0.25), -(float)(Math.PI * 0.5), ArcType.Sector, 4);
            drawBatch.FillArc(Brush.Blue, new CCVector2(400, 410), 50, 0, -(float)Math.PI, ArcType.Sector, 4);
            drawBatch.FillArc(Brush.Blue, new CCVector2(400, 480), 50, (float)(Math.PI * 0.25), -(float)(Math.PI * 1.5), ArcType.Sector, 4);
        }
    }
}
