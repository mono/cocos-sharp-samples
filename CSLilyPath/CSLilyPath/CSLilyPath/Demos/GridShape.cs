using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;
using LilyPath.Shapes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CSLilyPath.Demos
{
    public class GridShape : TestLayer
    {

        private DrawCache _cache;

        public GridShape()
        {

        }

        public override void OnEnter()
        {
            var penWidth = 6.0f;
            Pen pen = new Pen(new Microsoft.Xna.Framework.Color(Microsoft.Xna.Framework.Color.Blue, 92), penWidth);

            var bounds = VisibleBoundsWorldspace;
            var size = bounds.Size;
            var width = size.Width - penWidth;
            var height = size.Height - penWidth;
            //_cache = new Grid(12, 10).Compile(pen, 3, 3, 30 * 12, 30 * 10);
            _cache = new Grid(12, 10).Compile(pen, penWidth/2, penWidth/2, width, height);

            base.OnEnter();
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
            var center = VisibleBoundsWorldspace.Center - new CCPoint(150,150);
            
            //drawBatch.FillRectangle(Brush.Gray, new CCVector2(200, 15), 300, 300);
            drawBatch.FillRectangle(Brush.Gray, center, 300, 300);
            drawBatch.DrawCache(_cache);

        }

    }
}
