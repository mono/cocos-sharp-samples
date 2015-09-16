using System;
using System.Collections.Generic;
using CocosSharp;
using LilyPath;

namespace CSLilyPath
{
    public class TestLayer : CCLayerColor
    {

        protected DrawBatch drawBatch;
        private CCColor4B clearColor = new CCColor4B((byte)255,(byte)245,(byte)245,(byte)245);

        

        public TestLayer()
            : base(CCColor4B.Transparent)
        {
            ClearColor = clearColor;
            drawBatch = new DrawBatch();
            AddChild(drawBatch, 10);
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Draw(drawBatch);
        }

        public CCColor4B ClearColor 
        {
            get { return clearColor; }
            set 
            {
                clearColor = value;
                Color = new CCColor3B(clearColor);
                Opacity = clearColor.A;
            }
        }

        public virtual void Draw(DrawBatch drawBatch)
        {

        }

        protected static List<CCVector2> ShiftPath(List<CCVector2> path, float x, float y)
        {
            for (int i = 0; i < path.Count; i++)
                path[i] = new CCVector2(path[i].X + x, path[i].Y + y);
            return path;
        }

        protected static List<CCVector2> StarPoints(CCVector2 center, int pointCount, float outerRadius, float innerRadius, float rotation, bool close)
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

        protected static CCTexture2D BuildXorTexture(int bits)
        {
            if (bits < 1 || bits > 8)
                throw new ArgumentException("Xor texture must have between 1 and 8 bits", "bits");

            CCTexture2D tex2 = new CCTexture2D(1 << bits, 1 << bits);
            var tex = tex2.XNATexture;

            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[tex.Width * tex.Height];

            for (int y = 0; y < tex.Height; y++)
            {
                for (int x = 0; x < tex.Width; x++)
                {
                    float lum = ((x << (8 - bits)) ^ (y << (8 - bits))) / 255f;
                    data[y * tex.Width + x] = new Microsoft.Xna.Framework.Color(lum, lum, lum);
                }
            }

            tex.SetData(data);

            return tex2;
        }
    }
}

