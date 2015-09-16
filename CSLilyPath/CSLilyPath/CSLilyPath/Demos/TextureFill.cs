using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;
using LilyPath.Pens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CSLilyPath.Demos
{
    public class TextureFill : TestLayer
    {

        private CCTexture2D _xor6;

        private TextureBrush _brush1;
        private TextureBrush _brush2;
        private TextureBrush _brush3;
        private TextureBrush _brush4;
        private TextureBrush _brush5;
        private TextureBrush _brush6;

        private TextureBrush repeatBrush;
        private TextureBrush mirrorBrush;

        public TextureFill()
        {
            
            var _xor6 = BuildXorTexture(6);
            _xor6.SamplerState = SamplerState.LinearWrap;
            _brush1 = new TextureBrush(_xor6);
            _brush2 = new TextureBrush(_xor6)
            {
                Transform = Matrix.CreateTranslation(-50f / _xor6.XNATexture.Width, -175f / _xor6.XNATexture.Height, 0)
            };
            _brush3 = new TextureBrush(_xor6)
            {
                Transform = Matrix.CreateScale(.25f, .5f, 1f)
            };
            _brush4 = new TextureBrush(_xor6)
            {
                Transform = Matrix.CreateRotationZ((float)Math.PI / 4)
            };
            _brush5 = new TextureBrush(_xor6, .5f);

            _brush6 = new TextureBrush(_xor6)
            {
                Color = Microsoft.Xna.Framework.Color.Purple
            };

            var pattern = new CCTexture2D("images/pattern1");

            var state = new SamplerState();
            state.AddressU = TextureAddressMode.Mirror;
            state.AddressV = TextureAddressMode.Wrap;
            pattern.SamplerState = state;

            mirrorBrush = new TextureBrush(pattern)
            {
                Color = Microsoft.Xna.Framework.Color.White
            };

            var pattern2 = new CCTexture2D("images/pattern2");
            pattern2.SamplerState = SamplerState.LinearWrap;
            repeatBrush = new TextureBrush(pattern2);


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
            drawBatch.FillRectangle(_brush1, new CCRect(50, 50, 200, 100));
            drawBatch.FillRectangle(_brush2, new CCRect(50, 175, 200, 100));
            drawBatch.FillRectangle(_brush3, new CCRect(50, 300, 200, 100));
            drawBatch.FillRectangle(_brush4, new CCRect(50, 425, 200, 100));
            drawBatch.FillCircle(_brush5, new CCVector2(350, 100), 50);
            drawBatch.FillCircle(_brush6, new CCVector2(350, 225), 50);

            drawBatch.FillCircle(mirrorBrush, new CCVector2(350, 600), 50);
            drawBatch.FillRectangle(repeatBrush, new CCRect(50, 550, 200, 100));
        }

    }
}
