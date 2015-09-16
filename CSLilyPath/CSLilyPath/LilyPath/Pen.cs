using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LilyPath.Utility;
using CocosSharp;

namespace LilyPath
{
    /// <summary>
    /// A <see cref="Pen"/> that can only have a solid color and width of 1.
    /// </summary>
    public class PrimitivePen : Pen
    {
        /// <summary>
        /// Creates a new <see cref="PrimitivePen"/> with the given color.
        /// </summary>
        /// <param name="color">The pen color.</param>
        public PrimitivePen (Color color)
            : base(color, 1)
        { }
    }

    internal struct InsetOutsetCount
    {
        public readonly short InsetCount;
        public readonly short OutsetCount;
        public readonly bool CCW;

        public InsetOutsetCount (short insetCount, short outsetCount)
        {
            InsetCount = insetCount;
            OutsetCount = outsetCount;
            CCW = true;
        }

        public InsetOutsetCount (short insetCount, short outsetCount, bool ccw)
        {
            InsetCount = insetCount;
            OutsetCount = outsetCount;
            CCW = ccw;
        }
    }

    /// <summary>
    /// Objects used to draw paths.
    /// </summary>
    public class Pen : IDisposable
    {
        #region Default Pens

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Black { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Blue { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Brown { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Cyan { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkBlue { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkCyan { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkGoldenrod { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkGray { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkGreen { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkMagenta { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkOrange { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen DarkRed { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Goldenrod { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Gray { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Green { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen LightBlue { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen LightCyan { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen LightGray { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen LightGreen { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen LightPink { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen LightYellow { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Lime { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Magenta { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Orange { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Pink { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Purple { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Red { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Teal { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen White { get; private set; }

        /// <summary>A system-defined <see cref="Pen"/> object.</summary>
        public static Pen Yellow { get; private set; }

        static Pen ()
        {
            Black = new Pen(Brush.Black);
            Blue = new Pen(Brush.Blue);
            Brown = new Pen(Brush.Brown);
            Cyan = new Pen(Brush.Cyan);
            DarkBlue = new Pen(Brush.DarkBlue);
            DarkCyan = new Pen(Brush.DarkCyan);
            DarkGoldenrod = new Pen(Brush.DarkGoldenrod);
            DarkGray = new Pen(Brush.DarkGray);
            DarkGreen = new Pen(Brush.DarkGreen);
            DarkMagenta = new Pen(Brush.DarkMagenta);
            DarkOrange = new Pen(Brush.DarkOrange);
            DarkRed = new Pen(Brush.DarkRed);
            Goldenrod = new Pen(Brush.Goldenrod);
            Gray = new Pen(Brush.Gray);
            Green = new Pen(Brush.Green);
            LightBlue = new Pen(Brush.LightBlue);
            LightCyan = new Pen(Brush.LightCyan);
            LightGray = new Pen(Brush.LightGray);
            LightGreen = new Pen(Brush.LightGreen);
            LightPink = new Pen(Brush.LightPink);
            LightYellow = new Pen(Brush.LightYellow);
            Lime = new Pen(Brush.Lime);
            Magenta = new Pen(Brush.Magenta);
            Orange = new Pen(Brush.Orange);
            Pink = new Pen(Brush.Pink);
            Purple = new Pen(Brush.Purple);
            Red = new Pen(Brush.Red);
            Teal = new Pen(Brush.Teal);
            White = new Pen(Brush.White);
            Yellow = new Pen(Brush.Yellow);
        }

        #endregion

        private float _joinLimit;
        private float _joinLimitCos2;

        private float _width;
        private LineCap _startCap;
        private LineCap _endCap;

        /// <summary>
        /// Gets the solid color or blending color of the pen.
        /// </summary>
        public Color Color
        {
            get { return Brush.Color; }
        }

        /// <summary>
        /// Gets the <see cref="Brush"/> used to fill stroked paths.
        /// </summary>
        public Brush Brush { get; private set; }

        /// <summary>
        /// Gets or sets the width of the stroked path in graphical units (usually pixels).
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                StartCapInfo = CreateLineCapInfo(StartCap, value);
                EndCapInfo = CreateLineCapInfo(EndCap, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the stroked path relative to the ideal path being stroked.
        /// </summary>
        public PenAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets how the start of a stroked path is terminated.
        /// </summary>
        public LineCap StartCap
        {
            get { return _startCap; }
            set
            {
                _startCap = value;
                StartCapInfo = CreateLineCapInfo(value, Width);
            }
        }

        /// <summary>
        /// Gets or sets how the end of a stroked path is terminated.
        /// </summary>
        public LineCap EndCap
        {
            get { return _endCap; }
            set
            {
                _endCap = value;
                EndCapInfo = CreateLineCapInfo(value, Width);
            }
        }

        /// <summary>
        /// Gets or sets how the segments in the path are joined together.
        /// </summary>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the limit of the thickness of the join on a mitered corner.
        /// </summary>
        /// <remarks><para>The miter length is the distance from the intersection of the line walls on the inside of the join to the intersection of the line walls outside of the join. The miter length can be large when the angle between two lines is small. The miter limit is the maximum allowed ratio of miter length to stroke width. The default value is 10.0f.</para>
        /// <para>If the miter length of the join of the intersection exceeds the limit of the join, then the join will be beveled to keep it within the limit of the join of the intersection.</para></remarks>
        public float MiterLimit { get; set; }

        /// <summary>
        /// Gets or sets the angle difference threshold in radians under which joins will be mitered instead of beveled or rounded.  
        /// Defaults to PI / 8 (11.25 degrees).
        /// </summary>
        public float JoinLimit
        {
            get { return _joinLimit; }
            set
            {
                _joinLimit = value;
                _joinLimitCos2 = (float)Math.Cos(_joinLimit);
                _joinLimitCos2 *= _joinLimitCos2;
            }
        }

        /// <summary>
        /// Gets or sets whether this pen "owns" the brush used to construct it, and should therefor dispose the brush
        /// along with itself.
        /// </summary>
        public bool OwnsBrush { get; set; }

        /// <summary>
        /// Gets whether this pen needs path length values to properly calculate values at each sample point on the path.
        /// </summary>
        public virtual bool NeedsPathLength
        {
            get { return false; }
        }

        internal LineCapInfo StartCapInfo { get; private set; }
        internal LineCapInfo EndCapInfo { get; private set; }

        private Pen ()
        {
            //Color = Color.White;
            Alignment = PenAlignment.Center;
            MiterLimit = 10f;
            JoinLimit = (float)(Math.PI / 8);

            _startCap = LineCap.Flat;
            _endCap = LineCap.Flat;
        }

        /// <summary>
        /// Creates a new <see cref="Pen"/> with the given brush and width.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/> used to stroke the pen.</param>
        /// <param name="width">The width of the paths drawn by the pen.</param>
        /// <param name="ownsBrush"><c>true</c> if the pen should be responsible for disposing the <see cref="Brush"/>, <c>false</c> otherwise.</param>
        public Pen (Brush brush, float width, bool ownsBrush)
            : this()
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            _width = width;

            Brush = brush;
            OwnsBrush = ownsBrush;

            StartCapInfo = CreateLineCapInfo(StartCap, Width);
            EndCapInfo = CreateLineCapInfo(EndCap, Width);
        }

        /// <summary>
        /// Creates a new <see cref="Pen"/> with the given brush and width.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/> used to stroke the pen.</param>
        /// <param name="width">The width of the paths drawn by the pen.</param>
        /// <remarks>By default, the pen will not take resposibility for disposing the <see cref="Brush"/>.</remarks>
        public Pen (Brush brush, float width)
            : this(brush, width, false)
        { }

        /// <summary>
        /// Creates a new <see cref="Pen"/> with the given color and width.
        /// </summary>
        /// <param name="color">The color used to stroke the pen.</param>
        /// <param name="width">The width of the paths drawn by the pen.</param>
        public Pen (Color color, float width)
            : this(new SolidColorBrush(color), width, true)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Pen"/> with the given brush and a width of 1.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/> used to stroke the pen.</param>
        /// <remarks>By default, the pen will not take resposibility for disposing the <see cref="Brush"/>.</remarks>
        public Pen (Brush brush)
            : this(brush, 1, false)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Pen"/> with the given brush and a width of 1.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/> used to stroke the pen.</param>
        /// <param name="ownsBrush"><c>true</c> if the pen should be responsible for disposing the <see cref="Brush"/>, <c>false</c> otherwise.</param>
        public Pen (Brush brush, bool ownsBrush)
            : this(brush, 1, ownsBrush)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Pen"/> with the given color and a width of 1.
        /// </summary>
        /// <param name="color">The color used to stroke the pen.</param>
        public Pen (Color color)
            : this(color, 1)
        {
        }

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Releases all resources used by the <see cref="Pen"/> object.
        /// </summary>
        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose (bool disposing)
        {
            if (!_disposed) {
                if (disposing) {
                    if (OwnsBrush && Brush != null)
                        Brush.Dispose();
                    DisposeManaged();
                }
                DisposeUnmanaged();
                _disposed = true;
            }
        }

        /// <summary>
        /// Attempts to dispose unmanaged resources.
        /// </summary>
        ~Pen ()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="Pen"/>.
        /// </summary>
        protected virtual void DisposeManaged () { }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Pen"/>.
        /// </summary>
        protected virtual void DisposeUnmanaged () { }

        #endregion

        /// <summary>
        /// Queries the <see cref="Pen"/> for its color at a coordinate relative to the stroke width of the pen and length of the path.
        /// </summary>
        /// <param name="widthPosition">A value between 0 and 1 interpolated across the stroke width.</param>
        /// <param name="lengthPosition">A value between 0 and the full length of the path.</param>
        /// <param name="length">A scaling factor such that lengthPosition can be normalized to a value between 0 and 1.</param>
        /// <returns>A color value.</returns>
        protected internal virtual Color ColorAt (float widthPosition, float lengthPosition, float length)
        {
            return Brush.Color;
        }

        private LineCapInfo CreateLineCapInfo (LineCap lineCapType, float width)
        {
            switch (lineCapType) {
                case LineCap.Flat:
                    return new LineCapFlat(width);
                case LineCap.Square:
                    return new LineCapSquare(width);
                case LineCap.Triangle:
                    return new LineCapTriangle(width);
                case LineCap.InvTriangle:
                    return new LineCapInvTriangle(width);
                case LineCap.Arrow:
                    return new LineCapArrow(width);
            }

            return new LineCapFlat(width);
        }

        internal Color ColorAt(CCVector2 uv, float lengthScale)
        {
            return ColorAt(uv.X, uv.Y, lengthScale);
        }

        internal int StartPointVertexBound ()
        {
            return StartCapInfo.VertexCount;
        }

        internal int EndPointVertexBound ()
        {
            return EndCapInfo.VertexCount;
        }

        internal int LineJoinVertexBound ()
        {
            switch (LineJoin) {
                case LineJoin.Miter:
                    return 3;
                case LineJoin.Bevel:
                    return 3;
            }

            return 0;
        }

        internal int MaximumVertexCount (int pointCount)
        {
            int expected = StartCapInfo.VertexCount + EndCapInfo.VertexCount;

            int joinCount = Math.Max(0, pointCount - 1);
            switch (LineJoin) {
                case LineJoin.Bevel:
                    expected += joinCount * 3;
                    break;

                case LineJoin.Miter:
                    expected += joinCount * 3;
                    break;

                //case LineJoin.Round:
                //    expected += (int)Math.Ceiling(joinCount * (Width / 6f + 2));
                //    break;
            }

            return expected;
        }

        internal int MaximumIndexCount (int pointCount)
        {
            int extra = StartCapInfo.IndexCount + EndCapInfo.IndexCount;

            int joinCount = Math.Max(0, pointCount - 1);
            switch (LineJoin) {
                case LineJoin.Bevel:
                    extra += joinCount;
                    break;

                case LineJoin.Miter:
                    extra += joinCount;
                    break;

                //case LineJoin.Round:
                //    extra += (int)Math.Ceiling(joinCount * (Width / 6f));
                //    break;
            }

            return extra * 3 + (pointCount - 1) * 6;
        }

        //internal InsetOutsetCount ComputeMiter (CCVector2 a, CCVector2 b, CCVector2 c, PenWorkspace ws)
        internal InsetOutsetCount ComputeMiter (ref JoinSample js, PenWorkspace ws)
        {
            CCVector2 a = js.PointA;
            CCVector2 b = js.PointB;
            CCVector2 c = js.PointC;

            CCVector2 edgeAB = new CCVector2(b.X - a.X, b.Y - a.Y);
            edgeAB.Normalize();
            CCVector2 edgeABt = new CCVector2(-edgeAB.Y, edgeAB.X);

            CCVector2 edgeBC = new CCVector2(c.X - b.X, c.Y - b.Y);
            edgeBC.Normalize();
            CCVector2 edgeBCt = new CCVector2(-edgeBC.Y, edgeBC.X);

            CCVector2 point1, point2, point3, point4;

            switch (Alignment) {
                case PenAlignment.Center:
                    float w2 = Width / 2;

                    point2 = new CCVector2(a.X + w2 * edgeABt.X, a.Y + w2 * edgeABt.Y);
                    point4 = new CCVector2(a.X - w2 * edgeABt.X, a.Y - w2 * edgeABt.Y);

                    point1 = new CCVector2(c.X + w2 * edgeBCt.X, c.Y + w2 * edgeBCt.Y);
                    point3 = new CCVector2(c.X - w2 * edgeBCt.X, c.Y - w2 * edgeBCt.Y);
                    break;

                case PenAlignment.Inset:
                    point2 = new CCVector2(a.X + Width * edgeABt.X, a.Y + Width * edgeABt.Y);
                    point4 = a;

                    point1 = new CCVector2(c.X + Width * edgeBCt.X, c.Y + Width * edgeBCt.Y);
                    point3 = c;
                    break;

                case PenAlignment.Outset:
                    point2 = a;
                    point4 = new CCVector2(a.X - Width * edgeABt.X, a.Y - Width * edgeABt.Y);

                    point1 = c;
                    point3 = new CCVector2(c.X - Width * edgeBCt.X, c.Y - Width * edgeBCt.Y);
                    break;

                default:
                    point2 = CCVector2.Zero;
                    point4 = CCVector2.Zero;

                    point1 = CCVector2.Zero;
                    point3 = CCVector2.Zero;
                    break;
            }

            CCVector2 point0, point5;

            float tdiv = CCVector2.Dot(edgeBCt, edgeAB);

            if (Math.Abs(tdiv) < .0005f) {
                point0 = new CCVector2((point2.X + point1.X) / 2, (point2.Y + point1.Y) / 2);
                point5 = new CCVector2((point4.X + point3.X) / 2, (point4.Y + point3.Y) / 2);
            }
            else {
                float offset01 = CCVector2.Dot(edgeBCt, point1);
                float t0 = (offset01 - CCVector2.Dot(edgeBCt, point2)) / tdiv;

                float offset35 = CCVector2.Dot(edgeBCt, point3);
                float t5 = (offset35 - CCVector2.Dot(edgeBCt, point4)) / tdiv;

                point0 = new CCVector2(point2.X + t0 * edgeAB.X, point2.Y + t0 * edgeAB.Y);
                point5 = new CCVector2(point4.X + t5 * edgeAB.X, point4.Y + t5 * edgeAB.Y);
            }

            double miterLimit = MiterLimit * Width;
            if ((point0 - point5).LengthSquared() > miterLimit * miterLimit)
                return ComputeBevel(ref js, ws);

            ws.XYInsetBuffer[0] = point0;
            ws.XYOutsetBuffer[0] = point5;

            ws.UVInsetBuffer[0] = new CCVector2(0, js.LengthB);
            ws.UVOutsetBuffer[0] = new CCVector2(1, js.LengthB);

            return new InsetOutsetCount(1, 1);
        }

        //internal InsetOutsetCount ComputeBevel (CCVector2 a, CCVector2 b, CCVector2 c, PenWorkspace ws)
        internal InsetOutsetCount ComputeBevel (ref JoinSample js, PenWorkspace ws)
        {
            CCVector2 a = js.PointA;
            CCVector2 b = js.PointB;
            CCVector2 c = js.PointC;

            CCVector2 edgeBA = new CCVector2(a.X - b.X, a.Y - b.Y);
            CCVector2 edgeBC = new CCVector2(c.X - b.X, c.Y - b.Y);
            double dot = CCVector2.Dot(edgeBA, edgeBC);
            if (dot < 0) {
                double den = edgeBA.LengthSquared() * edgeBC.LengthSquared();
                double cos2 = (dot * dot) / den;

                if (cos2 > _joinLimitCos2)
                    return ComputeMiter(ref js, ws);
            }

            CCVector2 edgeAB = new CCVector2(b.X - a.X, b.Y - a.Y);
            edgeAB.Normalize();
            CCVector2 edgeABt = new CCVector2(-edgeAB.Y, edgeAB.X);

            edgeBC.Normalize();
            CCVector2 edgeBCt = new CCVector2(-edgeBC.Y, edgeBC.X);

            CCVector2 pointA = a;
            CCVector2 pointC = c;

            short vertexCount = 0;

            if (Cross2D(edgeAB, edgeBC) > 0) {
                switch (Alignment) {
                    case PenAlignment.Center:
                        float w2 = Width / 2;
                        pointA = new CCVector2(a.X - w2 * edgeABt.X, a.Y - w2 * edgeABt.Y);
                        pointC = new CCVector2(c.X - w2 * edgeBCt.X, c.Y - w2 * edgeBCt.Y);

                        ws.XYInsetBuffer[0] = new CCVector2(b.X + w2 * edgeABt.X, b.Y + w2 * edgeABt.Y);
                        ws.XYInsetBuffer[1] = new CCVector2(b.X + w2 * edgeBCt.X, b.Y + w2 * edgeBCt.Y);

                        vertexCount = 2;
                        break;

                    case PenAlignment.Inset:
                        ws.XYInsetBuffer[0] = new CCVector2(b.X + Width * edgeABt.X, b.Y + Width * edgeABt.Y);
                        ws.XYInsetBuffer[1] = new CCVector2(b.X + Width * edgeBCt.X, b.Y + Width * edgeBCt.Y);

                        vertexCount = 2;
                        break;

                    case PenAlignment.Outset:
                        pointA = new CCVector2(a.X - Width * edgeABt.X, a.Y - Width * edgeABt.Y);
                        pointC = new CCVector2(c.X - Width * edgeBCt.X, c.Y - Width * edgeBCt.Y);

                        ws.XYInsetBuffer[0] = b;

                        vertexCount = 1;
                        break;
                }

                CCVector2 point5;

                float tdiv = CCVector2.Dot(edgeBCt, edgeAB);
                if (Math.Abs(tdiv) < 0.0005f) {
                    point5 = new CCVector2((pointA.X + pointC.X) / 2, (pointA.Y + pointC.Y) / 2);
                }
                else {
                    float offset35 = CCVector2.Dot(edgeBCt, pointC);
                    float t5 = (offset35 - CCVector2.Dot(edgeBCt, pointA)) / tdiv;

                    point5 = new CCVector2(pointA.X + t5 * edgeAB.X, pointA.Y + t5 * edgeAB.Y);
                }

                ws.XYOutsetBuffer[0] = point5;

                ws.UVOutsetBuffer[0] = new CCVector2(1, js.LengthB);
                for (int i = 0; i < vertexCount; i++)
                    ws.UVInsetBuffer[i] = new CCVector2(0, js.LengthB);

                return new InsetOutsetCount(vertexCount, 1, false);
            }
            else {
                switch (Alignment) {
                    case PenAlignment.Center:
                        float w2 = Width / 2;
                        pointA = new CCVector2(a.X + w2 * edgeABt.X, a.Y + w2 * edgeABt.Y);
                        pointC = new CCVector2(c.X + w2 * edgeBCt.X, c.Y + w2 * edgeBCt.Y);

                        ws.XYOutsetBuffer[0] = new CCVector2(b.X - w2 * edgeABt.X, b.Y - w2 * edgeABt.Y);
                        ws.XYOutsetBuffer[1] = new CCVector2(b.X - w2 * edgeBCt.X, b.Y - w2 * edgeBCt.Y);

                        vertexCount = 2;
                        break;

                    case PenAlignment.Inset:
                        pointA = new CCVector2(a.X + Width * edgeABt.X, a.Y + Width * edgeABt.Y);
                        pointC = new CCVector2(c.X + Width * edgeBCt.X, c.Y + Width * edgeBCt.Y);

                        ws.XYOutsetBuffer[0] = b;

                        vertexCount = 1;
                        break;

                    case PenAlignment.Outset:
                        ws.XYOutsetBuffer[0] = new CCVector2(b.X - Width * edgeABt.X, b.Y - Width * edgeABt.Y);
                        ws.XYOutsetBuffer[1] = new CCVector2(b.X - Width * edgeBCt.X, b.Y - Width * edgeBCt.Y);

                        vertexCount = 2;
                        break;
                }

                CCVector2 point0;

                float tdiv = CCVector2.Dot(edgeBCt, edgeAB);
                if (Math.Abs(tdiv) < 0.0005f) {
                    point0 = new CCVector2((pointA.X + pointC.X) / 2, (pointA.Y + pointC.Y) / 2);
                }
                else {
                    float offset01 = CCVector2.Dot(edgeBCt, pointC);
                    float t0 = (offset01 - CCVector2.Dot(edgeBCt, pointA)) / tdiv;

                    point0 = new CCVector2(pointA.X + t0 * edgeAB.X, pointA.Y + t0 * edgeAB.Y);
                }

                ws.XYInsetBuffer[0] = point0;

                ws.UVInsetBuffer[0] = new CCVector2(0, js.LengthB);
                for (int i = 0; i < vertexCount; i++)
                    ws.UVOutsetBuffer[i] = new CCVector2(1, js.LengthB);

                return new InsetOutsetCount(1, vertexCount, true);
            }
        }

        private float Cross2D (CCVector2 u, CCVector2 v)
        {
            return (u.Y * v.X) - (u.X * v.Y);
        }

        private bool TriangleIsCCW (CCVector2 a, CCVector2 b, CCVector2 c)
        {
            return Cross2D(b - a, c - b) < 0;
        }

        internal void ComputeStartPoint (CCVector2 a, CCVector2 b, PenWorkspace ws)
        {
            StartCapInfo.Calculate(a, b - a, ws, Alignment, true);
        }

        internal void ComputeEndPoint (CCVector2 a, CCVector2 b, PenWorkspace ws)
        {
            EndCapInfo.Calculate(b, a - b, ws, Alignment, false);

            for (int i = 0; i < ws.UVBuffer.Index; i++)
                ws.UVBuffer[i] = new CCVector2(1 - ws.UVBuffer[i].X, ws.PathLength);
        }
    }
}
