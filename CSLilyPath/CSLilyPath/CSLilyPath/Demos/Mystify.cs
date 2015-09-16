using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class Mystify : TestLayer
    {
        private class Figure
        {
            private static Random rand = new Random();

            public CCVector2[] Points;
            public CCVector2[] Velocities;
            public List<CCVector2[]> History;

            public Pen ColorPen;
            public float Skip = 0;
            public float SkipLimit = .03f;

            public Figure(int pointCount, int lineCount)
            {
                Points = new CCVector2[pointCount];
                Velocities = new CCVector2[pointCount];

                History = new List<CCVector2[]>();
                for (int i = 0; i < lineCount; i++)
                {
                    History.Add(new CCVector2[pointCount]);
                }
            }

            public void Initialize(CCRect bounds, float mag)
            {
                ColorPen = new Pen(new Microsoft.Xna.Framework.Color(rand.Next(255), rand.Next(255), rand.Next(255)));

                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i] = new CCVector2(bounds.MinX + (float)rand.NextDouble() * bounds.Size.Width, bounds.MaxY + (float)rand.NextDouble() * bounds.Size.Height);
                    Velocities[i] = new CCVector2((float)(rand.NextDouble() - .5) * mag, (float)(rand.NextDouble() - .5) * mag);

                    for (int j = 0; j < History.Count; j++)
                        History[j][i] = Points[i];
                }
            }

            public void Update(CCRect bounds, float time)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i] += Velocities[i] * time;
                    if (Points[i].X < bounds.MinX)
                        Velocities[i].X = Math.Abs(Velocities[i].X);
                    else if (Points[i].X > bounds.MaxX)
                        Velocities[i].X = -Math.Abs(Velocities[i].X);
                    if (Points[i].Y < bounds.MaxY)
                        Velocities[i].Y = Math.Abs(Velocities[i].Y);
                    else if (Points[i].Y > bounds.MinY)
                        Velocities[i].Y = -Math.Abs(Velocities[i].Y);
                }

                Skip += time;
                if (Skip >= SkipLimit)
                {
                    Skip = 0;
                    for (int i = History.Count - 1; i > 0; i--)
                        History[i - 1].CopyTo(History[i], 0);
                    Points.CopyTo(History[0], 0);
                }
            }
        }

        private List<Figure> _figures = new List<Figure>();
        private CCRect _bounds;
        float accumulatedTime = 0;

        public Mystify()
        {
            ClearColor = CCColor4B.Black;
            Schedule((dt) => 
            {
                accumulatedTime += dt;

                foreach (Figure fig in _figures)
                {
                    fig.Update(_bounds, accumulatedTime);
                    
                }
                drawBatch.ClearInstances();
                Draw(drawBatch);
            });
        }

        public override void OnEnter()
        {
            
            _bounds = VisibleBoundsWorldspace;

            _figures.Add(new Figure(4, 5));
            _figures.Add(new Figure(4, 7));

            for (int i = 0; i < _figures.Count; i++)
                _figures[i].Initialize(_bounds, 400);
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
            _bounds = VisibleBoundsWorldspace;

            foreach (Figure fig in _figures)
            {
                foreach (var points in fig.History)
                {
                    drawBatch.DrawPrimitivePath(fig.ColorPen, points, PathType.Closed);
                }
            }
        }

    }
}
