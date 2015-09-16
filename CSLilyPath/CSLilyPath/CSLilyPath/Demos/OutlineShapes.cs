using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

namespace CSLilyPath.Demos
{
    public class OutlineShapes : TestLayer
    {

        private List<CCVector2> _wavy = new List<CCVector2>();
        private GraphicsPath _wavyPath;

        private Pen _thickBlue;
        private Pen _thickRed;
        private Pen _thickMagenta;
        private Pen _thickBlack;
        private Pen _thickDarkGray;
        private Pen _thickGreen;

        public OutlineShapes()
        {

            for (int i = 0; i < 20; i++)
            {
                if (i % 2 == 0)
                    _wavy.Add(new CCVector2(50 + i * 10, 100));
                else
                    _wavy.Add(new CCVector2(50 + i * 10, 110));
            }

            _thickBlue = new Pen(Microsoft.Xna.Framework.Color.Blue, 15);
            _thickRed = new Pen(Microsoft.Xna.Framework.Color.Red, 15)
            {
                EndCap = LineCap.Square,
                StartCap = LineCap.Square,
            };
            _thickMagenta = new Pen(Microsoft.Xna.Framework.Color.Magenta, 15);
            _thickBlack = new Pen(Microsoft.Xna.Framework.Color.Black, 15);
            _thickDarkGray = new Pen(Microsoft.Xna.Framework.Color.DarkGray, 15);
            _thickGreen = new Pen(Microsoft.Xna.Framework.Color.Green, 15);

            _wavyPath = new GraphicsPath(_thickRed, _wavy);
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
            drawBatch.DrawLine(_thickBlue, new CCVector2(50, 50), new CCVector2(250, 50));
            drawBatch.DrawPath(_wavyPath);
            drawBatch.DrawRectangle(_thickMagenta, new CCRect(50, 160, 200, 100));
            drawBatch.DrawCircle(_thickBlack, new CCVector2(350, 100), 50);
            drawBatch.DrawCircle(_thickDarkGray, new CCVector2(350, 225), 50, 16);
            drawBatch.DrawRectangle(_thickGreen, new CCRect(50, 350, 200, 100), (float)Math.PI / 4f);
        }

    }
}
