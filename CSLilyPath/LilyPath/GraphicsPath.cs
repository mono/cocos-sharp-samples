using System.Collections.Generic;
using System;
using LilyPath.Utility;
using CocosSharp;

namespace LilyPath
{
    /// <summary>
    /// Represents computed path geometry.
    /// </summary>
    public interface IGraphicsPath
    {
        /// <summary>
        /// The number of vertices in the computed geometry.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// The number of vertex indexes in the computed geometry.
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// The raw vertex data of the computed geometry.
        /// </summary>
        CCVector2[] VertexPositionData { get; }

        /// <summary>
        /// The raw texture data of the computed geometry.
        /// </summary>
        CCVector2[] VertexTextureData { get; }

        /// <summary>
        /// The raw color data of the computed geometry.
        /// </summary>
        CCColor4B[] VertexColorData { get; }

        /// <summary>
        /// The raw index data of the computed geometry.
        /// </summary>
        short[] IndexData { get; }

        /// <summary>
        /// The <see cref="Pen"/> used to compute the geometry.
        /// </summary>
        Pen Pen { get; }
    }

    /// <summary>
    /// Represents a stroked path.
    /// </summary>
    public class GraphicsPath : IGraphicsPath
    {
        private static GraphicsPath[] _emptyOutlinePaths = new GraphicsPath[0];
        private static ZeroList _zeroList = new ZeroList();

        private Pen _pen;
        private StrokeType _strokeType;

        private int _pointCount;
        private int _vertexBufferIndex;
        private int _indexBufferIndex;

        private CCVector2[] _positionData;
        private CCVector2[] _textureData;
        private CCColor4B[] _colorData;
        private short[] _indexData;

        private bool[] _jointCCW;

        private GraphicsPath[] _outlinePaths;

        /// <summary>
        /// Create an empty path with a given <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen"></param>
        public GraphicsPath (Pen pen)
        {
            _pen = pen;
        }

        /// <summary>
        /// Compute a stroked open path given a set of points and a <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        public GraphicsPath (Pen pen, IList<CCVector2> points)
            : this(pen, points, PathType.Open, 0, points.Count)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        public GraphicsPath (Pen pen, IList<CCVector2> points, PathType pathType)
            : this(pen, points, pathType, 0, points.Count)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <param name="offset">The offset into the list of points that starts the path.</param>
        /// <param name="count">The number of points in the path.</param>
        public GraphicsPath (Pen pen, IList<CCVector2> points, PathType pathType, int offset, int count)
            : this(pen)
        {
            _pointCount = count;
            _strokeType = StrokeType.Fill;

            switch (pathType) {
                case PathType.Open:
                    CompileOpenPath(points, null, offset, count, null);
                    break;

                case PathType.Closed:
                    CompileClosedPath(points, null, offset, count, null);
                    break;
            }
        }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="lengths">The lengths of each point from the path start.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <param name="offset">The offset into the list of points that starts the path.</param>
        /// <param name="count">The number of points in the path.</param>
        public GraphicsPath (Pen pen, IList<CCVector2> points, IList<float> lengths, PathType pathType, int offset, int count)
            : this(pen)
        {
            _pointCount = count;
            _strokeType = StrokeType.Fill;

            switch (pathType) {
                case PathType.Open:
                    CompileOpenPath(points, lengths, offset, count, null);
                    break;

                case PathType.Closed:
                    CompileClosedPath(points, lengths, offset, count, null);
                    break;
            }
        }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a path and outline <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="outlinePen">The pen to stroke the outline of the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        public GraphicsPath (Pen pen, Pen outlinePen, IList<CCVector2> points)
            : this(pen, outlinePen, points, PathType.Open, 0, points.Count, StrokeType.Both)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a path and outline <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="outlinePen">The pen to stroke the outline of the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="strokeType">Whether to stroke just the path, the outline, or both.</param>
        public GraphicsPath (Pen pen, Pen outlinePen, IList<CCVector2> points, StrokeType strokeType)
            : this(pen, outlinePen, points, PathType.Open, 0, points.Count, strokeType)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a path and outline <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="outlinePen">The pen to stroke the outline of the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        public GraphicsPath (Pen pen, Pen outlinePen, IList<CCVector2> points, PathType pathType)
            : this(pen, outlinePen, points, pathType, 0, points.Count, StrokeType.Both)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a path and outline <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="outlinePen">The pen to stroke the outline of the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <param name="strokeType">Whether to stroke just the path, the outline, or both.</param>
        public GraphicsPath (Pen pen, Pen outlinePen, IList<CCVector2> points, PathType pathType, StrokeType strokeType)
            : this(pen, outlinePen, points, pathType, 0, points.Count, strokeType)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a path and outline <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="outlinePen">The pen to stroke the outline of the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <param name="offset">The offset into the list of points that starts the path.</param>
        /// <param name="count">The number of points in the path.</param>
        public GraphicsPath (Pen pen, Pen outlinePen, IList<CCVector2> points, PathType pathType, int offset, int count)
            : this(pen, outlinePen, points, pathType, offset, count, StrokeType.Both)
        { }

        /// <summary>
        /// Compute a stroked open or closed path given a set of points and a path and outline <see cref="Pen"/>.
        /// </summary>
        /// <param name="pen">The pen to stroke the path with.</param>
        /// <param name="outlinePen">The pen to stroke the outline of the path with.</param>
        /// <param name="points">The points making up the ideal path.</param>
        /// <param name="pathType">Whether the path is open or closed.</param>
        /// <param name="offset">The offset into the list of points that starts the path.</param>
        /// <param name="count">The number of points in the path.</param>
        /// <param name="strokeType">Whether to stroke just the path, the outline, or both.</param>
        public GraphicsPath (Pen pen, Pen outlinePen, IList<CCVector2> points, PathType pathType, int offset, int count, StrokeType strokeType)
            : this(pen)
        {
            _pointCount = count;
            _strokeType = strokeType;

            switch (pathType) {
                case PathType.Open:
                    CompileOpenPath(points, null, offset, count, outlinePen);
                    break;

                case PathType.Closed:
                    CompileClosedPath(points, null, offset, count, outlinePen);
                    break;
            }
        }

        /// <summary>
        /// Gets the outline paths that have been generated for this path.
        /// </summary>
        public GraphicsPath[] OutlinePaths
        {
            get { return _outlinePaths ?? _emptyOutlinePaths; }
        }

        #region IGraphicsPath Interface

        /// <inherit />
        public int IndexCount
        {
            get { return _indexBufferIndex; }
        }

        /// <inherit />
        public int VertexCount
        {
            get { return _vertexBufferIndex; }
        }

        /// <inherit />
        public CCVector2[] VertexPositionData
        {
            get { return _positionData; }
        }

        /// <inherit />
        public CCVector2[] VertexTextureData
        {
            get { return _textureData; }
        }

        /// <inherit />
        public CCColor4B[] VertexColorData
        {
            get { return _colorData; }
        }

        /// <inherit />
        public short[] IndexData
        {
            get { return _indexData; }
        }

        /// <inherit />
        public Pen Pen
        {
            get { return _pen; }
        }

        #endregion

        private void InitializeBuffers (int pointCount)
        {
            _jointCCW = new bool[pointCount];

            int vertexCount = Pen.MaximumVertexCount(pointCount);
            int indexCount = Pen.MaximumIndexCount(pointCount);

            _indexData = new short[indexCount];
            _positionData = new CCVector2[vertexCount];

            if (_pen.Brush != null) {
                if (_pen.Brush.Color != null)
                    _colorData = new CCColor4B[vertexCount];

                if (_pen.Brush.Texture != null)
                    _textureData = new CCVector2[vertexCount];
            }
        }

        private void CompileOpenPath (IList<CCVector2> points, IList<float> accumLengths, int offset, int count, Pen outlinePen)
        {
            if (_strokeType != StrokeType.Outline)
                InitializeBuffers(count);

            IList<float> lengths = accumLengths ?? _zeroList;

            Buffer<CCVector2> insetBuffer = null;
            Buffer<CCVector2> outsetBuffer = null;

            if (outlinePen != null && _strokeType != StrokeType.Fill) {
                insetBuffer = Pools<Buffer<CCVector2>>.Obtain();
                outsetBuffer = Pools<Buffer<CCVector2>>.Obtain();

                int vCount = _positionData != null ? _positionData.Length : _pen.MaximumVertexCount(count);

                insetBuffer.EnsureCapacity(vCount);
                outsetBuffer.EnsureCapacity(vCount);
            }

            PenWorkspace ws = Pools<PenWorkspace>.Obtain();
            ws.ResetWorkspace(_pen);
            ws.PathLength = lengths[offset + count - 1];

            int vPrevCount = 0;
            int vNextCount = AddStartPoint(0, points[offset + 0], points[offset + 1], ws, insetBuffer);

            if (insetBuffer != null)
                Array.Reverse(insetBuffer.Data, 0, insetBuffer.Index);

            JoinSample joinSample = new JoinSample(CCVector2.Zero, points[offset + 0], points[offset + 1], 0, lengths[offset + 0], lengths[offset + 1]);

            for (int i = 0; i < count - 2; i++) {
                joinSample.Advance(points[offset + i + 2], lengths[offset + i + 2]);

                vPrevCount = vNextCount;
                vNextCount = AddJoint(i + 1, ref joinSample, ws, insetBuffer, outsetBuffer);
                if (_strokeType != StrokeType.Outline)
                    AddSegment(_vertexBufferIndex - vNextCount - vPrevCount, vPrevCount, _jointCCW[i], _vertexBufferIndex - vNextCount, vNextCount, _jointCCW[i + 1]);
            }

            /*for (int i = 0; i < count - 2; i++) {
                int i0 = offset + i;
                int i1 = i0 + 1;
                int i2 = i0 + 2;

                vPrevCount = vNextCount;
                vNextCount = AddJoint(i + 1, points[i0], points[i1], points[i2], lengths[i0], lengths[i1], lengths[i2], ws, insetBuffer, outsetBuffer);
                if (_strokeType != StrokeType.Outline)
                    AddSegment(_vertexBufferIndex - vNextCount - vPrevCount, vPrevCount, _jointCCW[i], _vertexBufferIndex - vNextCount, vNextCount, _jointCCW[i + 1]);
            }*/

            vPrevCount = vNextCount;
            vNextCount = AddEndPoint(count - 1, points[offset + count - 2], points[offset + count - 1], ws, insetBuffer);
            if (_strokeType != StrokeType.Outline)
                AddSegment(_vertexBufferIndex - vNextCount - vPrevCount, vPrevCount, _jointCCW[count - 2], _vertexBufferIndex - vNextCount, vNextCount, _jointCCW[count - 1]);

            if (insetBuffer != null)
                Array.Reverse(insetBuffer.Data, 0, insetBuffer.Index);

            Pools<PenWorkspace>.Release(ws);

            if (outlinePen != null && _strokeType != StrokeType.Fill) {
                Buffer<CCVector2> mergedBuffer = Pools<Buffer<CCVector2>>.Obtain();
                mergedBuffer.EnsureCapacity(insetBuffer.Index + outsetBuffer.Index);

                Array.Copy(insetBuffer.Data, 0, mergedBuffer.Data, 0, insetBuffer.Index);
                Array.Copy(outsetBuffer.Data, 0, mergedBuffer.Data, insetBuffer.Index, outsetBuffer.Index);

                _outlinePaths = new GraphicsPath[] {
                    new GraphicsPath(outlinePen, mergedBuffer.Data, PathType.Closed, 0, insetBuffer.Index + outsetBuffer.Index),
                };

                Pools<Buffer<CCVector2>>.Release(mergedBuffer);
                Pools<Buffer<CCVector2>>.Release(insetBuffer);
                Pools<Buffer<CCVector2>>.Release(outsetBuffer);
            }
        }

        private void CompileClosedPath (IList<CCVector2> points, IList<float> accumLengths, int offset, int count, Pen outlinePen)
        {
            if (_strokeType != StrokeType.Outline)
                InitializeBuffers(count + 1);

            if (IsClose(points[offset], points[offset + count - 1]))
                count--;

            IList<float> lengths = accumLengths ?? _zeroList;

            Buffer<CCVector2> insetBuffer = null;
            Buffer<CCVector2> outsetBuffer = null;

            if (outlinePen != null && _strokeType != StrokeType.Fill) {
                insetBuffer = Pools<Buffer<CCVector2>>.Obtain();
                outsetBuffer = Pools<Buffer<CCVector2>>.Obtain();

                int vCount = _positionData != null ? _positionData.Length : _pen.MaximumVertexCount(count);

                insetBuffer.EnsureCapacity(vCount);
                outsetBuffer.EnsureCapacity(vCount);
            }

            PenWorkspace ws = Pools<PenWorkspace>.Obtain();
            ws.ResetWorkspace(_pen);

            JoinSample joinSample = new JoinSample(points[offset + count - 1], points[offset + 0], points[offset + 1], 0, lengths[offset + 0], lengths[offset + 1]);

            int vBaseIndex = _vertexBufferIndex;
            int vBaseCount = AddJoint(0, ref joinSample, ws, insetBuffer, outsetBuffer);

            int vPrevCount = 0;
            int vNextCount = vBaseCount;

            for (int i = 0; i < count - 2; i++) {
                joinSample.Advance(points[offset + i + 2], lengths[offset + i + 2]);

                vPrevCount = vNextCount;
                vNextCount = AddJoint(i + 1,ref joinSample, ws, insetBuffer, outsetBuffer);
                if (_strokeType != StrokeType.Outline)
                    AddSegment(_vertexBufferIndex - vNextCount - vPrevCount, vPrevCount, _jointCCW[i], _vertexBufferIndex - vNextCount, vNextCount, _jointCCW[i + 1]);
            }

            joinSample.Advance(points[offset + 0], lengths[offset + 0]);

            vPrevCount = vNextCount;
            vNextCount = AddJoint(count - 1, ref joinSample, ws, insetBuffer, outsetBuffer);

            if (_strokeType != StrokeType.Outline) {
                AddSegment(_vertexBufferIndex - vNextCount - vPrevCount, vPrevCount, _jointCCW[count - 2], _vertexBufferIndex - vNextCount, vNextCount, _jointCCW[count - 1]);
                AddSegment(_vertexBufferIndex - vNextCount, vNextCount, _jointCCW[count - 1], vBaseIndex, vBaseCount, _jointCCW[0]);
            }

            Pools<PenWorkspace>.Release(ws);

            if (outlinePen != null && _strokeType != StrokeType.Fill) {
                Array.Reverse(insetBuffer.Data, 0, insetBuffer.Index);

                _outlinePaths = new GraphicsPath[] {
                    new GraphicsPath(outlinePen, insetBuffer.Data, PathType.Closed, 0, insetBuffer.Index),
                    new GraphicsPath(outlinePen, outsetBuffer.Data, PathType.Closed, 0, outsetBuffer.Index),
                };

                Pools<Buffer<CCVector2>>.Release(insetBuffer);
                Pools<Buffer<CCVector2>>.Release(outsetBuffer);
            }
        }

        private bool IsClose (CCVector2 a, CCVector2 b)
        {
            return Math.Abs(a.X - b.X) < 0.001 && Math.Abs(a.Y - b.Y) < 0.001;
        }

        private int AddStartPoint (int pointIndex, CCVector2 a, CCVector2 b, PenWorkspace ws, Buffer<CCVector2> positionBuffer)
        {
            _pen.ComputeStartPoint(a, b, ws);

            int xyCount = AddStartOrEndPoint(pointIndex, ws, positionBuffer, false);

            if (positionBuffer != null)
                Array.Reverse(positionBuffer.Data, positionBuffer.Index - ws.OutlineIndexBuffer.Index, ws.OutlineIndexBuffer.Index);

            return xyCount;
        }

        private int AddEndPoint (int pointIndex, CCVector2 a, CCVector2 b, PenWorkspace ws, Buffer<CCVector2> positionBuffer)
        {
            _pen.ComputeEndPoint(a, b, ws);

            int xyCount = AddStartOrEndPoint(pointIndex, ws, positionBuffer, true);

            return xyCount;
        }

        private int AddStartOrEndPoint (int pointIndex, PenWorkspace ws, Buffer<CCVector2> positionBuffer, bool ccw)
        {
            int xyCount = ws.XYBuffer.Index;

            if (positionBuffer != null) {
                for (int i = 0; i < ws.OutlineIndexBuffer.Index; i++)
                    positionBuffer.SetNext(ws.XYBuffer[ws.OutlineIndexBuffer[i]]);
            }

            if (_strokeType == StrokeType.Outline)
                return 0;

            int baseIndex = _vertexBufferIndex;

            _vertexBufferIndex += xyCount;

            for (int i = 0; i < xyCount; i++)
                _positionData[baseIndex + i] = ws.XYBuffer[i];

            for (int i = 0; i < ws.IndexBuffer.Index; i++)
                _indexData[_indexBufferIndex++] = (short)(baseIndex + ws.IndexBuffer[i]);

            if (_colorData != null) {
                for (int i = 0; i < xyCount; i++)
                    _colorData[baseIndex + i] = _pen.ColorAt(ws.UVBuffer[i], ws.PathLengthScale);
            }

            if (_textureData != null) {
                int texWidth = _pen.Brush.Texture.XNATexture.Width;
                int texHeight = _pen.Brush.Texture.XNATexture.Height;

                for (int i = baseIndex; i < _vertexBufferIndex; i++) {
                    CCVector2 pos = _positionData[i];
                    _textureData[i] = new CCVector2(pos.X / texWidth, pos.Y / texHeight);
                }
            }

            _jointCCW[pointIndex] = ccw;

            return xyCount;
        }

        private int AddJoint (int pointIndex, ref JoinSample joinSample, PenWorkspace ws, Buffer<CCVector2> insetBuffer, Buffer<CCVector2> outsetBuffer)
        {
            InsetOutsetCount vioCount = new InsetOutsetCount();

            switch (_pen.LineJoin) {
                case LineJoin.Miter:
                    vioCount = _pen.ComputeMiter(ref joinSample, ws);
                    break;
                case LineJoin.Bevel:
                    vioCount = _pen.ComputeBevel(ref joinSample, ws);
                    break;
            }

            if (insetBuffer != null) {
                for (int i = 0; i < vioCount.InsetCount; i++)
                    insetBuffer.SetNext(ws.XYInsetBuffer[i]);
            }
            if (outsetBuffer != null) {
                for (int i = 0; i < vioCount.OutsetCount; i++)
                    outsetBuffer.SetNext(ws.XYOutsetBuffer[i]);
            }

            return (_strokeType != StrokeType.Outline) ? AddJoint(pointIndex, vioCount, ws) : 0;
        }

        private int AddJoint (int pointIndex, InsetOutsetCount vioCount, PenWorkspace ws)
        {
            int vIndex = _vertexBufferIndex;

            _vertexBufferIndex += vioCount.InsetCount + vioCount.OutsetCount;

            if (!vioCount.CCW) {
                _jointCCW[pointIndex] = false;
                _positionData[vIndex + 0] = ws.XYOutsetBuffer[0];
                for (int i = 0; i < vioCount.InsetCount; i++)
                    _positionData[vIndex + 1 + i] = ws.XYInsetBuffer[i];

                for (int i = 0; i < vioCount.InsetCount - 1; i++) {
                    _indexData[_indexBufferIndex++] = (short)(vIndex);
                    _indexData[_indexBufferIndex++] = (short)(vIndex + i + 2);
                    _indexData[_indexBufferIndex++] = (short)(vIndex + i + 1);
                }
            }
            else {
                _jointCCW[pointIndex] = true;
                _positionData[vIndex + 0] = ws.XYInsetBuffer[0];
                for (int i = 0; i < vioCount.OutsetCount; i++)
                    _positionData[vIndex + 1 + i] = ws.XYOutsetBuffer[i];

                for (int i = 0; i < vioCount.OutsetCount - 1; i++) {
                    _indexData[_indexBufferIndex++] = (short)(vIndex);
                    _indexData[_indexBufferIndex++] = (short)(vIndex + i + 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndex + i + 2);
                }
            }

            if (_colorData != null) {
                if (!vioCount.CCW) {
                    _colorData[vIndex] = _pen.ColorAt(ws.UVOutsetBuffer[0], ws.PathLengthScale);
                    for (int i = 0; i < vioCount.InsetCount; i++)
                        _colorData[vIndex + 1 + i] = _pen.ColorAt(ws.UVInsetBuffer[i], ws.PathLengthScale);
                }
                else {
                    _colorData[vIndex] = _pen.ColorAt(ws.UVInsetBuffer[0], ws.PathLengthScale);
                    for (int i = 0; i < vioCount.OutsetCount; i++)
                        _colorData[vIndex + 1 + i] = _pen.ColorAt(ws.UVOutsetBuffer[i], ws.PathLengthScale);
                }
            }

            if (_textureData != null) {
                int texWidth = _pen.Brush.Texture.XNATexture.Width;
                int texHeight = _pen.Brush.Texture.XNATexture.Height;

                for (int i = vIndex; i < _vertexBufferIndex; i++) {
                    CCVector2 pos = _positionData[i];
                    _textureData[i] = new CCVector2(pos.X / texWidth, pos.Y / texHeight);
                }
            }

            return _vertexBufferIndex - vIndex;
        }

        private void AddSegment (int vIndexStart, int vStartCount, bool vStartCCW, int vIndexEnd, int vEndCount, bool vEndCCW)
        {
            if (vStartCCW) {
                if (vEndCCW) {
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + vStartCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + vEndCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + vEndCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + 0);
                }
                else {
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + vStartCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + vEndCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + 0);
                }
            }
            else {
                if (vEndCCW) {
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + vStartCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + vEndCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + vEndCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + vStartCount - 1);
                }
                else {
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + vStartCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + 0);
                    _indexData[_indexBufferIndex++] = (short)(vIndexEnd + vEndCount - 1);
                    _indexData[_indexBufferIndex++] = (short)(vIndexStart + vStartCount - 1);
                }
            }
        }
    }
}
