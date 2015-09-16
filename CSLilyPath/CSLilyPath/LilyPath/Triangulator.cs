using System;
using System.Collections.Generic;
using CocosSharp;

namespace LilyPath
{
    /// <summary>
    /// Computes a set of interior triangles from a set of points that defines a containing path.
    /// </summary>
    public class Triangulator
    {
        private int[] _triPrev = new int[128];
        private int[] _triNext = new int[128];

        private int[] _indexComputeBuffer = new int[128];
        private int _indexCount = 0;

        /// <summary>
        /// The indexes of triangle list entries for the list of points used in the last <see cref="Triangulate"/> call.
        /// </summary>
        public int[] ComputedIndexes
        {
            get { return _indexComputeBuffer; }
        }

        /// <summary>
        /// The number of indexes generated in the last computation.
        /// </summary>
        public int ComputedIndexCount
        {
            get { return _indexCount; }
        }

        /// <summary>
        /// Computes a triangle list that fully covers the area enclosed by the given set of points.
        /// </summary>
        /// <param name="points">A list of points that defines an enclosing path.</param>
        /// <param name="offset">The offset of the first point in the list.</param>
        /// <param name="count">The number of points in the path.</param>
        public void Triangulate (IList<CCVector2> points, int offset, int count)
        {
            Initialize(count);

            int index = 0;
            int computeIndex = 0;
            while (count >= 3) {
                bool isEar = true;

                CCVector2 a = points[offset + _triPrev[index]];
                CCVector2 b = points[offset + index];
                CCVector2 c = points[offset + _triNext[index]];
                if (TriangleIsCCW(a, b, c)) {
                    int k = _triNext[_triNext[index]];
                    do {
                        if (PointInTriangleInclusive(points[offset + k], a, b, c)) {
                            isEar = false;
                            break;
                        }
                        k = _triNext[k];
                    } while (k != _triPrev[index]);
                }
                else {
                    isEar = false;
                }

                if (isEar) {
                    if (_indexComputeBuffer.Length < computeIndex + 3)
                        Array.Resize(ref _indexComputeBuffer, _indexComputeBuffer.Length * 2);

                    _indexComputeBuffer[computeIndex++] = offset + _triPrev[index];
                    _indexComputeBuffer[computeIndex++] = offset + index;
                    _indexComputeBuffer[computeIndex++] = offset + _triNext[index];

                    _triNext[_triPrev[index]] = _triNext[index];
                    _triPrev[_triNext[index]] = _triPrev[index];
                    count--;
                    index = _triPrev[index];
                }
                else {
                    index = _triNext[index];
                }
            }

            _indexCount = computeIndex;
        }

        private void Initialize (int count)
        {
            _indexCount = 0;

            if (_triNext.Length < count)
                Array.Resize(ref _triNext, Math.Max(_triNext.Length * 2, count));
            if (_triPrev.Length < count)
                Array.Resize(ref _triPrev, Math.Min(_triPrev.Length * 2, count));

            for (int i = 0; i < count; i++) {
                _triPrev[i] = i - 1;
                _triNext[i] = i + 1;
            }

            _triPrev[0] = count - 1;
            _triNext[count - 1] = 0;
        }

        private float Cross2D (CCVector2 u, CCVector2 v)
        {
            return (u.Y * v.X) - (u.X * v.Y);
        }

        private bool PointInTriangleInclusive (CCVector2 point, CCVector2 a, CCVector2 b, CCVector2 c)
        {
            if (Cross2D(point - a, b - a) <= 0f)
                return false;
            if (Cross2D(point - b, c - b) <= 0f)
                return false;
            if (Cross2D(point - c, a - c) <= 0f)
                return false;
            return true;
        }

        private bool TriangleIsCCW (CCVector2 a, CCVector2 b, CCVector2 c)
        {
            return Cross2D(b - a, c - b) < 0;
        }
    }
}
