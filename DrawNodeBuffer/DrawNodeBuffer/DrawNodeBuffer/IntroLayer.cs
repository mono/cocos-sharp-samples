using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace DrawNodeBuffer
{
    public class IntroLayer : CCLayerColor
    {

        DrawNodeBuffer drawBuffer = new DrawNodeBuffer();

        public IntroLayer()
            : base(CCColor4B.Black)
        {


        }

        public override void OnEnter()
        {
            base.OnEnter();

            CreateGeometry();

            var draw = new CCDrawNode();
            draw.BlendFunc = CCBlendFunc.NonPremultiplied;

            AddChild(draw, 10);

            draw.DrawTriangleList(drawBuffer.TriangleVertices.ToArray());
            draw.DrawLineList(drawBuffer.LineVertices.ToArray());

        }

        void CreateGeometry()
        {
            var windowSize = Layer.VisibleBoundsWorldspace.Size;

            // Draw 10 circles
            for (int i = 0; i < 10; i++)
            {
                drawBuffer.DrawSolidCircle(windowSize.Center, 10 * (10 - i),
                    new CCColor4F(CCRandom.Float_0_1(), CCRandom.Float_0_1(), CCRandom.Float_0_1(), 1));
            }

            // Draw polygons
            CCPoint[] points = new CCPoint[]
            {
                new CCPoint(windowSize.Height / 4, 0),
                new CCPoint(windowSize.Width, windowSize.Height / 5),
                new CCPoint(windowSize.Width / 3 * 2, windowSize.Height)
            };
            drawBuffer.DrawPolygon(points, points.Length, new CCColor4F(1.0f, 0, 0, 0.5f), 4, new CCColor4F(0, 0, 1, 1));

            // star poly (triggers buggs)
            {
                const float o = 80;
                const float w = 20;
                const float h = 50;
                CCPoint[] star = new CCPoint[]
	            {
	                new CCPoint(o + w, o - h), new CCPoint(o + w * 2, o),                           // lower spike
	                new CCPoint(o + w * 2 + h, o + w), new CCPoint(o + w * 2, o + w * 2),           // right spike
	            };

                drawBuffer.DrawPolygon(star, star.Length, new CCColor4F(1, 0, 0, 0.5f), 1, new CCColor4F(0, 0, 1, 1));
            }

            // star poly (doesn't trigger bug... order is important un tesselation is supported.
            {
                const float o = 180;
                const float w = 20;
                const float h = 50;
                var star = new CCPoint[]
                {
                    new CCPoint(o, o), new CCPoint(o + w, o - h), new CCPoint(o + w * 2, o),        // lower spike
                    new CCPoint(o + w * 2 + h, o + w), new CCPoint(o + w * 2, o + w * 2),           // right spike
                    new CCPoint(o + w, o + w * 2 + h), new CCPoint(o, o + w * 2),                   // top spike
                    new CCPoint(o - h, o + w), // left spike
                };

                drawBuffer.DrawPolygon(star, star.Length, new CCColor4F(1, 0, 0, 0.5f), 1, new CCColor4F(0, 0, 1, 1));
            }


            // Draw segment
            drawBuffer.DrawLine(new CCPoint(20, windowSize.Height), new CCPoint(20, windowSize.Height / 2), 10, new CCColor4F(0, 1, 0, 1), DrawNodeBuffer.LineCap.Round);

            drawBuffer.DrawLine(new CCPoint(10, windowSize.Height / 2), new CCPoint(windowSize.Width / 2, windowSize.Height / 2), 40,
                new CCColor4F(1, 0, 1, 0.5f), DrawNodeBuffer.LineCap.Round);

            CCSize size = VisibleBoundsWorldspace.Size;

            var visibleRect = VisibleBoundsWorldspace;

            // draw quad bezier path
            drawBuffer.DrawQuadBezier(new CCPoint(0, size.Height),
                visibleRect.Center,
                (CCPoint)visibleRect.Size,
                50, 3,
                new CCColor4B(255, 0, 255, 255));

            // draw cubic bezier path
            drawBuffer.DrawCubicBezier(visibleRect.Center,
                new CCPoint(size.Width / 2 + 30, size.Height / 2 + 50),
                new CCPoint(size.Width / 2 + 60, size.Height / 2 - 50),
                new CCPoint(size.Width, size.Height / 2), 100, 2, CCColor4B.Green);

            // draw an ellipse within rectangular region
            drawBuffer.DrawEllipse(new CCRect(100, 300, 100, 200), 8, CCColor4B.AliceBlue);

            var splinePoints = new List<CCPoint>();
            splinePoints.Add(new CCPoint(0, 0));
            splinePoints.Add(new CCPoint(50, 70));
            splinePoints.Add(new CCPoint(0, 140));
            splinePoints.Add(new CCPoint(100, 210));
            splinePoints.Add(new CCPoint(0, 280));
            splinePoints.Add(new CCPoint(150, 350));

            int numberOfSegments = 64;
            float tension = .05f;
            drawBuffer.DrawCardinalSpline(splinePoints, tension, numberOfSegments);

            drawBuffer.DrawSolidArc(
                pos: new CCPoint(350, windowSize.Height * 0.75f),
                radius: 100,
                startAngle: CCMathHelper.ToRadians(45),
                sweepAngle: CCMathHelper.Pi / 2, // this is in radians, clockwise
                color: CCColor4B.Aquamarine);


        }


        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

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
    }
}

