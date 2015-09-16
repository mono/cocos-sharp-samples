using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using LilyPath;

namespace CSLilyPath
{
    public class IntroLayer : CCLayerColor
    {

        protected DrawBatch drawBatch;

        public IntroLayer()
            : base(CCColor4B.Blue)
        {
            drawBatch = new DrawBatch();
            AddChild(drawBatch, 10);
            
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;
            var center = bounds.Center;

            InitializePaths();

            drawBatch.FillCircle(new SolidColorBrush(Microsoft.Xna.Framework.Color.SkyBlue), center, 175);
            drawBatch.FillPath(new SolidColorBrush(Microsoft.Xna.Framework.Color.LimeGreen), _lilypadPath.Buffer, 0, _lilypadPath.Count);
            drawBatch.DrawPath(_lilypadStroke);
            drawBatch.DrawPath(_outerFlowerStroke);
            drawBatch.DrawPath(_innerFlowerStroke);

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

        private PathBuilder _lilypadPath;
        private GraphicsPath _lilypadStroke;
        private GraphicsPath _outerFlowerStroke;
        private GraphicsPath _innerFlowerStroke;

        private void InitializePaths()
        {
            var bounds = VisibleBoundsWorldspace;
            var center = bounds.Center;
            //var center = new CCVector2(200, 200);

            Pen lilypadPen = new Pen(CCColor4B.Green, 15)
            {
                Alignment = PenAlignment.Center
            };

            _lilypadPath = BuildLillyPad(center, 150, 0);
            _lilypadStroke = _lilypadPath.Stroke(lilypadPen, PathType.Closed);

            Pen outerFlowerPen = new Pen(CCColor4B.White * 0.75f, 15)
            {
                Alignment = PenAlignment.Outset
            };

            _outerFlowerStroke = BuildFlower(center, 8, 120, 100, (float)(Math.PI / 8)).Stroke(outerFlowerPen, PathType.Closed);

            Pen innerFlowerPen = new Pen(Microsoft.Xna.Framework.Color.MediumPurple * 0.5f, 10)
            {
                Alignment = PenAlignment.Outset
            };

            _innerFlowerStroke = BuildFlower(center, 16, 105, 60, 0).Stroke(innerFlowerPen, PathType.Closed);
        }

        private static PathBuilder BuildLillyPad(CCVector2 center, int radius, float rotation)
        {
            float segment = (float)(Math.PI * 2 / 32);

            PathBuilder builder = new PathBuilder();

            builder.AddPoint(center);
            builder.AddLine(radius, segment * 25 + rotation);
            builder.AddArcByAngle(center, segment * 30, radius / 2);

            return builder;
        }

        private static PathBuilder BuildFlower(CCVector2 center, int petalCount, float petalLength, float petalWidth, float rotation)
        {
            List<CCVector2> points = StarPoints(center, petalCount / 2, petalLength, petalLength, rotation, false);

            PathBuilder builder = new PathBuilder();
            builder.AddPoint(center);

            foreach (var point in points)
            {
                builder.AddArcByPoint(point, petalWidth / 2);
                builder.AddArcByPoint(center, petalWidth / 2);
            }

            return builder;
        }

        private static List<CCVector2> StarPoints(CCVector2 center, int pointCount, float outerRadius, float innerRadius, float rotation, bool close)
        {
            List<CCVector2> points = new List<CCVector2>();

            int limit = (close) ? pointCount * 2 + 1 : pointCount * 2;

            float rot = (float)((Math.PI * 2) / (pointCount * 2));
            for (int i = 0; i < limit; i++)
            {
                float si = (float)Math.Sin(-i * rot + Math.PI + rotation);
                float ci = (float)Math.Cos(-i * rot + Math.PI + rotation);

                if (i % 2 == 0)
                    points.Add(center + new CCVector2(si, ci) * outerRadius);
                else
                    points.Add(center + new CCVector2(si, ci) * innerRadius);
            }

            return points;
        }

    }
}

