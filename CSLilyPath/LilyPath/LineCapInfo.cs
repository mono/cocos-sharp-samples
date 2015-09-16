using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LilyPath.Utility;
using CocosSharp;

namespace LilyPath
{
    internal abstract class LineCapInfo
    {
        protected readonly float _width;
        protected readonly CCVector2[] _xyBuffer;
        protected readonly CCVector2[] _uvBuffer;
        protected readonly short[] _indexBuffer;
        protected readonly short[] _outlineBuffer;

        protected LineCapInfo (float width, int vertexCount, int polyCount)
        {
            _width = width;
            _xyBuffer = new CCVector2[vertexCount];
            _indexBuffer = new short[polyCount * 3];
            _outlineBuffer = new short[vertexCount];
        }

        protected LineCapInfo (float width, CCVector2[] xyBuffer, CCVector2[] uvBuffer, short[] indexBuffer, short[] outlineBuffer)
        {
            _width = width;
            _xyBuffer = xyBuffer;
            _uvBuffer = uvBuffer;
            _indexBuffer = indexBuffer;
            _outlineBuffer = outlineBuffer;
        }

        public int VertexCount
        {
            get { return _xyBuffer.Length; }
        }

        public int IndexCount
        {
            get { return _indexBuffer.Length; }
        }

        public void Calculate (CCVector2 p, CCVector2 edgeAB, PenWorkspace ws, PenAlignment alignment, bool start)
        {
            edgeAB.Normalize();

            // [  eAB.X  -eAB.Y ]
            // [  eAB.Y   eAB.X ]

            float tC = edgeAB.X * _width;
            float tS = edgeAB.Y * _width;

            float tX = p.X;
            float tY = p.Y;

            switch (alignment) {
                case PenAlignment.Center:
                    break;

                case PenAlignment.Inset:
                    if (start) {
                        tX = p.X + (-.5f * tS);
                        tY = p.Y - (-.5f * tC);
                    }
                    else {
                        tX = p.X - (-.5f * tS);
                        tY = p.Y + (-.5f * tC);
                    }
                    break;

                case PenAlignment.Outset:
                    if (start) {
                        tX = p.X + (.5f * tS);
                        tY = p.Y - (.5f * tC);
                    }
                    else {
                        tX = p.X - (.5f * tS);
                        tY = p.Y + (.5f * tC);
                    }
                    break;
            }

            for (int i = 0; i < _xyBuffer.Length; i++)
                ws.XYBuffer[i] = new CCVector2(_xyBuffer[i].X * tC - _xyBuffer[i].Y * tS + tX, _xyBuffer[i].X * tS + _xyBuffer[i].Y * tC + tY);

            for (int i = 0; i < _uvBuffer.Length; i++)
                ws.UVBuffer[i] = _uvBuffer[i];

            for (int i = 0; i < _indexBuffer.Length; i++)
                ws.IndexBuffer[i] = _indexBuffer[i];

            for (int i = 0; i < _outlineBuffer.Length; i++)
                ws.OutlineIndexBuffer[i] = _outlineBuffer[i];

            ws.XYBuffer.Index = _xyBuffer.Length;
            ws.UVBuffer.Index = _uvBuffer.Length;
            ws.IndexBuffer.Index = _indexBuffer.Length;
            ws.OutlineIndexBuffer.Index = _outlineBuffer.Length;
        }
    }

    internal class LineCapFlat : LineCapInfo
    {
        private static readonly CCVector2[] XYBuffer = new CCVector2[] { new CCVector2(0, -.5f), new CCVector2(0, .5f) };
        private static readonly CCVector2[] UVBuffer = new CCVector2[] { new CCVector2(1, 0), new CCVector2(0, 0) };
        private static readonly short[] IndexBuffer = new short[] { };
        private static readonly short[] OutlineBuffer = new short[] { 0, 1 };

        public LineCapFlat (float width)
            : base(width, XYBuffer, UVBuffer, IndexBuffer, OutlineBuffer)
        { }
    }

    internal class LineCapSquare : LineCapInfo
    {
        private static readonly CCVector2[] XYBuffer = new CCVector2[] { new CCVector2(-.5f, -.5f), new CCVector2(-.5f, .5f) };
        private static readonly CCVector2[] UVBuffer = new CCVector2[] { new CCVector2(1, 0), new CCVector2(0, 0) };
        private static readonly short[] IndexBuffer = new short[] { };
        private static readonly short[] OutlineBuffer = new short[] { 0, 1 };

        public LineCapSquare (float width)
            : base(width, XYBuffer, UVBuffer, IndexBuffer, OutlineBuffer)
        { }
    }

    internal class LineCapTriangle : LineCapInfo
    {
        private static readonly CCVector2[] XYBuffer = new CCVector2[] { new CCVector2(0, -.5f), new CCVector2(-.5f, 0), new CCVector2(0, .5f) };
        private static readonly CCVector2[] UVBuffer = new CCVector2[] { new CCVector2(1, 0), new CCVector2(.5f, 0), new CCVector2(0, 0) };
        private static readonly short[] IndexBuffer = new short[] { 2, 1, 0 };
        private static readonly short[] OutlineBuffer = new short[] { 0, 1, 2 };

        public LineCapTriangle (float width)
            : base(width, XYBuffer, UVBuffer, IndexBuffer, OutlineBuffer)
        { }
    }

    internal class LineCapInvTriangle : LineCapInfo
    {
        private static readonly CCVector2[] XYBuffer = new CCVector2[] { new CCVector2(0, -.5f), new CCVector2(-.5f, -.5f), 
            new CCVector2(0, 0), new CCVector2(-.5f, .5f), new CCVector2(0, .5f) };
        private static readonly CCVector2[] UVBuffer = new CCVector2[] { new CCVector2(1, 0), new CCVector2(1, 0),
            new CCVector2(.5f, 0), new CCVector2(0, 0), new CCVector2(0, 0) };
        private static readonly short[] IndexBuffer = new short[] { 4, 3, 2, 2, 1, 0 };
        private static readonly short[] OutlineBuffer = new short[] { 0, 1, 2, 3, 4 };

        public LineCapInvTriangle (float width)
            : base(width, XYBuffer, UVBuffer, IndexBuffer, OutlineBuffer)
        { }
    }

    internal class LineCapArrow : LineCapInfo
    {
        private static readonly CCVector2[] XYBuffer = new CCVector2[] { new CCVector2(1f, -.5f), new CCVector2(1f, -.75f), 
            new CCVector2(-.5f, 0), new CCVector2(1f, .75f), new CCVector2(1f, .5f) };
        private static readonly CCVector2[] UVBuffer = new CCVector2[] { new CCVector2(1, 0), new CCVector2(1, 0),
            new CCVector2(.5f, 0), new CCVector2(0, 0), new CCVector2(0, 0) };
        private static readonly short[] IndexBuffer = new short[] { 0, 2, 1, 4, 2, 0, 3, 2, 4 };
        private static readonly short[] OutlineBuffer = new short[] { 0, 1, 2, 3, 4 };

        public LineCapArrow (float width)
            : base(width, XYBuffer, UVBuffer, IndexBuffer, OutlineBuffer)
        { }
    }
}
