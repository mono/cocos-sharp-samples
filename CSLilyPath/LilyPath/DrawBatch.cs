using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CocosSharp;
using LilyPath.Utility;

namespace LilyPath
{
    /// <summary>
    /// Enables a group of figures to be drawn using the same settings.
    /// </summary>
    /// <remarks><para>Figures can be made up of primitive lines or paths stroked
    /// or filled into more complex polygon geometry.</para>
    /// <para>When drawing paths with a thick pen and very short segments, such as closed
    /// arcs, you may encounter overdraw that is visible with semitransparent pens.  You can 
    /// avoid this overdraw by using an inset or outset <see cref="PenAlignment"/> on your pen,
    /// depending on the winding order of the path being stroked.</para></remarks>
    public class DrawBatch : CCGeometryNode, IDisposable
    {
        private struct DrawingInfo
        {
            public CCTexture2D Texture;
            public PrimitiveType Primitive;
            public int IndexCount;
            public int VertexCount;
        }

        private static CCAffineTransform _identityMatrix = CCAffineTransform.Identity;

        //private GraphicsDevice _device;
        private bool _inDraw = true;
        private bool _isDisposed;

        //// Render data
        private DrawingInfo[] _infoBuffer;
        private short[] _indexBuffer;
        private VertexPositionColorTexture[] _vertexBuffer;

        //// Temporary compute buffers
        private CCVector2[] _computeBuffer;
        private Color[] _colorBuffer;

        //// Temporary geometry building
        private CCVector2[] _geometryBuffer;
        private PathBuilder _pathBuilder;

        private int _infoBufferIndex;
        private int _indexBufferIndex;
        private int _vertexBufferIndex;

        private Triangulator _triangulator;

        private DrawSortMode _sortMode;
        //private BlendState _blendState;
        //private SamplerState _samplerState;
        //private DepthStencilState _depthStencilState;
        //private RasterizerState _rasterizerState;
        //private Effect _effect;
        //private Matrix _transform;

        //private BasicEffect _standardEffect;

        private CCTexture2D _defaultTexture;

        private PenWorkspace _ws;

        /// <summary>
        /// Enables a group of figures to be drawn using the same settings.
        /// </summary>
        /// <param name="device"></param>
        //public DrawBatch (GraphicsDevice device)
        //{
        public DrawBatch () : base()
        {
            //if (device == null)
            //    throw new ArgumentNullException("device");

            //_device = device;
            //_device.DeviceReset += GraphicsDeviceReset;

            _infoBuffer = new DrawingInfo[2048];
            //var info = base.CreateGeometryInstance(8192, 32768);
            
            _indexBuffer = new short[32768];
            _vertexBuffer = new VertexPositionColorTexture[8192];
            _computeBuffer = new CCVector2[64];
            _colorBuffer = new Color[64];
            _geometryBuffer = new CCVector2[256];
            _pathBuilder = new PathBuilder();

            //_standardEffect = new BasicEffect(device);
            //_standardEffect.TextureEnabled = true;
            //_standardEffect.VertexColorEnabled = true;

            //_defaultTexture = new Texture2D(device, 1, 1);
            //_defaultTexture.SetData<Color>(new Color[] { Microsoft.Xna.Framework.Color.White });
            _sortMode = DrawSortMode.Immediate;

            _ws = new PenWorkspace();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="DrawBatch"/> object.
        /// </summary>
        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose (bool disposing)
        {
            if (!_isDisposed && disposing) {
                //_device.DeviceReset -= GraphicsDeviceReset;

                //_standardEffect.Dispose();
                _defaultTexture.Dispose();
                
                _isDisposed = true;
            }
        }

        private void GraphicsDeviceReset (object sender, EventArgs e)
        {
            //_standardEffect.Dispose();
            //_standardEffect = new BasicEffect(_device);
            //_standardEffect.TextureEnabled = true;
            //_standardEffect.VertexColorEnabled = true;

            //_defaultTexture.Dispose();
            //_defaultTexture = new Texture2D(_device, 1, 1);
            //_defaultTexture.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// Gets whether the <see cref="DrawBatch"/> has been disposed or not.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        ///// <summary>
        ///// The <see cref="GraphicsDevice"/> associated with this batch.
        ///// </summary>
        //public GraphicsDevice GraphicsDevice
        //{
        //    get { return _device; }
        //}

        internal CCTexture2D DefaultTexture
        {
            get { return _defaultTexture; }
        }

        /// <summary>
        /// Begins a draw batch operation using deferred sort and default state objects.
        /// </summary>
        //public void Begin ()
        //{
        //    Begin(DrawSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
        //}

        ///// <summary>
        ///// Begins a draw batch operation using the specified sort and default state objects.
        ///// </summary>
        ///// <param name="sortMode">The drawing order.</param>
        //public void Begin (DrawSortMode sortMode)
        //{
        //    Begin(sortMode, null, null, null, null, null, Matrix.Identity);
        //}

        ///// <summary>
        ///// Begins a draw batch operation using the specified sort and given state objects.
        ///// </summary>
        ///// <param name="sortMode">The drawing order.</param>
        ///// <param name="blendState">Blending options.</param>
        //public void Begin (DrawSortMode sortMode, BlendState blendState)
        //{
        //    Begin(sortMode, blendState, null, null, null, null, Matrix.Identity);
        //}

        ///// <summary>
        ///// Begins a draw batch operation using the specified sort and given state objects.
        ///// </summary>
        ///// <param name="sortMode">The drawing order.</param>
        ///// <param name="blendState">Blending options.</param>
        ///// <param name="samplerState">Texture sampling options.</param>
        ///// <param name="depthStencilState">Depth and stencil options.</param>
        ///// <param name="rasterizerState">Rasterization options.</param>
        //public void Begin (DrawSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        //{
        //    Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Matrix.Identity);
        //}

        ///// <summary>
        ///// Begins a draw batch operation using the specified sort and given state objects.
        ///// </summary>
        ///// <param name="sortMode">The drawing order.</param>
        ///// <param name="blendState">Blending options.</param>
        ///// <param name="samplerState">Texture sampling options.</param>
        ///// <param name="depthStencilState">Depth and stencil options.</param>
        ///// <param name="rasterizerState">Rasterization options.</param>
        ///// <param name="effect">Effect state options.</param>
        //public void Begin (DrawSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        //{
        //    Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
        //}

        ///// <summary>
        ///// Begins a draw batch operation using the specified sort, given state objects, and transformation matrix.
        ///// </summary>
        ///// <param name="sortMode">The drawing order.</param>
        ///// <param name="blendState">Blending options.</param>
        ///// <param name="samplerState">Texture sampling options.</param>
        ///// <param name="depthStencilState">Depth and stencil options.</param>
        ///// <param name="rasterizerState">Rasterization options.</param>
        ///// <param name="effect">Effect state options.</param>
        ///// <param name="transform">Transformation matrix for scale, rotate, translate options.</param>
        //public void Begin (DrawSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transform)
        //{
        //    if (_inDraw)
        //        throw new InvalidOperationException("DrawBatch already inside Begin/End pair");

        //    _sortMode = sortMode;
        //    _blendState = blendState;
        //    _samplerState = samplerState;
        //    _depthStencilState = depthStencilState;
        //    _rasterizerState = rasterizerState;
        //    _effect = effect;
        //    _transform = transform;

        //    _infoBufferIndex = 0;
        //    _indexBufferIndex = 0;
        //    _vertexBufferIndex = 0;

        //    if (sortMode == DrawSortMode.Immediate)
        //        SetRenderState();

        //    _inDraw = true;
        //}

        ///// <summary>
        ///// Flushes the draw batch.
        ///// </summary>
        //public void End ()
        //{
        //    if (!_inDraw)
        //        throw new InvalidOperationException();

        //    _inDraw = false;

        //    if (_sortMode != DrawSortMode.Immediate)
        //        SetRenderState();

        //    FlushBuffer();
        //}

        /// <summary>
        /// Computes and adds a rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="rect">The rectangle to be rendered.</param>
        /// <exception cref="InvalidOperationException"><c>DrawRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawRectangle (Pen pen, CCRect rect)
        {
            DrawRectangle(pen, rect, 0);
        }

        /// <summary>
        /// Computes and adds a rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="rect">The rectangle to be rendered.</param>
        /// <param name="angle">The angle to rotate the rectangle by around its center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawRectangle (Pen pen, CCRect rect, float angle)
        {
            _geometryBuffer[0] = new CCVector2(rect.MinX, rect.MaxY);
            _geometryBuffer[1] = new CCVector2(rect.MaxX, rect.MaxY);
            _geometryBuffer[2] = new CCVector2(rect.MaxX, rect.MinY);
            _geometryBuffer[3] = new CCVector2(rect.MinX, rect.MinY);

            DrawQuad(pen, _geometryBuffer, 0, angle);
        }

        /// <summary>
        /// Computes and adds a rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="location">The top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <exception cref="InvalidOperationException"><c>DrawRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawRectangle(Pen pen, CCVector2 location, float width, float height)
        {
            DrawRectangle(pen, location, width, height, 0);
        }

        /// <summary>
        /// Computes and adds a rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="location">The top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="angle">The angle to rotate the rectangle by around its center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawRectangle(Pen pen, CCVector2 location, float width, float height, float angle)
        {
            _geometryBuffer[0] = location;
            _geometryBuffer[1] = new CCVector2(location.X + width, location.Y);
            _geometryBuffer[2] = new CCVector2(location.X + width, location.Y + height);
            _geometryBuffer[3] = new CCVector2(location.X, location.Y + height);

            DrawQuad(pen, _geometryBuffer, 0, angle);
        }

        /// <summary>
        /// Computes and adds a quadrilateral path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="points">An array containing the coordinates of the quad.</param>
        /// <param name="offset">The offset into the points array.</param>
        /// <exception cref="InvalidOperationException"><c>DrawQuad</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawQuad(Pen pen, CCVector2[] points, int offset)
        {
            DrawQuad(pen, points, offset, 0);
        }
        
        /// <summary>
        /// Computes and adds a quadrilateral path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="points">An array containing the coordinates of the quad.</param>
        /// <param name="offset">The offset into the points array.</param>
        /// <param name="angle">The angle to rotate the quad by around its weighted center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawQuad</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawQuad(Pen pen, CCVector2[] points, int offset, float angle)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < offset + 4)
                throw new ArgumentException("Points array is too small for the given offset.");

            RequestBufferSpace(8, 24);
            _ws.ResetWorkspace(pen);

            AddInfo(PrimitiveType.TriangleList, 8, 24, pen.Brush);

            if (points != _geometryBuffer)
                Array.Copy(points, _geometryBuffer, 4);

            if (angle != 0) {
                float centerX = (_geometryBuffer[0].X + _geometryBuffer[1].X + _geometryBuffer[2].X + _geometryBuffer[3].X) / 4;
                float centerY = (_geometryBuffer[0].Y + _geometryBuffer[1].Y + _geometryBuffer[2].Y + _geometryBuffer[3].Y) / 4;
                CCVector2 center = new CCVector2(centerX, centerY);

                Matrix transform = Matrix.CreateRotationZ(angle);
                transform.Translation = new Vector3(center.X, centerY, 0);

                var refTransform = transform.toCCAffineTransform();
                
                for (int i = 0; i < 4; i++)
                    _geometryBuffer[i] = CCVector2.Transform(_geometryBuffer[i] - center, refTransform);
            }

            int baseVertexIndex = _vertexBufferIndex;

            JoinSample js = new JoinSample(_geometryBuffer[0], _geometryBuffer[1], _geometryBuffer[2]);
            AddMiteredJoint(ref js, pen, _ws);
            js.Advance(_geometryBuffer[3]);
            AddMiteredJoint(ref js, pen, _ws);
            js.Advance(_geometryBuffer[0]);
            AddMiteredJoint(ref js, pen, _ws);
            js.Advance(_geometryBuffer[1]);
            AddMiteredJoint(ref js, pen, _ws);

            AddSegment(baseVertexIndex + 0, baseVertexIndex + 2);
            AddSegment(baseVertexIndex + 2, baseVertexIndex + 4);
            AddSegment(baseVertexIndex + 4, baseVertexIndex + 6);
            AddSegment(baseVertexIndex + 6, baseVertexIndex + 0);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a primitive rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="rect">The rectangle to be rendered.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveRectangle (Pen pen, CCRect rect)
        {
            DrawPrimitiveRectangle(pen, rect, 0);
        }

        /// <summary>
        /// Adds a primitive rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="rect">The rectangle to be rendered.</param>
        /// <param name="angle">The angle to rotate the rectangle by around its center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveRectangle (Pen pen, CCRect rect, float angle)
        {
            _geometryBuffer[0] = new CCVector2(rect.MinX, rect.MaxY);
            _geometryBuffer[1] = new CCVector2(rect.MaxX, rect.MaxY);
            _geometryBuffer[2] = new CCVector2(rect.MaxX, rect.MinY);
            _geometryBuffer[3] = new CCVector2(rect.MinX, rect.MinY);

            DrawPrimitiveQuad(pen, _geometryBuffer, 0, angle);
        }

        /// <summary>
        /// Adds a primitive rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="location">The top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveRectangle (Pen pen, CCVector2 location, float width, float height)
        {
            DrawPrimitiveRectangle(pen, location, width, height, 0);
        }

        /// <summary>
        /// Adds a primitive rectangle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="location">The top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="angle">The angle to rotate the rectangle by around its center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveRectangle (Pen pen, CCVector2 location, float width, float height, float angle)
        {
            _geometryBuffer[0] = location;
            _geometryBuffer[1] = new CCVector2(location.X + width, location.Y);
            _geometryBuffer[2] = new CCVector2(location.X + width, location.Y + height);
            _geometryBuffer[3] = new CCVector2(location.X, location.Y + height);

            DrawPrimitiveQuad(pen, _geometryBuffer, 0, angle);
        }

        /// <summary>
        /// Adds a primitive quadrilateral to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="points">An array containing the coordinates of the quad.</param>
        /// <param name="offset">The offset into the points array.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveQuad</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveQuad(Pen pen, CCVector2[] points, int offset)
        {
            DrawPrimitiveQuad(pen, points, offset, 0);
        }

        /// <summary>
        /// Adds a primitive quadrilateral to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="points">An array containing the coordinates of the quad.</param>
        /// <param name="offset">The offset into the points array.</param>
        /// <param name="angle">The angle to rotate the quad by around its weighted center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveQuad</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveQuad(Pen pen, CCVector2[] points, int offset, float angle)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < offset + 4)
                throw new ArgumentException("Points array is too small for the given offset.");

            RequestBufferSpace(4, 8);

            AddInfo(PrimitiveType.LineList, 4, 8, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(points[offset + 0].X, points[offset + 0].Y, 0), pen.Color, new Vector2(points[offset + 0].X,points[offset + 0].Y) );
            _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(points[offset + 1].X, points[offset + 1].Y, 0), pen.Color, new Vector2(points[offset + 1].X, points[offset + 1].Y));
            _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(points[offset + 2].X, points[offset + 2].Y, 0), pen.Color, new Vector2(points[offset + 2].X, points[offset + 2].Y));
            _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(points[offset + 3].X, points[offset + 3].Y, 0), pen.Color, new Vector2(points[offset + 3].X, points[offset + 3].Y));

            if (angle != 0) {
                float centerX = (points[offset + 0].X + points[offset + 1].X + points[offset + 2].X + points[offset + 3].X) / 4;
                float centerY = (points[offset + 0].Y + points[offset + 1].Y + points[offset + 2].Y + points[offset + 3].Y) / 4;
                Vector3 center = new Vector3(centerX, centerY, 0);

                Matrix transform = Matrix.CreateRotationZ(angle);
                transform.Translation = center;

                for (int i = _vertexBufferIndex - 4; i < _vertexBufferIndex; i++)
                    _vertexBuffer[i].Position = Vector3.Transform(_vertexBuffer[i].Position - center, transform);
            }

            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 1);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 1);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 2);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 2);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 3);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 3);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Computes and adds a point path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="point">The point to be rendered.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPoint</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPoint(Pen pen, CCVector2 point)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            RequestBufferSpace(4, 6);

            AddInfo(PrimitiveType.TriangleList, 4, 6, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            float w2 = pen.Width / 2;
            AddVertex(new Vector2(point.X - w2, point.Y - w2), pen);
            AddVertex(new Vector2(point.X + w2, point.Y - w2), pen);
            AddVertex(new Vector2(point.X - w2, point.Y + w2), pen);
            AddVertex(new Vector2(point.X + w2, point.Y + w2), pen);

            AddSegment(baseVertexIndex + 0, baseVertexIndex + 2);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Computes and adds a line segment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="p0">The first point of the line segment.</param>
        /// <param name="p1">The second point of the line segment.</param>
        /// <exception cref="InvalidOperationException"><c>DrawLine</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawLine(Pen pen, CCVector2 p0, CCVector2 p1)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            RequestBufferSpace(4, 6);
            _ws.ResetWorkspace(pen);

            if (pen.NeedsPathLength)
                _ws.PathLength = CCVector2.Distance(p0, p1);

            AddInfo(PrimitiveType.TriangleList, 4, 6, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            AddStartPoint(p0, p1, pen, _ws);
            AddEndPoint(p0, p1, pen, _ws);

            AddSegment(baseVertexIndex + 0, baseVertexIndex + 2);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a primitive line segment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="p0">The first point of the line segment.</param>
        /// <param name="p1">The second point of the line segment.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveLine</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveLine(Pen pen, CCVector2 p0, CCVector2 p1)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            RequestBufferSpace(2, 2);

            AddInfo(PrimitiveType.LineList, 2, 2, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(p0.X, p0.Y, 0), pen.Color, new Vector2(p0.X, p0.Y));
            _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(p1.X, p1.Y, 0), pen.Color, new Vector2(p1.X, p1.Y));

            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex);
            _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + 1);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a primitive multisegment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="points">The list of points that make up the path to be rendered.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitivePath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitivePath(Pen pen, IList<CCVector2> points)
        {
            DrawPrimitivePath(pen, points, 0, points.Count, PathType.Open);
        }

        /// <summary>
        /// Adds a primitive multisegment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="points">The list of points that make up the path to be rendered.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitivePath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitivePath (Pen pen, IList<CCVector2> points, PathType pathType)
        {
            DrawPrimitivePath(pen, points, 0, points.Count, pathType);
        }

        /// <summary>
        /// Adds a primitive multisegment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="points">The list of points that make up the path to be rendered.</param>
        /// <param name="offset">The offset into the <paramref name="points"/> list to begin rendering.</param>
        /// <param name="count">The number of points that should be rendered, starting from <paramref name="offset"/>.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitivePath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitivePath (Pen pen, IList<CCVector2> points, int offset, int count)
        {
            DrawPrimitivePath(pen, points, offset, count, PathType.Open);
        }

        /// <summary>
        /// Adds a primitive multisegment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying a color to render the path with.</param>
        /// <param name="points">The list of points that make up the path to be rendered.</param>
        /// <param name="offset">The offset into the <paramref name="points"/> list to begin rendering.</param>
        /// <param name="count">The number of points that should be rendered, starting from <paramref name="offset"/>.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitivePath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitivePath(Pen pen, IList<CCVector2> points, int offset, int count, PathType pathType)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (offset + count > points.Count)
                throw new ArgumentOutOfRangeException("points", "The offset and count exceed the bounds of the list");

            RequestBufferSpace(count, (pathType == PathType.Open) ? count * 2 - 2 : count * 2);

            AddInfo(PrimitiveType.LineList, count, (pathType == PathType.Open) ? count * 2 - 2 : count * 2, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            for (int i = 0; i < count; i++) {
                CCVector2 pos = new CCVector2(points[offset + i].X, points[offset + i].Y);
                _vertexBuffer[_vertexBufferIndex++] = new VertexPositionColorTexture(new Vector3(pos.X, pos.Y, 0), pen.Color, Vector2.Zero);
            }

            for (int i = 1; i < count; i++) {
                _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + i - 1);
                _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + i);
            }

            if (pathType == PathType.Closed) {
                _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex + count - 1);
                _indexBuffer[_indexBufferIndex++] = (short)(baseVertexIndex);
            }

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a precomputed path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="path">A path that has already been stroked with a <see cref="Pen"/>.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPath(GraphicsPath path)
        {
            if (!_inDraw)
                throw new InvalidOperationException();

            var identity = Matrix.Identity;

            //DrawPathInner(path, ref _identityMatrix, false);
            DrawPathInner(path, ref identity, false);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /*public void DrawPath (GraphicsPath path, ref Matrix vertexTransform)
        {
            if (!_inDraw)
                throw new InvalidOperationException();

            DrawPathInner(path, ref vertexTransform, true);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }*/

        private void DrawPathInner(GraphicsPath path, ref Matrix vertexTransform, bool applyVertexTransform)
        {
            RequestBufferSpace(path.VertexCount, path.IndexCount);

            AddInfo(PrimitiveType.TriangleList, path.VertexCount, path.IndexCount, path.Pen.Brush);

            if (path.VertexTextureData != null)
            {
                for (int i = 0; i < path.VertexCount; i++)
                {
                    _vertexBuffer[_vertexBufferIndex + i] = new VertexPositionColorTexture(
                        new Vector3(path.VertexPositionData[i].X,path.VertexPositionData[i].Y, 0),
                        path.VertexColorData[i],
                        new Vector2(path.VertexPositionData[i].X,path.VertexPositionData[i].Y));
                }
            }
            else
            {
                for (int i = 0; i < path.VertexCount; i++)
                {
                    _vertexBuffer[_vertexBufferIndex + i] = new VertexPositionColorTexture(
                        new Vector3(path.VertexPositionData[i].X, path.VertexPositionData[i].Y, 0),
                        path.VertexColorData[i],
                        Vector2.Zero);
                }
            }

            for (int i = 0; i < path.IndexCount; i++)
            {
                _indexBuffer[_indexBufferIndex + i] = (short)(path.IndexData[i] + _vertexBufferIndex);
            }

            _vertexBufferIndex += path.VertexCount;
            _indexBufferIndex += path.IndexCount;

            foreach (GraphicsPath outlinePath in path.OutlinePaths)
                DrawPathInner(outlinePath, ref vertexTransform, applyVertexTransform);
        }

        //private void TransformData (Vector3[] data, int start, int length, ref Matrix tranform)
        //{
        //    //Vector3.Transform()
        //}

        /// <summary>
        /// Computes and adds a circle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <exception cref="InvalidOperationException"><c>DrawCircle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the circle is computed as the radius / 1.5.</remarks>
        public void DrawCircle(Pen pen, CCVector2 center, float radius)
        {
            DrawCircle(pen, center, radius, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Computes and adds a circle path to the batch of figures to be rendered using a given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the circle with.</param>
        /// <exception cref="InvalidOperationException"><c>DrawCircle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawCircle(Pen pen, CCVector2 center, float radius, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            BuildCircleGeometryBuffer(center, radius, subdivisions, false);
            AddClosedPath(_geometryBuffer, 0, subdivisions, pen, _ws);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Computes and adds an ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <exception cref="InvalidOperationException"><c>DrawEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void DrawEllipse (Pen pen, CCRect bound)
        {
            DrawEllipse(pen, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, 0);
        }

        /// <summary>
        /// Computes and adds an ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void DrawEllipse (Pen pen, CCRect bound, float angle)
        {
            DrawEllipse(pen, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, angle);
        }

        /// <summary>
        /// Computes and adds an ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the ellipse with.</param>
        /// <exception cref="InvalidOperationException"><c>DrawEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawEllipse (Pen pen, CCRect bound, float angle, int subdivisions)
        {
            DrawEllipse(pen, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, angle, subdivisions);
        }

        /// <summary>
        /// Computes and adds an ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the ellipse along the y-acis.</param>
        /// <exception cref="InvalidOperationException"><c>DrawEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void DrawEllipse(Pen pen, CCVector2 center, float xRadius, float yRadius)
        {
            DrawEllipse(pen, center, xRadius, yRadius, 0);
        }

        /// <summary>
        /// Computes and adds an ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the ellipse along the y-acis.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void DrawEllipse(Pen pen, CCVector2 center, float xRadius, float yRadius, float angle)
        {
            DrawEllipse(pen, center, xRadius, yRadius, angle, DefaultSubdivisions(xRadius, yRadius));
        }

        /// <summary>
        /// Computes and adds an ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the ellipse along the y-acis.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the ellipse with.</param>
        /// <exception cref="InvalidOperationException"><c>DrawEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawEllipse(Pen pen, CCVector2 center, float xRadius, float yRadius, float angle, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            BuildEllipseGeometryBuffer(center, xRadius, yRadius, angle, subdivisions);
            AddClosedPath(_geometryBuffer, 0, subdivisions, pen, _ws);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a primitive circle path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveCircle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the circle is computed as the radius / 1.5.</remarks>
        public void DrawPrimitiveCircle(Pen pen, CCVector2 center, float radius)
        {
            DrawPrimitiveCircle(pen, center, radius, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Adds a primitive circle path to the batch of figures to be rendered using a given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the circle with.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveCircle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveCircle(Pen pen, CCVector2 center, float radius, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            BuildCircleGeometryBuffer(center, radius, subdivisions, false);
            DrawPrimitivePath(pen, _geometryBuffer, 0, subdivisions, PathType.Closed);
        }

        private void BuildCircleGeometryBuffer(CCVector2 center, float radius, int subdivisions, bool connect)
        {
            List<CCVector2> unitCircle = CalculateCircleSubdivisions(subdivisions);

            if (_geometryBuffer.Length < subdivisions + 1)
                Array.Resize(ref _geometryBuffer, (subdivisions + 1) * 2);

            for (int i = 0; i < subdivisions; i++)
                _geometryBuffer[i] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y + radius * unitCircle[i].Y);

            if (connect)
                _geometryBuffer[subdivisions] = new CCVector2(center.X + radius * unitCircle[0].X, center.Y + radius * unitCircle[0].Y);
        }

        /// <summary>
        /// Adds a primitive ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(width, height) / 3.0.</remarks>
        public void DrawPrimitiveEllipse (Pen pen, CCRect bound)
        {
            DrawPrimitiveEllipse(pen, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, 0);
        }

        /// <summary>
        /// Adds a primitive ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(width, height) / 3.0.</remarks>
        public void DrawPrimitiveEllipse (Pen pen, CCRect bound, float angle)
        {
            DrawPrimitiveEllipse(pen, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, angle);
        }

        /// <summary>
        /// Adds a primitive ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the ellipse with.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveEllipse (Pen pen, CCRect bound, float angle, int subdivisions)
        {
            DrawPrimitiveEllipse(pen, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, angle, subdivisions);
        }

        /// <summary>
        /// Adds a primitive ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the ellipse along the y-acis.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void DrawPrimitiveEllipse (Pen pen, CCVector2 center, float xRadius, float yRadius)
        {
            DrawPrimitiveEllipse(pen, center, xRadius, yRadius, 0);
        }

        /// <summary>
        /// Adds a primitive ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the ellipse along the y-acis.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void DrawPrimitiveEllipse (Pen pen, CCVector2 center, float xRadius, float yRadius, float angle)
        {
            DrawPrimitiveEllipse(pen, center, xRadius, yRadius, angle, DefaultSubdivisions(xRadius, yRadius));
        }

        /// <summary>
        /// Adds a primitive ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the ellipse along the y-acis.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the ellipse with.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveEllipse (Pen pen, CCVector2 center, float xRadius, float yRadius, float angle, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            BuildEllipseGeometryBuffer(center, xRadius, yRadius, angle, subdivisions);
            DrawPrimitivePath(pen, _geometryBuffer, 0, subdivisions, PathType.Closed);
        }

        private void BuildEllipseGeometryBuffer (CCVector2 center, float xRadius, float yRadius, float angle, int subdivisions)
        {
            float radius = Math.Min(xRadius, yRadius);

            BuildCircleGeometryBuffer(CCVector2.Zero, radius, subdivisions, false);

            Matrix transform = Matrix.CreateScale(xRadius / radius, yRadius / radius, 1f);
            transform *= Matrix.CreateRotationZ(angle);
            transform.Translation = new Vector3(center.X, center.Y, 0);

            var affineTransform = transform.toCCAffineTransform();

            for (int i = 0; i < subdivisions; i++)
                _geometryBuffer[i] = CCVector2.Transform(_geometryBuffer[i], affineTransform);
        }

        /// <summary>
        /// Computes and adds an arc path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle)
        {
            DrawArc(pen, center, radius, startAngle, arcAngle, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Computes and adds an arc path to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions in a circle of the same radius.</param>
        /// <exception cref="InvalidOperationException"><c>DrawArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>subdivisions * (arcAngle / 2PI)</c>.</remarks>
        public void DrawArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddArc(center, radius, startAngle, arcAngle, subdivisions);

            if (_pathBuilder.Count > 1) {
                AddPath(_pathBuilder.Buffer, 0, _pathBuilder.Count, pen, _ws);

                if (_sortMode == DrawSortMode.Immediate)
                    FlushBuffer();
            }
        }

        /// <summary>
        /// Computes and adds an arc path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="p0">The starting point of the arc.</param>
        /// <param name="p1">The ending point of the arc.</param>
        /// <param name="height">The furthest point on the arc from the line connecting <paramref name="p0"/> and <paramref name="p1"/>.</param>
        /// <exception cref="InvalidOperationException"><c>DrawArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawArc (Pen pen, CCVector2 p0, CCVector2 p1, float height)
        {
            float width = (p1 - p0).Length();
            float radius = (height / 2) + (width * width) / (height * 8);
            DrawArc(pen, p0, p1, height, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Computes and adds an arc path to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="p0">The starting point of the arc.</param>
        /// <param name="p1">The ending point of the arc.</param>
        /// <param name="height">The furthest point on the arc from the line connecting <paramref name="p0"/> and <paramref name="p1"/>.</param>
        /// <param name="subdivisions">The number of subdivisions in a circle of the same arc radius.</param>
        /// <exception cref="InvalidOperationException"><c>DrawArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(subdivisions) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawArc (Pen pen, CCVector2 p0, CCVector2 p1, float height, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddArc(p0, p1, height, subdivisions);

            if (_pathBuilder.Count > 1)
            {
                AddPath(_pathBuilder.Buffer, 0, _pathBuilder.Count, pen, _ws);

                if (_sortMode == DrawSortMode.Immediate)
                    FlushBuffer();
            }
        }

        /// <summary>
        /// Adds a primitive arc path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawPrimitiveArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle)
        {
            DrawPrimitiveArc(pen, center, radius, startAngle, arcAngle, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Adds a primitive arc path to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions in a circle of the same radius.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawPrimitiveArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddArc(center, radius, startAngle, arcAngle, subdivisions);

            if (_pathBuilder.Count > 1)
                DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count, PathType.Open);
        }

        /// <summary>
        /// Adds a primitive arc path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="p0">The starting point of the arc.</param>
        /// <param name="p1">The ending point of the arc.</param>
        /// <param name="height">The furthest point on the arc from the line connecting <paramref name="p0"/> and <paramref name="p1"/>.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawPrimitiveArc (Pen pen, CCVector2 p0, CCVector2 p1, float height)
        {
            float width = (p1 - p0).Length();
            float radius = (height / 2) + (width * width) / (height * 8);
            DrawPrimitiveArc(pen, p0, p1, height, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Adds a primitive arc path to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="p0">The starting point of the arc.</param>
        /// <param name="p1">The ending point of the arc.</param>
        /// <param name="height">The furthest point on the arc from the line connecting <paramref name="p0"/> and <paramref name="p1"/>.</param>
        /// <param name="subdivisions">The number of subdivisions in a circle of the same arc radius.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(subdivisions) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawPrimitiveArc (Pen pen, CCVector2 p0, CCVector2 p1, float height, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddArc(p0, p1, height, subdivisions);

            if (_pathBuilder.Count > 1)
                DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count, PathType.Open);
        }

        /// <summary>
        /// Adds a closed primitive arc path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="arcType">Whether the arc is drawn as a segment or a sector.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveClosedArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawPrimitiveClosedArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle, ArcType arcType)
        {
            DrawPrimitiveClosedArc(pen, center, radius, startAngle, arcAngle, arcType, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Adds a closed primitive arc path to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="arcType">Whether the arc is drawn as a segment or a sector.</param>
        /// <param name="subdivisions">The number of subdivisions in a circle of the same radius.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveClosedArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(subdivisions * (arcAngle / 2PI)</c>.</remarks>
        public void DrawPrimitiveClosedArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle, ArcType arcType, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;

            if (arcType == ArcType.Sector)
                _pathBuilder.AddPoint(center);
            _pathBuilder.AddArc(center, radius, startAngle, arcAngle, subdivisions);

            if (_pathBuilder.Count > 1)
                DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count, PathType.Closed);
        }

        /// <summary>
        /// Computes and adds a closed arc path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="arcType">Whether the arc is drawn as a segment or a sector.</param>
        /// <exception cref="InvalidOperationException"><c>DrawClosedArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void DrawClosedArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle, ArcType arcType)
        {
            DrawClosedArc(pen, center, radius, startAngle, arcAngle, arcType, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Computes and adds a closed arc path to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="center">The center coordinate of the the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="arcType">Whether the arc is drawn as a segment or a sector.</param>
        /// <param name="subdivisions">The number of subdivisions in a circle of the same radius.</param>
        /// <exception cref="InvalidOperationException"><c>DrawClosedArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>subdivisions * (arcAngle / 2PI)</c>.</remarks>
        public void DrawClosedArc (Pen pen, CCVector2 center, float radius, float startAngle, float arcAngle, ArcType arcType, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;

            if (arcType == ArcType.Sector)
                _pathBuilder.AddPoint(center);
            _pathBuilder.AddArc(center, radius, startAngle, arcAngle, subdivisions);

            if (_pathBuilder.Count > 1)
            {
                AddClosedPath(_pathBuilder.Buffer, 0, _pathBuilder.Count, pen, _ws);

                if (_sortMode == DrawSortMode.Immediate)
                    FlushBuffer();
            }
        }

        /// <summary>
        /// Computes and adds a quadratic Bezier path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="p0">The start point of the curve.</param>
        /// <param name="p1">The first control point.</param>
        /// <param name="p2">The end point of the curve.</param>
        /// <exception cref="InvalidOperationException"><c>DrawBezier</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawBezier (Pen pen, CCVector2 p0, CCVector2 p1, CCVector2 p2)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddBezier(p0, p1, p2);

            AddPath(_pathBuilder.Buffer, 0, _pathBuilder.Count, pen, _ws);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Computes and adds a cubic Bezier path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="p0">The start point of the curve.</param>
        /// <param name="p1">The first control point.</param>
        /// <param name="p2">The second control point.</param>
        /// <param name="p3">The end point of the curve.</param>
        /// <exception cref="InvalidOperationException"><c>DrawBezier</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawBezier (Pen pen, CCVector2 p0, CCVector2 p1, CCVector2 p2, CCVector2 p3)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _ws.ResetWorkspace(pen);

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddBezier(p0, p1, p2, p3);

            AddPath(_pathBuilder.Buffer, 0, _pathBuilder.Count, pen, _ws);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Computes and adds a series of Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawBeziers (Pen pen, IList<CCVector2> points, BezierType bezierType)
        {
            DrawBeziers(pen, points, 0, points.Count, bezierType, PathType.Open);
        }

        /// <summary>
        /// Computes and adds a series of Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawBeziers (Pen pen, IList<CCVector2> points, BezierType bezierType, PathType pathType)
        {
            DrawBeziers(pen, points, 0, points.Count, bezierType, pathType);
        }

        /// <summary>
        /// Computes and adds a series of Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="offset">The index of the first start point in the list.</param>
        /// <param name="count">The number of points to use.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawBeziers (Pen pen, IList<CCVector2> points, int offset, int count, BezierType bezierType)
        {
            DrawBeziers(pen, points, offset, count, bezierType, PathType.Open);
        }

        /// <summary>
        /// Computes and adds a series of Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="offset">The index of the first start point in the list.</param>
        /// <param name="count">The number of points to use.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawBeziers (Pen pen, IList<CCVector2> points, int offset, int count, BezierType bezierType, PathType pathType)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points.Count < offset + count)
                throw new ArgumentOutOfRangeException("The offset and count are out of range for the given points argument.");

            _ws.ResetWorkspace(pen);

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;

            switch (bezierType)
            {
                case BezierType.Quadratic:
                    for (int i = offset + 2; i < offset + count; i += 2)
                        _pathBuilder.AddBezier(points[i - 2], points[i - 1], points[i]);
                    if (pathType == PathType.Closed)
                        _pathBuilder.AddBezier(points[offset + count - 2], points[offset + count - 1], points[offset]);
                    break;

                case BezierType.Cubic:
                    for (int i = offset + 3; i < offset + count; i += 3)
                        _pathBuilder.AddBezier(points[i - 3], points[i - 2], points[i - 1], points[i]);
                    if (pathType == PathType.Closed)
                        _pathBuilder.AddBezier(points[offset + count - 3], points[offset + count - 2], points[offset + count - 1], points[offset]);
                    break;
            }

            switch (pathType)
            {
                case PathType.Open:
                    AddPath(_pathBuilder.Buffer, 0, _pathBuilder.Count, pen, _ws);
                    break;

                case PathType.Closed:
                    AddClosedPath(_pathBuilder.Buffer, 0, _pathBuilder.Count - 1, pen, _ws);
                    break;
            }

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a primitive quadratic Bezier path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="p0">The start point of the curve.</param>
        /// <param name="p1">The first control point.</param>
        /// <param name="p2">The end point of the curve.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveBezier</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveBezier (Pen pen, CCVector2 p0, CCVector2 p1, CCVector2 p2)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddBezier(p0, p1, p2);

            DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a primitive cubic Bezier path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="p0">The start point of the curve.</param>
        /// <param name="p1">The first control point.</param>
        /// <param name="p2">The second control point.</param>
        /// <param name="p3">The end point of the curve.</param>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveBezier</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveBezier (Pen pen, CCVector2 p0, CCVector2 p1, CCVector2 p2, CCVector2 p3)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;
            _pathBuilder.AddBezier(p0, p1, p2, p3);

            DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a series of primitive Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveBeziers (Pen pen, IList<CCVector2> points, BezierType bezierType)
        {
            DrawPrimitiveBeziers(pen, points, 0, points.Count, bezierType, PathType.Open);
        }

        /// <summary>
        /// Adds a series of primitive Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveBeziers (Pen pen, IList<CCVector2> points, BezierType bezierType, PathType pathType)
        {
            DrawPrimitiveBeziers(pen, points, 0, points.Count, bezierType, pathType);
        }

        /// <summary>
        /// Adds a series of primitive Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="offset">The index of the first start point in the list.</param>
        /// <param name="count">The number of points to use.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveBeziers (Pen pen, IList<CCVector2> points, int offset, int count, BezierType bezierType)
        {
            DrawPrimitiveBeziers(pen, points, offset, count, bezierType, PathType.Open);
        }

        /// <summary>
        /// Adds a series of primitive Bezier paths to the batch of figures to be rendered.
        /// </summary>
        /// <param name="pen">The pen supplying the color to render the path with.</param>
        /// <param name="points">A list of Bezier points.</param>
        /// <param name="offset">The index of the first start point in the list.</param>
        /// <param name="count">The number of points to use.</param>
        /// <param name="bezierType">The type of Bezier curves to draw.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <remarks><para>For quadratic Bezier curves, the number of points defined by the parameters should be a multiple of 2 plus 1
        /// for open curves or 2 for closed curves.  For cubic Bezier curves, the number of points defined by the parameters should be 
        /// a multiple of 3 plus 1 for open curves or 3 for closed curves.  For each curve drawn after the first, the ending point of 
        /// the previous curve is used as the starting point.  For closed curves, the end point of the last curve is the start point
        /// of the first curve.</para></remarks>
        /// <exception cref="InvalidOperationException"><c>DrawPrimitiveBeziers</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void DrawPrimitiveBeziers (Pen pen, IList<CCVector2> points, int offset, int count, BezierType bezierType, PathType pathType)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points.Count < offset + count)
                throw new ArgumentOutOfRangeException("The offset and count are out of range for the given points argument.");

            _pathBuilder.Reset();
            _pathBuilder.CalculateLengths = pen.NeedsPathLength;

            switch (bezierType) {
                case BezierType.Quadratic:
                    for (int i = offset + 2; i < offset + count; i += 2)
                        _pathBuilder.AddBezier(points[i - 2], points[i - 1], points[i]);
                    if (pathType == PathType.Closed)
                        _pathBuilder.AddBezier(points[offset + count - 2], points[offset + count - 1], points[offset]);
                    break;

                case BezierType.Cubic:
                    for (int i = offset + 3; i < offset + count; i += 3)
                        _pathBuilder.AddBezier(points[i - 3], points[i - 2], points[i - 1], points[i]);
                    if (pathType == PathType.Closed)
                        _pathBuilder.AddBezier(points[offset + count - 3], points[offset + count - 2], points[offset + count - 1], points[offset]);
                    break;
            }

            switch (pathType) {
                case PathType.Open:
                    DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count, PathType.Open);
                    break;

                case PathType.Closed:
                    DrawPrimitivePath(pen, _pathBuilder.Buffer, 0, _pathBuilder.Count - 1, PathType.Closed);
                    break;
            }

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        private float ClampAngle (float angle)
        {
            if (angle < 0)
                angle += (float)(Math.Ceiling(angle / (Math.PI * -2)) * Math.PI * 2);
            else if (angle >= (Math.PI * 2))
                angle -= (float)(Math.Floor(angle / (Math.PI * 2)) * Math.PI * 2);

            return angle;
        }

        private float PointToAngle (CCVector2 center, CCVector2 point)
        {
            double angle = Math.Atan2(point.Y - center.Y, point.X - center.X);
            if (angle < 0)
                angle += Math.PI * 2;

            return (float)angle;
        }

        private int BuildArcGeometryBuffer (CCVector2 center, float radius, int subdivisions, float startAngle, float arcAngle)
        {
            float stopAngle = startAngle + arcAngle;

            startAngle = ClampAngle(startAngle);
            stopAngle = ClampAngle(stopAngle);

            List<CCVector2> unitCircle = CalculateCircleSubdivisions(subdivisions);

            float subLength = (float)(2 * Math.PI / subdivisions);

            CCVector2 unitStart = new CCVector2((float)Math.Cos(-startAngle), (float)Math.Sin(-startAngle));
            CCVector2 unitStop = new CCVector2((float)Math.Cos(-stopAngle), (float)Math.Sin(-stopAngle));

            int startIndex, stopIndex;
            int vertexCount = 0;

            if (arcAngle >= 0) {
                startIndex = (int)Math.Ceiling(startAngle / subLength);
                stopIndex = (int)Math.Floor(stopAngle / subLength);

                vertexCount = (stopIndex >= startIndex)
                    ? stopIndex - startIndex + 1
                    : (unitCircle.Count - startIndex) + stopIndex + 1;
            }
            else {
                startIndex = (int)Math.Floor(startAngle / subLength);
                stopIndex = (int)Math.Ceiling(stopAngle / subLength);

                vertexCount = (startIndex >= stopIndex)
                    ? startIndex - stopIndex + 1
                    : (unitCircle.Count - stopIndex) + startIndex + 1;
            }
            int bufIndex = 0;

            if (_geometryBuffer.Length < vertexCount + 2)
                Array.Resize(ref _geometryBuffer, (vertexCount + 2) * 2);

            if (arcAngle >= 0) {
                if ((startIndex * subLength) - startAngle > 0.005f) {
                    _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * (float)Math.Cos(-startAngle), center.Y - radius * (float)Math.Sin(-startAngle));
                    vertexCount++;
                }

                if (startIndex <= stopIndex) {
                    for (int i = startIndex; i <= stopIndex; i++)
                        _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y - radius * unitCircle[i].Y);
                }
                else {
                    for (int i = startIndex; i < unitCircle.Count; i++)
                        _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y - radius * unitCircle[i].Y);
                    for (int i = 0; i <= stopIndex; i++)
                        _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y - radius * unitCircle[i].Y);
                }

                if (stopAngle - (stopIndex * subLength) > 0.005f) {
                    _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * (float)Math.Cos(-stopAngle), center.Y - radius * (float)Math.Sin(-stopAngle));
                    vertexCount++;
                }
            }
            else {
                if (startAngle - (startIndex * subLength) > 0.005f) {
                    _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * (float)Math.Cos(-startAngle), center.Y - radius * (float)Math.Sin(-startAngle));
                    vertexCount++;
                }

                if (stopIndex <= startIndex) {
                    for (int i = startIndex; i >= stopIndex; i--)
                        _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y - radius * unitCircle[i].Y);
                }
                else {
                    for (int i = startIndex; i >= 0; i--)
                        _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y - radius * unitCircle[i].Y);
                    for (int i = unitCircle.Count - 1; i >= stopIndex; i--)
                        _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * unitCircle[i].X, center.Y - radius * unitCircle[i].Y);
                }

                if ((stopIndex * subLength) - stopAngle > 0.005f) {
                    _geometryBuffer[bufIndex++] = new CCVector2(center.X + radius * (float)Math.Cos(-stopAngle), center.Y - radius * (float)Math.Sin(-stopAngle));
                    vertexCount++;
                }
            }

            return vertexCount;
        }

        /// <summary>
        /// Adds a filled circle to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <exception cref="InvalidOperationException"><c>FillCircle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the circle is computed as <c>(radius / 1.5)</c>.</remarks>
        public void FillCircle (Brush brush, CCVector2 center, float radius)
        {
            FillCircle(brush, center, radius, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Adds a filled circle to the batch of figures to be rendered using the given number of subdivisions.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="subdivisions">The number of subdivisions to render the circle with.</param>
        /// <exception cref="InvalidOperationException"><c>FillCircle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillCircle (Brush brush, CCVector2 center, float radius, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (brush == null)
                throw new ArgumentNullException("brush");

            RequestBufferSpace(subdivisions + 1, subdivisions * 3);
            AddInfo(PrimitiveType.TriangleList, subdivisions + 1, subdivisions * 3, brush);

            BuildCircleGeometryBuffer(center, radius, subdivisions, true);

            int baseVertexIndex = _vertexBufferIndex;

            for (int i = 0; i < subdivisions; i++)
                AddVertex(_geometryBuffer[i], brush);

            AddVertex(new Vector2(center.X, center.Y), brush);

            for (int i = 0; i < subdivisions - 1; i++)
                AddTriangle(baseVertexIndex + subdivisions, baseVertexIndex + i + 1, baseVertexIndex + i);

            AddTriangle(baseVertexIndex + subdivisions, baseVertexIndex, baseVertexIndex + subdivisions - 1);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a filled ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <exception cref="InvalidOperationException"><c>FillEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(width, height) / 3.0.</remarks>
        public void FillEllipse (Brush brush, CCRect bound)
        {
            FillEllipse(brush, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, 0);
        }

        /// <summary>
        /// Adds a filled ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>FillEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(width, height) / 3.0.</remarks>
        public void FillEllipse (Brush brush, CCRect bound, float angle)
        {
            FillEllipse(brush, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, angle);
        }

        /// <summary>
        /// Adds a filled ellipse path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="bound">The bounding rectangle of the ellipse.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions (sides) to render the ellipse with.</param>
        /// <exception cref="InvalidOperationException"><c>FillEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillEllipse (Brush brush, CCRect bound, float angle, int subdivisions)
        {
            FillEllipse(brush, new CCVector2(bound.Center.X, bound.Center.Y), bound.Size.Width / 2f, bound.Size.Height / 2f, angle, subdivisions);
        }

        /// <summary>
        /// Adds a filled ellipse to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the llipse along the y-axis.</param>
        /// <exception cref="InvalidOperationException"><c>FillEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void FillEllipse (Brush brush, CCVector2 center, float xRadius, float yRadius)
        {
            FillEllipse(brush, center, xRadius, yRadius, 0);
        }

        /// <summary>
        /// Adds a filled ellipse to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the llipse along the y-axis.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>FillEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the ellipse is computed as max(xRadius, yRadius) / 1.5.</remarks>
        public void FillEllipse (Brush brush, CCVector2 center, float xRadius, float yRadius, float angle)
        {
            FillEllipse(brush, center, xRadius, yRadius, angle, DefaultSubdivisions(xRadius, yRadius));
        }

        /// <summary>
        /// Adds a filled ellipse to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the ellipse.</param>
        /// <param name="xRadius">The radius of the ellipse along the x-axis.</param>
        /// <param name="yRadius">The radius of the llipse along the y-axis.</param>
        /// <param name="angle">The angle to rotate the ellipse by in radians.  Positive values rotate clockwise.</param>
        /// <param name="subdivisions">The number of subdivisions to render the circle with.</param>
        /// <exception cref="InvalidOperationException"><c>FillEllipse</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillEllipse (Brush brush, CCVector2 center, float xRadius, float yRadius, float angle, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (brush == null)
                throw new ArgumentNullException("brush");

            RequestBufferSpace(subdivisions + 1, subdivisions * 3);
            AddInfo(PrimitiveType.TriangleList, subdivisions + 1, subdivisions * 3, brush);

            BuildEllipseGeometryBuffer(center, xRadius, yRadius, angle, subdivisions);

            int baseVertexIndex = _vertexBufferIndex;

            for (int i = 0; i < subdivisions; i++)
                AddVertex(_geometryBuffer[i], brush);

            AddVertex(new CCVector2(center.X, center.Y), brush);

            for (int i = 0; i < subdivisions - 1; i++)
                AddTriangle(baseVertexIndex + subdivisions, baseVertexIndex + i + 1, baseVertexIndex + i);

            AddTriangle(baseVertexIndex + subdivisions, baseVertexIndex, baseVertexIndex + subdivisions - 1);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a filled arc to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="arcType">Whether the arc is drawn as a segment or a sector.</param>
        /// <exception cref="InvalidOperationException"><c>FillArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>(radius / 1.5) * (arcAngle / 2PI)</c>.</remarks>
        public void FillArc (Brush brush, CCVector2 center, float radius, float startAngle, float arcAngle, ArcType arcType)
        {
            FillArc(brush, center, radius, startAngle, arcAngle, arcType, DefaultSubdivisions(radius));
        }

        /// <summary>
        /// Adds a filled arc to the batch of figures to be rendered using up to the given number of subdivisions.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="center">The center coordinate of the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle of the arc in radians, where 0 is 3 O'Clock.</param>
        /// <param name="arcAngle">The sweep of the arc in radians.  Positive values draw clockwise.</param>
        /// <param name="arcType">Whether the arc is drawn as a segment or a sector.</param>
        /// <param name="subdivisions">The number of subdivisions in the circle with the same radius as the arc.</param>
        /// <exception cref="InvalidOperationException"><c>FillArc</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>The number of subdivisions in the arc is computed as <c>subdivisions * (arcAngle / 2PI)</c>.</remarks>
        public void FillArc (Brush brush, CCVector2 center, float radius, float startAngle, float arcAngle, ArcType arcType, int subdivisions)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (brush == null)
                throw new ArgumentNullException("brush");

            int vertexCount = BuildArcGeometryBuffer(center, radius, subdivisions, startAngle, arcAngle);

            RequestBufferSpace(vertexCount + 1, (vertexCount - 1) * 3);
            AddInfo(PrimitiveType.TriangleList, vertexCount + 1, (vertexCount - 1) * 3, brush);

            int baseVertexIndex = _vertexBufferIndex;

            for (int i = 0; i < vertexCount; i++)
                AddVertex(_geometryBuffer[i], brush);

            switch (arcType) {
                case ArcType.Sector:
                    AddVertex(new CCVector2(center.X, center.Y), brush);
                    break;
                case ArcType.Segment:
                    AddVertex(new CCVector2((_geometryBuffer[0].X + _geometryBuffer[vertexCount - 1].X) / 2,
                        (_geometryBuffer[0].Y + _geometryBuffer[vertexCount - 1].Y) / 2), brush);
                    break;
            }

            if (arcAngle < 0)
            {
                for (int i = 0; i < vertexCount - 1; i++)
                    AddTriangle(baseVertexIndex + vertexCount, baseVertexIndex + i + 1, baseVertexIndex + i);
            }
            else
            {
                for (int i = vertexCount - 1; i > 0; i--)
                    AddTriangle(baseVertexIndex + vertexCount, baseVertexIndex + i - 1, baseVertexIndex + i);
            }

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a filled rectangle to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="rect">The rectangle to be rendered.</param>
        /// <exception cref="InvalidOperationException"><c>FillRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillRectangle (Brush brush, CCRect rect)
        {
            FillRectangle(brush, rect, 0);
        }

        /// <summary>
        /// Adds a filled rectangle to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="rect">The rectangle to be rendered.</param>
        /// <param name="angle">The angle to rotate the rectangle by around its center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>FillRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillRectangle (Brush brush, CCRect rect, float angle)
        {
            _geometryBuffer[0] = new CCVector2(rect.MinX, rect.MaxY);
            _geometryBuffer[1] = new CCVector2(rect.MaxX, rect.MaxY);
            _geometryBuffer[2] = new CCVector2(rect.MaxX, rect.MinY);
            _geometryBuffer[3] = new CCVector2(rect.MinX, rect.MinY);

            FillQuad(brush, _geometryBuffer, 0, angle);
        }

        /// <summary>
        /// Adds a filled rectangle to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="location">The top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <exception cref="InvalidOperationException"><c>FillRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillRectangle (Brush brush, CCVector2 location, float width, float height)
        {
            FillRectangle(brush, location, width, height, 0);
        }

        /// <summary>
        /// Adds a filled rectangle to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="location">The top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="angle">The angle to rotate the rectangle by around its center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>FillRectangle</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillRectangle (Brush brush, CCVector2 location, float width, float height, float angle)
        {
            _geometryBuffer[0] = location;
            _geometryBuffer[1] = new CCVector2(location.X + width, location.Y);
            _geometryBuffer[2] = new CCVector2(location.X + width, location.Y + height);
            _geometryBuffer[3] = new CCVector2(location.X, location.Y + height);

            FillQuad(brush, _geometryBuffer, 0, angle);
        }

        /// <summary>
        /// Adds a filled quadrilateral to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="points">An array containing the coordinates of the quad.</param>
        /// <param name="offset">The offset into the points array.</param>
        /// <exception cref="InvalidOperationException"><c>FillQuad</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillQuad (Brush brush, CCVector2[] points, int offset)
        {
            FillQuad(brush, points, offset, 0);
        }

        /// <summary>
        /// Adds a filled quadrilateral to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="points">An array containing the coordinates of the quad.</param>
        /// <param name="offset">The offset into the points array.</param>
        /// <param name="angle">The angle to rotate the quad around its weighted center in radians.  Positive values rotate clockwise.</param>
        /// <exception cref="InvalidOperationException"><c>FillQuad</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        public void FillQuad (Brush brush, CCVector2[] points, int offset, float angle)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < offset + 4)
                throw new ArgumentException("Points array is too small for the given offset.");

            RequestBufferSpace(4, 6);
            AddInfo(PrimitiveType.TriangleList, 4, 6, brush);

            int baseVertexIndex = _vertexBufferIndex;

            if (points != _geometryBuffer)
                Array.Copy(points, _geometryBuffer, 4);

            if (angle != 0)
            {
                float centerX = (_geometryBuffer[0].X + _geometryBuffer[1].X + _geometryBuffer[2].X + _geometryBuffer[3].X) / 4;
                float centerY = (_geometryBuffer[0].Y + _geometryBuffer[1].Y + _geometryBuffer[2].Y + _geometryBuffer[3].Y) / 4;
                CCVector2 center = new CCVector2(centerX, centerY);

                Matrix transform = Matrix.CreateRotationZ(angle);
                transform.Translation = new Vector3(center.X, center.Y, 0);

                var affineTransform = transform.toCCAffineTransform();

                for (int i = 0; i < 4; i++)
                    _geometryBuffer[i] = CCVector2.Transform(_geometryBuffer[i] - center, affineTransform);
            }

            for (int i = 0; i < 4; i++)
                AddVertex(_geometryBuffer[i], brush);

            AddTriangle(baseVertexIndex + 0, baseVertexIndex + 1, baseVertexIndex + 3);
            AddTriangle(baseVertexIndex + 1, baseVertexIndex + 2, baseVertexIndex + 3);

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Adds a filled region enclosed by the given multisegment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="points">The list of points that make up the multisegment path enclosing the region.</param>
        /// <exception cref="InvalidOperationException"><c>FillPath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>Paths should be created with a clockwise winding order, or the resulting geometry will be backface-culled.</remarks>
        public void FillPath (Brush brush, IList<CCVector2> points)
        {
            FillPath(brush, points, 0, points.Count);
        }

        /// <summary>
        /// Adds a filled region enclosed by the given multisegment path to the batch of figures to be rendered.
        /// </summary>
        /// <param name="brush">The brush to render the shape with.</param>
        /// <param name="points">The list of points that make up the multisegment path enclosing the region.</param>
        /// <param name="offset">The offset into the <paramref name="points"/> list to begin rendering.</param>
        /// <param name="count">The number of points that should be rendered, starting from <paramref name="offset"/>.</param>
        /// <exception cref="InvalidOperationException"><c>FillPath</c> was called, but <see cref="Begin()"/> has not yet been called.</exception>
        /// <remarks>Paths should be created with a clockwise winding order, or the resulting geometry will be backface-culled.</remarks>
        public void FillPath (Brush brush, IList<CCVector2> points, int offset, int count)
        {
            if (!_inDraw)
                throw new InvalidOperationException();
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (_triangulator == null)
                _triangulator = new Triangulator();

            _triangulator.Triangulate(points, offset, count);

            RequestBufferSpace(count, _triangulator.ComputedIndexCount);
            AddInfo(PrimitiveType.TriangleList, count, _triangulator.ComputedIndexCount, brush);

            int baseVertexIndex = _vertexBufferIndex;

            for (int i = 0; i < count; i++) {
                AddVertex(points[offset + i], brush);
            }

            for (int i = 0; i < _triangulator.ComputedIndexCount; i++) {
                _indexBuffer[_indexBufferIndex + i] = (short)(_triangulator.ComputedIndexes[i] + baseVertexIndex);
            }

            _indexBufferIndex += _triangulator.ComputedIndexCount;

            if (_sortMode == DrawSortMode.Immediate)
                FlushBuffer();
        }

        /// <summary>
        /// Immediatley renders a <see cref="DrawCache"/> object.
        /// </summary>
        /// <param name="cache">A <see cref="DrawCache"/> object.</param>
        /// <remarks>Any previous unflushed geometry will be rendered first.</remarks>
        public void DrawCache(DrawCache cache)
        {
            //if (_sortMode != DrawSortMode.Immediate)
            //    SetRenderState();

            FlushBuffer();

            cache.Render(this, _defaultTexture);
        }

        //private void SetRenderState ()
        //{
        //    _device.BlendState = (_blendState != null)
        //        ? _blendState : BlendState.AlphaBlend;

        //    _device.DepthStencilState = (_depthStencilState != null)
        //        ? _depthStencilState : DepthStencilState.None;

        //    _device.RasterizerState = (_rasterizerState != null)
        //        ? _rasterizerState : RasterizerState.CullCounterClockwise;

        //    _device.SamplerStates[0] = (_samplerState != null)
        //        ? _samplerState : SamplerState.PointWrap;

        //    _standardEffect.Projection = Matrix.CreateOrthographicOffCenter(0, _device.Viewport.Width, _device.Viewport.Height, 0, -1, 1);
        //    _standardEffect.World = _transform;
        //    _standardEffect.CurrentTechnique.Passes[0].Apply();

        //    if (_effect != null)
        //        _effect.CurrentTechnique.Passes[0].Apply();
        //}

        private void AddMiteredJoint (ref JoinSample js, Pen pen, PenWorkspace ws)
        {
            InsetOutsetCount vioCount = pen.ComputeMiter(ref js, ws);

            AddVertex(ws.XYInsetBuffer[0], pen.ColorAt(ws.UVInsetBuffer[0], ws.PathLengthScale), pen);
            AddVertex(ws.XYOutsetBuffer[0], pen.ColorAt(ws.UVOutsetBuffer[0], ws.PathLengthScale), pen);
        }

        private void AddStartPoint (CCVector2 a, CCVector2 b, Pen pen, PenWorkspace ws)
        {
            pen.ComputeStartPoint(a, b, ws);

            AddVertex(ws.XYBuffer[1], pen.ColorAt(ws.UVBuffer[1], ws.PathLengthScale), pen);
            AddVertex(ws.XYBuffer[0], pen.ColorAt(ws.UVBuffer[0], ws.PathLengthScale), pen);
        }

        private void AddEndPoint (CCVector2 a, CCVector2 b, Pen pen, PenWorkspace ws)
        {
            pen.ComputeEndPoint(a, b, ws);

            AddVertex(ws.XYBuffer[0], pen.ColorAt(ws.UVBuffer[0], ws.PathLengthScale), pen);
            AddVertex(ws.XYBuffer[1], pen.ColorAt(ws.UVBuffer[1], ws.PathLengthScale), pen);
        }

        private void AddInfo (PrimitiveType primitiveType, int vertexCount, int indexCount, Brush brush)
        {
            AddInfo(primitiveType, vertexCount, indexCount, brush != null ? brush.Texture : _defaultTexture);
        }

        private void AddInfo (PrimitiveType primitiveType, int vertexCount, int indexCount, CCTexture2D texture)
        {
            _infoBuffer[_infoBufferIndex].Primitive = primitiveType;
            _infoBuffer[_infoBufferIndex].Texture = texture ?? _defaultTexture;
            _infoBuffer[_infoBufferIndex].IndexCount = indexCount;
            _infoBuffer[_infoBufferIndex].VertexCount = vertexCount;
            _infoBufferIndex++;
        }

        private void AddClosedPath (CCVector2[] points, int offset, int count, Pen pen, PenWorkspace ws)
        {
            RequestBufferSpace(count * 2, count * 6);

            AddInfo(PrimitiveType.TriangleList, count * 2, count * 6, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            JoinSample js = new JoinSample(CCVector2.Zero, points[offset + 0], points[offset + 1]);

            for (int i = 0; i < count - 2; i++) {
                js.Advance(points[offset + i + 2]);
                AddMiteredJoint(ref js, pen, ws);
            }

            js.Advance(points[offset + 0]);
            AddMiteredJoint(ref js, pen, ws);
            js.Advance(points[offset + 1]);
            AddMiteredJoint(ref js, pen, ws);

            for (int i = 0; i < count - 1; i++) {
                AddSegment(baseVertexIndex + i * 2, baseVertexIndex + (i + 1) * 2);
            }

            AddSegment(baseVertexIndex + (count - 1) * 2, baseVertexIndex + 0);
        }

        private void AddPath (CCVector2[] points, int offset, int count, Pen pen, PenWorkspace ws)
        {
            RequestBufferSpace(count * 2, (count - 1) * 6);
            _ws.ResetWorkspace(pen);

            AddInfo(PrimitiveType.TriangleList, count * 2, (count - 1) * 6, pen.Brush);

            int baseVertexIndex = _vertexBufferIndex;

            AddStartPoint(points[offset + 0], points[offset + 1], pen, _ws);

            JoinSample js = new JoinSample(CCVector2.Zero, points[offset + 0], points[offset + 1]);

            for (int i = 0; i < count - 2; i++) {
                js.Advance(points[offset + i + 2]);
                AddMiteredJoint(ref js, pen, ws);
            }

            AddEndPoint(points[offset + count - 2], points[offset + count - 1], pen, _ws);

            for (int i = 0; i < count - 1; i++)
                AddSegment(baseVertexIndex + i * 2, baseVertexIndex + (i + 1) * 2);
        }

        private void AddVertex(Vector2 position, Pen pen)
        {
            AddVertex(position, pen.Color, pen);
        }

        private void AddVertex(Vector2 position, Color color, Pen pen)
        {
            VertexPositionColorTexture vertex = new VertexPositionColorTexture();
            vertex.Position = new Vector3(position, 0);
            vertex.Color = color;

            if (pen.Brush != null && pen.Brush.Texture != null && pen.Brush.Texture.XNATexture != null)
            {
                Texture2D tex = pen.Brush.Texture.XNATexture;
                vertex.TextureCoordinate = new Vector2(position.X / tex.Width, position.Y / tex.Height);
                vertex.TextureCoordinate = Vector2.Transform(vertex.TextureCoordinate, pen.Brush.Transform);
                vertex.Color *= pen.Brush.Alpha;
            }
            else
            {
                vertex.TextureCoordinate = new Vector2(position.X, position.Y);
            }

            _vertexBuffer[_vertexBufferIndex++] = vertex;
        }

        private void AddVertex(CCVector2 position, Color color, Pen pen)
        {
            VertexPositionColorTexture vertex = new VertexPositionColorTexture();
            vertex.Position = new Vector3(position.X, position.Y, 0);
            vertex.Color = color;

            if (pen.Brush != null && pen.Brush.Texture != null && pen.Brush.Texture.XNATexture != null)
            {
                Texture2D tex = pen.Brush.Texture.XNATexture;
                vertex.TextureCoordinate = new Vector2(position.X / tex.Width, position.Y / tex.Height);
                vertex.TextureCoordinate = Vector2.Transform(vertex.TextureCoordinate, pen.Brush.Transform);
                vertex.Color *= pen.Brush.Alpha;
            }
            else
            {
                vertex.TextureCoordinate = new Vector2(position.X, position.Y);
            }

            _vertexBuffer[_vertexBufferIndex++] = vertex;
        }

        private void AddVertex(Vector2 position, Brush brush)
        {
            VertexPositionColorTexture vertex = new VertexPositionColorTexture();
            vertex.Position = new Vector3(position, 0);
            vertex.Color = brush.Color;

            if (brush != null && brush.Texture != null && brush.Texture.XNATexture != null)
            {
                Texture2D tex = brush.Texture.XNATexture;
                vertex.TextureCoordinate = new Vector2(position.X / tex.Width, position.Y / tex.Height);
                vertex.TextureCoordinate = Vector2.Transform(vertex.TextureCoordinate, brush.Transform);
                vertex.Color *= brush.Alpha;
            }
            else
            {
                vertex.TextureCoordinate = new Vector2(position.X, position.Y);
            }

            _vertexBuffer[_vertexBufferIndex++] = vertex;
        }

        private void AddVertex(CCVector2 position, Brush brush)
        {
            VertexPositionColorTexture vertex = new VertexPositionColorTexture();
            vertex.Position = new Vector3(position.X, position.Y, 0);
            vertex.Color = brush.Color;

            if (brush != null && brush.Texture != null && brush.Texture.XNATexture != null)
            {
                Texture2D tex = brush.Texture.XNATexture;
                vertex.TextureCoordinate = new Vector2(position.X / tex.Width, position.Y / tex.Height);
                vertex.TextureCoordinate = Vector2.Transform(vertex.TextureCoordinate, brush.Transform);
                vertex.Color *= brush.Alpha;
            }
            else
            {
                vertex.TextureCoordinate = new Vector2(position.X, position.Y);
            }

            _vertexBuffer[_vertexBufferIndex++] = vertex;
        }

        private void AddSegment(int startVertexIndex, int endVertexIndex)
        {
            _indexBuffer[_indexBufferIndex++] = (short)(startVertexIndex + 0);
            _indexBuffer[_indexBufferIndex++] = (short)(startVertexIndex + 1);
            _indexBuffer[_indexBufferIndex++] = (short)(endVertexIndex + 1);
            _indexBuffer[_indexBufferIndex++] = (short)(startVertexIndex + 0);
            _indexBuffer[_indexBufferIndex++] = (short)(endVertexIndex + 1);
            _indexBuffer[_indexBufferIndex++] = (short)(endVertexIndex + 0);
        }

        private void AddPrimitiveLineSegment (int startVertexIndex, int endVertexIndex)
        {
            _indexBuffer[_indexBufferIndex++] = (short)startVertexIndex;
            _indexBuffer[_indexBufferIndex++] = (short)endVertexIndex;
        }

        private void AddTriangle (int a, int b, int c)
        {
            _indexBuffer[_indexBufferIndex++] = (short)a;
            _indexBuffer[_indexBufferIndex++] = (short)b;
            _indexBuffer[_indexBufferIndex++] = (short)c;
        }

        private void FlushBuffer ()
        {
            int vertexOffset = 0;
            int indexOffset = 0;
            int vertexCount = 0;
            int indexCount = 0;
            CCTexture2D texture = null;
            PrimitiveType primitive = PrimitiveType.TriangleList;

            for (int i = 0; i < _infoBufferIndex; i++) {
                if (texture != _infoBuffer[i].Texture || primitive != _infoBuffer[i].Primitive) {
                    if (indexCount > 0) {
                        for (int j = 0; j < indexCount; j++) {
                            _indexBuffer[indexOffset + j] -= (short)vertexOffset;
                        }

                        RenderBatch(primitive, indexOffset, indexCount, vertexOffset, vertexCount, texture);
                    }

                    vertexOffset += vertexCount;
                    indexOffset += indexCount;
                    vertexCount = 0;
                    indexCount = 0;
                    texture = _infoBuffer[i].Texture;
                    primitive = _infoBuffer[i].Primitive;
                }

                vertexCount += _infoBuffer[i].VertexCount;
                indexCount += _infoBuffer[i].IndexCount;
            }

            if (indexCount > 0) {
                for (int j = 0; j < indexCount; j++) {
                    _indexBuffer[indexOffset + j] -= (short)vertexOffset;
                }

                RenderBatch(primitive, indexOffset, indexCount, vertexOffset, vertexCount, texture);
            }

            ClearInfoBuffer();

            _infoBufferIndex = 0;
            _indexBufferIndex = 0;
            _vertexBufferIndex = 0;
        }

        private void RenderBatch (PrimitiveType primitiveType, int indexOffset, int indexCount, int vertexOffset, int vertexCount, CCTexture2D texture)
        {
            
            var geoInstance = CreateGeometryInstance(vertexCount, indexCount, primitiveType);
            var packet = geoInstance.GeometryPacket;
            packet.Texture = texture;

            switch (primitiveType) {
                case PrimitiveType.LineList:
                    //_device.DrawUserIndexedPrimitives(primitiveType, _vertexBuffer, vertexOffset, vertexCount, _indexBuffer, indexOffset, indexCount / 2);
                    for (int io = indexOffset, ic = 0; ic < indexCount; ic++, io++)
                    {
                        packet.Indicies[ic] = _indexBuffer[io];
                    }
                    for (int tli = 0, tlo = vertexOffset; tli < vertexCount; tli++, tlo++)
                    {

                        var vertext = _vertexBuffer[tlo];
                        var ccvertext = packet.Vertices[tli];
                        ccvertext.Colors = ccvertext.Colors = vertext.Color.ToCCColor4B();
                        
                        //ccvertext.TexCoords.U = vertext.TextureCoordinate.X;
                        //ccvertext.TexCoords.V = vertext.TextureCoordinate.Y;
                        ccvertext.Vertices = new CCVertex3F(vertext.Position.X, vertext.Position.Y, vertext.Position.Z);
                        packet.Vertices[tli] = ccvertext;
                    }
                    break;
                case PrimitiveType.TriangleList:
                    //_device.DrawUserIndexedPrimitives(primitiveType, _vertexBuffer, vertexOffset, vertexCount, _indexBuffer, indexOffset, indexCount / 3);
                    for (int io = indexOffset, ic = 0; ic < indexCount; ic++, io++)
                    {
                        packet.Indicies[ic] = _indexBuffer[io];
                    }
                    for (int tli = 0, tlo = vertexOffset; tli < vertexCount; tli++, tlo++)
                    {

                        var vertext = _vertexBuffer[tlo];
                        var ccvertext = packet.Vertices[tli];
                        ccvertext.Colors = vertext.Color.ToCCColor4B();

                        ccvertext.TexCoords.U = vertext.TextureCoordinate.X;
                        ccvertext.TexCoords.V = vertext.TextureCoordinate.Y;
                        ccvertext.Vertices = new CCVertex3F(vertext.Position.X, vertext.Position.Y, vertext.Position.Z);
                        packet.Vertices[tli] = ccvertext;


                    }

                    break;
            }
        }

        internal void RenderBatch(PrimitiveType primitiveType, 
            int indexOffset, int indexCount, short[] _indexBuffer,
            int vertexOffset, int vertexCount, VertexPositionColorTexture[] _vertexBuffer,
            CCTexture2D texture)
        {

            var geoInstance = CreateGeometryInstance(vertexCount, indexCount, primitiveType);
            var packet = geoInstance.GeometryPacket;
            packet.Texture = texture;

            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    //_device.DrawUserIndexedPrimitives(primitiveType, _vertexBuffer, vertexOffset, vertexCount, _indexBuffer, indexOffset, indexCount / 2);
                    for (int io = indexOffset, ic = 0; ic < indexCount; ic++, io++)
                    {
                        packet.Indicies[ic] = _indexBuffer[io];
                    }
                    for (int tli = 0, tlo = vertexOffset; tli < vertexCount; tli++, tlo++)
                    {

                        var vertext = _vertexBuffer[tlo];
                        var ccvertext = packet.Vertices[tli];
                        ccvertext.Colors = new CCColor4B(vertext.Color.R, vertext.Color.G, vertext.Color.B, vertext.Color.A);
                        ccvertext.Vertices = new CCVertex3F(vertext.Position.X, vertext.Position.Y, vertext.Position.Z);
                        packet.Vertices[tli] = ccvertext;
                    }
                    break;
                case PrimitiveType.TriangleList:
                    //_device.DrawUserIndexedPrimitives(primitiveType, _vertexBuffer, vertexOffset, vertexCount, _indexBuffer, indexOffset, indexCount / 3);
                    for (int io = indexOffset, ic = 0; ic < indexCount; ic++, io++)
                    {
                        packet.Indicies[ic] = _indexBuffer[io];
                    }
                    for (int tli = 0, tlo = vertexOffset; tli < vertexCount; tli++, tlo++)
                    {

                        var vertext = _vertexBuffer[tlo];
                        var ccvertext = packet.Vertices[tli];
                        ccvertext.Colors = vertext.Color.ToCCColor4B();

                        ccvertext.TexCoords.U = vertext.TextureCoordinate.X;
                        ccvertext.TexCoords.V = vertext.TextureCoordinate.Y;
                        ccvertext.Vertices = new CCVertex3F(vertext.Position.X, vertext.Position.Y, vertext.Position.Z);
                        packet.Vertices[tli] = ccvertext;


                    }

                    break;
            }
        }

        private void ClearInfoBuffer()
        {
            for (int i = 0; i < _infoBufferIndex; i++)
                _infoBuffer[i].Texture = null;
        }

        // TODO: Manage buffer overflow without requiring flush.
        private void RequestBufferSpace (int newVertexCount, int newIndexCount)
        {
            if (_indexBufferIndex + newIndexCount > short.MaxValue)
            {
                //if (_sortMode != DrawSortMode.Immediate)
                //    SetRenderState();
                FlushBuffer();
            }

            if (_infoBufferIndex + 1 > _infoBuffer.Length)
            {
                Array.Resize(ref _infoBuffer, _infoBuffer.Length * 2);
            }

            if (_indexBufferIndex + newIndexCount >= _indexBuffer.Length)
            {
                Array.Resize(ref _indexBuffer, _indexBuffer.Length * 2);
            }

            if (_vertexBufferIndex + newVertexCount >= _vertexBuffer.Length)
            {
                Array.Resize(ref _vertexBuffer, _vertexBuffer.Length * 2);
            }
        }

        private Dictionary<int, List<CCVector2>> _circleCache = new Dictionary<int, List<CCVector2>>();

        private List<CCVector2> CalculateCircleSubdivisions (int divisions)
        {
            if (_circleCache.ContainsKey(divisions))
                return _circleCache[divisions];

            if (divisions < 0)
                throw new ArgumentOutOfRangeException("divisions");

            double slice = Math.PI * 2 / divisions;

            List<CCVector2> unitCircle = new List<CCVector2>();

            for (int i = 0; i < divisions; i++) {
                double angle = -slice * i;
                unitCircle.Add(new CCVector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
            }

            _circleCache.Add(divisions, unitCircle);
            return unitCircle;
        }

        private static int DefaultSubdivisions (float radius)
        {
            return Math.Max(8, (int)Math.Ceiling(radius / 1.5f));
        }

        private static int DefaultSubdivisions (float xRadius, float yRadius)
        {
            return Math.Max(8, (int)Math.Ceiling(Math.Max(xRadius, yRadius) / 1.5f));
        }
    }

    
}
