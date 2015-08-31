using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CocosSharp;
using Microsoft.Xna.Framework;

namespace DrawNodeBuffer
{
    public class DrawNodeBuffer
    {
        /// <summary>
        /// Style of the end caps for a line.
        /// 
        /// Butt - Default.  Flat edge
        /// Round - A round cap is added to ends of each line
        /// Square - A square cap is added to the ends of each line
        /// </summary>
        public enum LineCap
        {
            Butt,
            Round,
            Square,
        }

        const int DefaultBufferSize = 512;
        public CCRawList<CCV3F_C4B> TriangleVertices { get; private set; }
        public CCRawList<CCV3F_C4B> LineVertices { get; private set; }

        bool dirty;

        public CCColor3B Color { get; set; }
        public byte Opacity { get; set; }

        public CCSize ContentSize { get; private set; }

        struct ExtrudeVerts
        {
            public CCPoint offset;
            public CCPoint n;
        }

        public DrawNodeBuffer()
        {
            TriangleVertices = new CCRawList<CCV3F_C4B>(DefaultBufferSize);
            LineVertices = new CCRawList<CCV3F_C4B>(DefaultBufferSize);

            Color = CCColor3B.White;
            Opacity = 255;
        }

        public void AddTriangleVertex(CCV3F_C4B triangleVertex)
        {
            TriangleVertices.Add(triangleVertex);
            dirty = true;
        }

        public void AddLineVertex(CCV3F_C4B lineVertex)
        {
            LineVertices.Add(lineVertex);
            dirty = true;
        }

        public void DrawCircle(CCPoint center, float radius, int segments, CCColor4B color)
        {
            var cl = color;

            float theta = MathHelper.Pi * 2.0f / segments;
            float tangetial_factor = (float)Math.Tan(theta);   //calculate the tangential factor 

            float radial_factor = (float)Math.Cos(theta);   //calculate the radial factor 

            float x = radius;  //we start at angle = 0 
            float y = 0;

            var vert1 = new CCV3F_C4B(CCVertex3F.Zero, cl);
            float tx = 0;
            float ty = 0;

            for (int i = 0; i < segments; i++)
            {

                vert1.Vertices.X = x + center.X;
                vert1.Vertices.Y = y + center.Y;
                AddLineVertex(vert1); // output vertex

                //calculate the tangential vector 
                //remember, the radial vector is (x, y) 
                //to get the tangential vector we flip those coordinates and negate one of them 
                tx = -y;
                ty = x;

                //add the tangential vector 
                x += tx * tangetial_factor;
                y += ty * tangetial_factor;

                //correct using the radial factor 
                x *= radial_factor;
                y *= radial_factor;

                vert1.Vertices.X = x + center.X;
                vert1.Vertices.Y = y + center.Y;
                AddLineVertex(vert1); // output vertex

            }

            dirty = true;

        }

        // See http://slabode.exofire.net/circle_draw.shtml
        // An Efficient Way to Draw Approximate Circles in OpenGL
        // Try to keep from calculating Cos and Sin of values everytime and just use
        // add and subtract where possible to calculate the values.
        public void DrawCircle(CCPoint center, float radius, CCColor4B color)
        {
            int segments = (int)(10 * (float)Math.Sqrt(radius));  //<- Let's try to guess at # segments for a reasonable smoothness
            DrawCircle(center, radius, segments, color);
        }

        // See http://slabode.exofire.net/circle_draw.shtml
        // An Efficient Way to Draw Approximate Circles in OpenGL
        // Try to keep from calculating Cos and Sin of values everytime and just use
        // add and subtract where possible to calculate the values.
        public void DrawSolidCircle(CCPoint pos, float radius, CCColor4B color)
        {
            var cl = color;

            int segments = (int)(10 * (float)Math.Sqrt(radius));  //<- Let's try to guess at # segments for a reasonable smoothness

            float theta = MathHelper.Pi * 2.0f / segments;
            float tangetial_factor = (float)Math.Tan(theta);   //calculate the tangential factor 

            float radial_factor = (float)Math.Cos(theta);   //calculate the radial factor 

            float x = radius;  //we start at angle = 0 
            float y = 0;

            var verticeCenter = new CCV3F_C4B(pos, cl);
            var vert1 = new CCV3F_C4B(CCVertex3F.Zero, cl);
            float tx = 0;
            float ty = 0;

            for (int i = 0; i < segments; i++)
            {
                AddTriangleVertex(verticeCenter);

                vert1.Vertices.X = x + pos.X;
                vert1.Vertices.Y = y + pos.Y;
                AddTriangleVertex(vert1); // output vertex

                //calculate the tangential vector 
                //remember, the radial vector is (x, y) 
                //to get the tangential vector we flip those coordinates and negate one of them 
                tx = -y;
                ty = x;

                //add the tangential vector 
                x += tx * tangetial_factor;
                y += ty * tangetial_factor;

                //correct using the radial factor 
                x *= radial_factor;
                y *= radial_factor;

                vert1.Vertices.X = x + pos.X;
                vert1.Vertices.Y = y + pos.Y;
                AddTriangleVertex(vert1); // output vertex
            }

            dirty = true;
        }


        // Used for drawing line caps
        public void DrawSolidArc(CCPoint pos, float radius, float startAngle, float sweepAngle, CCColor4B color)
        {
            var cl = color;

            int segments = (int)(10 * (float)Math.Sqrt(radius));  //<- Let's try to guess at # segments for a reasonable smoothness

            float theta = -sweepAngle / (segments - 1);// MathHelper.Pi * 2.0f / segments;
            float tangetial_factor = (float)Math.Tan(theta);   //calculate the tangential factor 

            float radial_factor = (float)Math.Cos(theta);   //calculate the radial factor 

            float x = radius * (float)Math.Cos(-startAngle);   //we now start at the start angle
            float y = radius * (float)Math.Sin(-startAngle);

            var verticeCenter = new CCV3F_C4B(pos, cl);
            var vert1 = new CCV3F_C4B(CCVertex3F.Zero, cl);
            float tx = 0;
            float ty = 0;

            for (int i = 0; i < segments - 1; i++)
            {
                AddTriangleVertex(verticeCenter);

                vert1.Vertices.X = x + pos.X;
                vert1.Vertices.Y = y + pos.Y;
                AddTriangleVertex(vert1); // output vertex

                //calculate the tangential vector 
                //remember, the radial vector is (x, y) 
                //to get the tangential vector we flip those coordinates and negate one of them 
                tx = -y;
                ty = x;

                //add the tangential vector 
                x += tx * tangetial_factor;
                y += ty * tangetial_factor;

                //correct using the radial factor 
                x *= radial_factor;
                y *= radial_factor;

                vert1.Vertices.X = x + pos.X;
                vert1.Vertices.Y = y + pos.Y;
                AddTriangleVertex(vert1); // output vertex
            }

            dirty = true;
        }

        public void DrawLine(CCPoint from, CCPoint to, float lineWidth = 1, LineCap lineCap = LineCap.Butt)
        {
            DrawLine(from, to, lineWidth, new CCColor4B(Color.R, Color.G, Color.B, Opacity));
        }

        public void DrawLine(CCPoint from, CCPoint to, CCColor4B color, LineCap lineCap = LineCap.Butt)
        {
            DrawLine(from, to, 1, color);
        }

        public void DrawLine(CCPoint from, CCPoint to, float lineWidth, CCColor4B color, LineCap lineCap = LineCap.Butt)
        {
            System.Diagnostics.Debug.Assert(lineWidth >= 0, "Invalid value specified for lineWidth : value is negative");
            if (lineWidth <= 0)
                return;

            var cl = color;

            var a = from;
            var b = to;

            var normal = CCPoint.Normalize(a - b);
            if (lineCap == LineCap.Square)
            {
                var nr = normal * lineWidth;
                a += nr;
                b -= nr;
            }

            var n = CCPoint.PerpendicularCCW(normal);

            var nw = n * lineWidth;
            var v0 = b - nw;
            var v1 = b + nw;
            var v2 = a - nw;
            var v3 = a + nw;

            // Triangles from beginning to end
            AddTriangleVertex(new CCV3F_C4B(v1, cl));
            AddTriangleVertex(new CCV3F_C4B(v2, cl));
            AddTriangleVertex(new CCV3F_C4B(v0, cl));

            AddTriangleVertex(new CCV3F_C4B(v1, cl));
            AddTriangleVertex(new CCV3F_C4B(v2, cl));
            AddTriangleVertex(new CCV3F_C4B(v3, cl));

            if (lineCap == LineCap.Round)
            {
                var mb = (float)Math.Atan2(v1.Y - b.Y, v1.X - b.X);
                var ma = (float)Math.Atan2(v2.Y - a.Y, v2.X - a.X);

                // Draw rounded line caps
                DrawSolidArc(a, lineWidth, -ma, -MathHelper.Pi, color);
                DrawSolidArc(b, lineWidth, -mb, -MathHelper.Pi, color);
            }

            dirty = true;
        }

        public void DrawRect(CCPoint p, float size)
        {

            DrawRect(p, size, new CCColor4B(Color.R, Color.G, Color.B, Opacity));
        }

        public void DrawRect(CCPoint p, float size, CCColor4B color)
        {

            var rect = CCRect.Zero;

            float hs = size / 2.0f;

            rect.Origin = p + new CCPoint(-hs, -hs);
            rect.Size = new CCSize(size, size);

            DrawRect(rect, color);
        }

        public void DrawRect(CCRect rect)
        {
            DrawRect(rect, new CCColor4B(Color.R, Color.G, Color.B, Opacity));
        }

        public void DrawRect(CCRect rect, CCColor4B fillColor)
        {
            DrawRect(rect, fillColor, 0, CCColor4B.Transparent);
        }

        public void DrawRect(CCRect rect, CCColor4B fillColor, float borderWidth,
            CCColor4B borderColor)
        {
            float x1 = rect.MinX;
            float y1 = rect.MinY;
            float x2 = rect.MaxX;
            float y2 = rect.MaxY;
            CCPoint[] pt = new CCPoint[] { 
                new CCPoint(x1,y1), new CCPoint(x2,y1), new CCPoint(x2,y2), new CCPoint(x1,y2)
            };
            CCColor4F cf = new CCColor4F(fillColor.R / 255f, fillColor.G / 255f, fillColor.B / 255f, fillColor.A / 255f);
            CCColor4F bc = new CCColor4F(borderColor.R / 255f, borderColor.G / 255f, borderColor.B / 255f, borderColor.A / 255f);
            DrawPolygon(pt, 4, cf, borderWidth, bc);
        }

        public void DrawEllipse(CCRect rect, float lineWidth, CCColor4B color)
        {
            DrawEllipticalArc(rect, 0, 360, false, lineWidth, color);
            dirty = true;
        }

        public void DrawEllipse(int x, int y, int width, int height, float lineWidth, CCColor4B color)
        {
            DrawEllipticalArc(x, y, width, height, 0, 360, false, lineWidth, color);
            dirty = true;
        }

        internal void DrawEllipticalArc(CCRect arcRect, double lambda1, double lambda2,
            bool isPieSlice, float lineWidth, CCColor4B color)
        {
            make_arcs(
                arcRect.Origin.X, arcRect.Origin.Y, arcRect.Size.Width, arcRect.Size.Height,
                (float)lambda1, (float)lambda2,
                false, true, isPieSlice, lineWidth, color);
        }

        internal void DrawEllipticalArc(float x, float y, float width, float height, double lambda1, double lambda2,
            bool isPieSlice, float lineWidth, CCColor4B color)
        {
            make_arcs(
                x, y, width, height,
                (float)lambda1, (float)lambda2,
                false, true, isPieSlice, lineWidth, color);
        }

        public void DrawCatmullRom(List<CCPoint> points, int segments)
        {
            DrawCardinalSpline(points, 0.5f, segments);
        }

        public void DrawCatmullRom(List<CCPoint> points, int segments, CCColor4B color)
        {
            DrawCardinalSpline(points, 0.5f, segments, color);
        }

        public void DrawCardinalSpline(List<CCPoint> config, float tension, int segments)
        {
            DrawCardinalSpline(config, tension, segments, new CCColor4B(Color.R, Color.G, Color.B, Opacity));
        }

        public void DrawCardinalSpline(List<CCPoint> config, float tension, int segments, CCColor4B color)
        {

            int p;
            float lt;
            float deltaT = 1.0f / config.Count;

            int count = config.Count;

            var vertices = new CCPoint[segments + 1];

            for (int i = 0; i < segments + 1; i++)
            {
                float dt = (float)i / segments;

                // border
                if (dt == 1)
                {
                    p = count - 1;
                    lt = 1;
                }
                else
                {
                    p = (int)(dt / deltaT);
                    lt = (dt - deltaT * p) / deltaT;
                }

                // Interpolate    
                int c = config.Count - 1;
                CCPoint pp0 = config[Math.Min(c, Math.Max(p - 1, 0))];
                CCPoint pp1 = config[Math.Min(c, Math.Max(p + 0, 0))];
                CCPoint pp2 = config[Math.Min(c, Math.Max(p + 1, 0))];
                CCPoint pp3 = config[Math.Min(c, Math.Max(p + 2, 0))];

                vertices[i] = SplineMath.CCCardinalSplineAt(pp0, pp1, pp2, pp3, tension, lt);
            }

            DrawPolygon(vertices, vertices.Length, CCColor4B.Transparent, 1, color, false);
        }



        /// <summary>
        /// draws a cubic bezier path
        /// @since v0.8
        /// </summary>
        public void DrawCubicBezier(CCPoint origin, CCPoint control1, CCPoint control2, CCPoint destination, int segments, float lineWidth, CCColor4B color)
        {

            float t = 0;
            float increment = 1.0f / segments;

            var vertices = new CCPoint[segments];

            vertices[0] = origin;

            for (int i = 1; i < segments; ++i, t += increment)
            {
                vertices[i].X = SplineMath.CubicBezier(origin.X, control1.X, control2.X, destination.X, t);
                vertices[i].Y = SplineMath.CubicBezier(origin.Y, control1.Y, control2.Y, destination.Y, t);
            }

            vertices[segments - 1] = destination;

            for (int i = 0; i < vertices.Length - 1; i++)
            {
                DrawLine(vertices[i], vertices[i + 1], lineWidth, color, LineCap.Square);
            }

            //DrawPolygon(vertices, vertices.Length, color, lineWidth, color);
        }

        public void DrawQuadBezier(CCPoint origin, CCPoint control, CCPoint destination, int segments, float lineWidth, CCColor4B color)
        {
            float t = 0;
            float increment = 1.0f / segments;

            var vertices = new CCPoint[segments];

            vertices[0] = origin;

            for (int i = 1; i < segments; ++i, t += increment)
            {
                vertices[i].X = SplineMath.QuadBezier(origin.X, control.X, destination.X, t);
                vertices[i].Y = SplineMath.QuadBezier(origin.Y, control.Y, destination.Y, t);
            }

            vertices[segments - 1] = destination;

            DrawPolygon(vertices, vertices.Length, CCColor4B.Transparent, lineWidth, color, false);
        }

        public void DrawTriangleList(CCV3F_C4B[] verts)
        {

            for (int x = 0; x < verts.Length; x++)
            {
                AddTriangleVertex(verts[x]);
            }
        }

        public void DrawLineList(CCV3F_C4B[] verts)
        {

            for (int x = 0; x < verts.Length; x++)
            {
                AddLineVertex(verts[x]);

            }
        }

        public void DrawPolygon(CCPoint[] verts, int count, CCColor4B fillColor, float borderWidth,
                                CCColor4B borderColor, bool closePolygon = true)
        {

            var polycount = count;

            var colorFill = fillColor;
            var borderFill = borderColor;

            bool outline = (borderColor.A > 0.0f && borderWidth > 0.0f);
            bool fill = fillColor.A > 0.0f;

            var numberOfTriangles = outline ? (3 * polycount - 2) : (polycount - 2);

            if (numberOfTriangles > 0 && fill && closePolygon)
            {
                for (int i = 1; i < polycount - 1; i++)
                {
                    AddTriangleVertex(new CCV3F_C4B(verts[0], colorFill));
                    AddTriangleVertex(new CCV3F_C4B(verts[i], colorFill));
                    AddTriangleVertex(new CCV3F_C4B(verts[i + 1], colorFill));
                }
            }
            else
            {
                for (int i = 0; i < polycount - 1; i++)
                {
                    DrawLine(verts[i], verts[i + 1], borderWidth, colorFill);
                }

            }

            if (outline)
            {
                var extrude = new ExtrudeVerts[polycount];

                for (int i = 0; i < polycount; i++)
                {
                    var v0 = verts[(i - 1 + polycount) % polycount];
                    var v1 = verts[i];
                    var v2 = verts[(i + 1) % polycount];

                    var n1 = CCPoint.Normalize(CCPoint.PerpendicularCCW(v1 - v0));
                    var n2 = CCPoint.Normalize(CCPoint.PerpendicularCCW(v2 - v1));

                    var offset = (n1 + n2) * (1.0f / (CCPoint.Dot(n1, n2) + 1.0f));
                    extrude[i] = new ExtrudeVerts() { offset = offset, n = n2 };
                }

                float inset = (!outline ? 0.5f : 0.0f);

                for (int i = 0; i < polycount - 2; i++)
                {
                    var v0 = verts[0] - (extrude[0].offset * inset);
                    var v1 = verts[i + 1] - (extrude[i + 1].offset * inset);
                    var v2 = verts[i + 2] - (extrude[i + 2].offset * inset);

                    AddTriangleVertex(new CCV3F_C4B(v0, colorFill)); //__t(v2fzero)
                    AddTriangleVertex(new CCV3F_C4B(v1, colorFill)); //__t(v2fzero)
                    AddTriangleVertex(new CCV3F_C4B(v2, colorFill)); //__t(v2fzero)
                }

                for (int i = 0; i < polycount - 1; i++)
                {
                    int j = (i + 1) % polycount;
                    var v0 = verts[i];
                    var v1 = verts[j];

                    var offset0 = extrude[i].offset;
                    var offset1 = extrude[j].offset;

                    var inner0 = (v0 - (offset0 * borderWidth));
                    var inner1 = (v1 - (offset1 * borderWidth));
                    var outer0 = (v0 + (offset0 * borderWidth));
                    var outer1 = (v1 + (offset1 * borderWidth));

                    AddTriangleVertex(new CCV3F_C4B(inner0, borderFill));
                    AddTriangleVertex(new CCV3F_C4B(inner1, borderFill));
                    AddTriangleVertex(new CCV3F_C4B(outer1, borderFill));

                    AddTriangleVertex(new CCV3F_C4B(inner0, borderFill));
                    AddTriangleVertex(new CCV3F_C4B(outer0, borderFill));
                    AddTriangleVertex(new CCV3F_C4B(outer1, borderFill));
                }

                if (closePolygon)
                {
                    for (int i = polycount - 1; i < polycount; i++)
                    {
                        int j = (i + 1) % polycount;
                        var v0 = verts[i];
                        var v1 = verts[j];

                        var offset0 = extrude[i].offset;
                        var offset1 = extrude[j].offset;

                        var inner0 = (v0 - (offset0 * borderWidth));
                        var inner1 = (v1 - (offset1 * borderWidth));
                        var outer0 = (v0 + (offset0 * borderWidth));
                        var outer1 = (v1 + (offset1 * borderWidth));

                        AddTriangleVertex(new CCV3F_C4B(inner0, borderFill));
                        AddTriangleVertex(new CCV3F_C4B(inner1, borderFill));
                        AddTriangleVertex(new CCV3F_C4B(outer1, borderFill));

                        AddTriangleVertex(new CCV3F_C4B(inner0, borderFill));
                        AddTriangleVertex(new CCV3F_C4B(outer0, borderFill));
                        AddTriangleVertex(new CCV3F_C4B(outer1, borderFill));
                    }
                }
            }

            dirty = true;
        }

        public void Clear()
        {
            TriangleVertices.Clear();
            LineVertices.Clear();

            dirty = false;
            ContentSize = CCSize.Zero;
        }

        static CCPoint startPoint = CCPoint.Zero;
        static CCPoint destinationPoint = CCPoint.Zero;
        static CCPoint controlPoint1 = CCPoint.Zero;
        static CCPoint controlPoint2 = CCPoint.Zero;

        const int SEGMENTS = 50;

        /*
         * Based on the algorithm described in
         *      http://www.stillhq.com/ctpfaq/2002/03/c1088.html#AEN1212
         */
        void
        make_arc(bool start, float x, float y, float width,
            float height, float startAngle, float endAngle, bool antialiasing, bool isPieSlice, float lineWidth, CCColor4B color)
        {
            float delta, bcp;
            double sin_alpha, sin_beta, cos_alpha, cos_beta;
            float PI = (float)Math.PI;

            float rx = width / 2;
            float ry = height / 2;

            /* center */
            float cx = x + rx;
            float cy = y + ry;

            /* angles in radians */
            float alpha = startAngle * PI / 180;
            float beta = endAngle * PI / 180;

            /* adjust angles for ellipses */
            alpha = (float)Math.Atan2(rx * Math.Sin(alpha), ry * Math.Cos(alpha));
            beta = (float)Math.Atan2(rx * Math.Sin(beta), ry * Math.Cos(beta));

            if (Math.Abs(beta - alpha) > PI)
            {
                if (beta > alpha)
                    beta -= 2 * PI;
                else
                    alpha -= 2 * PI;
            }

            delta = beta - alpha;
            bcp = (float)(4.0 / 3.0 * (1 - Math.Cos(delta / 2)) / Math.Sin(delta / 2));

            sin_alpha = Math.Sin(alpha);
            sin_beta = Math.Sin(beta);
            cos_alpha = Math.Cos(alpha);
            cos_beta = Math.Cos(beta);

            /* don't move to starting point if we're continuing an existing curve */
            if (start)
            {
                /* starting point */
                double sx = cx + rx * cos_alpha;
                double sy = cy + ry * sin_alpha;
                if (isPieSlice)
                {
                    destinationPoint.X = (float)sx;
                    destinationPoint.Y = (float)sy;

                    DrawPolygon(new CCPoint[] { startPoint, destinationPoint }, 2, CCColor4B.Transparent, lineWidth, color);
                }

                startPoint.X = (float)sx;
                startPoint.Y = (float)sy;
            }

            destinationPoint.X = cx + rx * (float)cos_beta;
            destinationPoint.Y = cy + ry * (float)sin_beta;

            controlPoint1.X = cx + rx * (float)(cos_alpha - bcp * sin_alpha);
            controlPoint1.Y = cy + ry * (float)(sin_alpha + bcp * cos_alpha);

            controlPoint2.X = cx + rx * (float)(cos_beta + bcp * sin_beta);
            controlPoint2.Y = cy + ry * (float)(sin_beta - bcp * cos_beta);


            DrawCubicBezier(startPoint, controlPoint1, controlPoint2, destinationPoint, SEGMENTS, lineWidth, color);

            startPoint.X = destinationPoint.X;
            startPoint.Y = destinationPoint.Y;
        }


        void
        make_arcs(float x, float y, float width, float height, float startAngle, float sweepAngle,
            bool convert_units, bool antialiasing, bool isPieSlice, float lineWidth, CCColor4B color)
        {
            int i;
            float drawn = 0;
            float endAngle;
            bool enough = false;

            endAngle = startAngle + sweepAngle;
            /* if we end before the start then reverse positions (to keep increment positive) */
            if (endAngle < startAngle)
            {
                var temp = endAngle;
                endAngle = startAngle;
                startAngle = temp;
            }

            if (isPieSlice)
            {
                startPoint.X = x + (width / 2);
                startPoint.Y = y + (height / 2);
            }

            /* i is the number of sub-arcs drawn, each sub-arc can be at most 90 degrees.*/
            /* there can be no more then 4 subarcs, ie. 90 + 90 + 90 + (something less than 90) */
            for (i = 0; i < 4; i++)
            {
                float current = startAngle + drawn;
                float additional;

                if (enough)
                {
                    if (isPieSlice)
                    {
                        startPoint.X = x + (width / 2);
                        startPoint.Y = y + (height / 2);
                        DrawPolygon(new CCPoint[] { destinationPoint, startPoint }, 2, CCColor4B.Transparent, lineWidth, color);
                    }
                    return;
                }

                additional = endAngle - current; /* otherwise, add the remainder */
                if (additional > 90)
                {
                    additional = 90.0f;
                }
                else
                {
                    /* a near zero value will introduce bad artefact in the drawing (#78999) */
                    if ((additional >= -0.0001f) && (additional <= 0.0001f))
                        return;
                    enough = true;
                }

                make_arc((i == 0),    /* only move to the starting pt in the 1st iteration */
                    x, y, width, height,   /* bounding rectangle */
                    current, current + additional, antialiasing, isPieSlice, lineWidth, color);

                drawn += additional;

            }

            if (isPieSlice)
            {
                startPoint.X = x + (width / 2);
                startPoint.Y = y + (height / 2);
                DrawPolygon(new CCPoint[] { destinationPoint, startPoint }, 2, CCColor4B.Transparent, lineWidth, color);
            }

        }

    }

    internal static class SplineMath
    {
        // CatmullRom Spline formula:
        /// <summary>
        /// See http://en.wikipedia.org/wiki/Cubic_Hermite_spline#Cardinal_spline
        /// </summary>
        /// <param name="p0">Control point 1</param>
        /// <param name="p1">Control point 2</param>
        /// <param name="p2">Control point 3</param>
        /// <param name="p3">Control point 4</param>
        /// <param name="tension"> The parameter c is a tension parameter that must be in the interval (0,1). In some sense, this can be interpreted as the "length" of the tangent. c=1 will yield all zero tangents, and c=0 yields a Catmull–Rom spline.</param>
        /// <param name="t">Time along the spline</param>
        /// <returns>The point along the spline for the given time (t)</returns>
        internal static CCPoint CCCardinalSplineAt(CCPoint p0, CCPoint p1, CCPoint p2, CCPoint p3, float tension, float t)
        {
            if (tension < 0f)
            {
                tension = 0f;
            }
            if (tension > 1f)
            {
                tension = 1f;
            }
            float t2 = t * t;
            float t3 = t2 * t;

            /*
             * Formula: s(-ttt + 2tt - t)P1 + s(-ttt + tt)P2 + (2ttt - 3tt + 1)P2 + s(ttt - 2tt + t)P3 + (-2ttt + 3tt)P3 + s(ttt - tt)P4
             */
            float s = (1 - tension) / 2;

            float b1 = s * ((-t3 + (2 * t2)) - t); // s(-t3 + 2 t2 - t)P1
            float b2 = s * (-t3 + t2) + (2 * t3 - 3 * t2 + 1); // s(-t3 + t2)P2 + (2 t3 - 3 t2 + 1)P2
            float b3 = s * (t3 - 2 * t2 + t) + (-2 * t3 + 3 * t2); // s(t3 - 2 t2 + t)P3 + (-2 t3 + 3 t2)P3
            float b4 = s * (t3 - t2); // s(t3 - t2)P4

            float x = (p0.X * b1 + p1.X * b2 + p2.X * b3 + p3.X * b4);
            float y = (p0.Y * b1 + p1.Y * b2 + p2.Y * b3 + p3.Y * b4);

            return new CCPoint(x, y);
        }

        // Bezier cubic formula:
        //	((1 - t) + t)3 = 1 
        // Expands to 
        //   (1 - t)3 + 3t(1-t)2 + 3t2(1 - t) + t3 = 1 
        internal static float CubicBezier(float a, float b, float c, float d, float t)
        {
            float t1 = 1f - t;
            return ((t1 * t1 * t1) * a + 3f * t * (t1 * t1) * b + 3f * (t * t) * (t1) * c + (t * t * t) * d);
        }

        internal static float QuadBezier(float a, float b, float c, float t)
        {
            float t1 = 1f - t;
            return (t1 * t1) * a + 2.0f * (t1) * t * b + (t * t) * c;

        }
    }

}
