using System;
using CocosSharp;

namespace LilyPath.Utility
{
    internal struct JoinSample
    {
        public CCVector2 PointA;
        public CCVector2 PointB;
        public CCVector2 PointC;

        public float LengthA;
        public float LengthB;
        public float LengthC;

        public JoinSample (CCVector2 pointA, CCVector2 pointB, CCVector2 pointC)
        {
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;

            LengthA = 0;
            LengthB = 0;
            LengthC = 0;
        }

        public JoinSample (CCVector2 pointA, CCVector2 pointB, CCVector2 pointC, float lengthA, float lengthB, float lengthC)
        {
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;

            LengthA = lengthA;
            LengthB = lengthB;
            LengthC = lengthC;
        }

        public void Advance (CCVector2 nextPoint)
        {
            PointA = PointB;
            PointB = PointC;
            PointC = nextPoint;
        }

        public void Advance (CCVector2 nextPoint, float nextLength)
        {
            PointA = PointB;
            PointB = PointC;
            PointC = nextPoint;

            LengthA = LengthB;
            LengthB = LengthC;
            LengthC = nextLength;
        }
    }

    internal class PenWorkspace
    {
        private float _pathLength;
        private float _pathLengthScale;

        public Buffer<CCVector2> XYBuffer;
        public Buffer<CCVector2> XYInsetBuffer;
        public Buffer<CCVector2> XYOutsetBuffer;

        public Buffer<CCVector2> UVBuffer;
        public Buffer<CCVector2> UVInsetBuffer;
        public Buffer<CCVector2> UVOutsetBuffer;

        public Buffer<short> IndexBuffer;
        public Buffer<short> OutlineIndexBuffer;

        public CCVector2[] BoundingQuad;

        public float PathLength
        {
            get { return _pathLength; }
            set
            {
                _pathLength = value;
                _pathLengthScale = (value == 0) ? 1 : 1 / _pathLength;
            }
        }

        public float PathLengthScale
        {
            get { return _pathLengthScale; }
        }

        public PenWorkspace ()
        {
            XYBuffer = new Buffer<CCVector2>();
            XYInsetBuffer = new Buffer<CCVector2>();
            XYOutsetBuffer = new Buffer<CCVector2>();

            UVBuffer = new Buffer<CCVector2>();
            UVInsetBuffer = new Buffer<CCVector2>();
            UVOutsetBuffer = new Buffer<CCVector2>();

            IndexBuffer = new Buffer<short>();
            OutlineIndexBuffer = new Buffer<short>();

            BoundingQuad = new CCVector2[4];
        }

        public PenWorkspace (Pen pen)
        {
            XYBuffer = new Buffer<CCVector2>(Math.Max(pen.StartPointVertexBound(), pen.EndPointVertexBound()));
            XYInsetBuffer = new Buffer<CCVector2>(pen.LineJoinVertexBound());
            XYOutsetBuffer = new Buffer<CCVector2>(XYInsetBuffer.Capacity);

            UVBuffer = new Buffer<CCVector2>(XYBuffer.Capacity);
            UVInsetBuffer = new Buffer<CCVector2>(XYInsetBuffer.Capacity);
            UVOutsetBuffer = new Buffer<CCVector2>(XYOutsetBuffer.Capacity);

            IndexBuffer = new Buffer<short>(Math.Max(pen.StartCapInfo.IndexCount, pen.EndCapInfo.IndexCount));
            OutlineIndexBuffer = new Buffer<short>(XYBuffer.Capacity);

            BoundingQuad = new CCVector2[4];
        }

        public void ResetWorkspace (Pen pen)
        {
            XYBuffer.EnsureCapacity(Math.Max(pen.StartPointVertexBound(), pen.EndPointVertexBound()));
            XYInsetBuffer.EnsureCapacity(pen.LineJoinVertexBound());
            XYOutsetBuffer.EnsureCapacity(XYInsetBuffer.Capacity);

            UVBuffer.EnsureCapacity(XYBuffer.Capacity);
            UVInsetBuffer.EnsureCapacity(XYInsetBuffer.Capacity);
            UVOutsetBuffer.EnsureCapacity(XYOutsetBuffer.Capacity);

            IndexBuffer.EnsureCapacity(Math.Max(pen.StartCapInfo.IndexCount, pen.EndCapInfo.IndexCount));
            OutlineIndexBuffer.EnsureCapacity(XYBuffer.Capacity);

            PathLength = 0;
        }
    }

    public static class GraphicsExtensions
    {

        public static CCColor4B ToCCColor4B(this Microsoft.Xna.Framework.Color color)
        {
            return new CCColor4B(color.R, color.G, color.B, color.A);
        }

        public static CCAffineTransform toCCAffineTransform(this Microsoft.Xna.Framework.Matrix matrix)
        {
            return new CCAffineTransform(matrix.M11, matrix.M12, matrix.M21, matrix.M22,
                                        matrix.M41, matrix.M42, matrix.M43);

        }
    }
}
