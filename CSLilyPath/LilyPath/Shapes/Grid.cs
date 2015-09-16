using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CocosSharp;

namespace LilyPath.Shapes
{
    /// <summary>
    /// An object that compiles a low-level geometry cache for a grid.
    /// </summary>
    /// <remarks>Geometry compiled by a <see cref="Grid"/> object does not overlap at intersections, unlike a grid rendered
    /// manually as a series of crossing lines.</remarks>
    public class Grid
    {
        private VertexPositionColorTexture[] _vertexBuffer;
        private short[] _indexBuffer;

        private int _columns;
        private int _rows;

        private int[] _serial = new int[1];

        /// <summary>
        /// Creates a new <see cref="Grid"/> object from the given columns and rows.
        /// </summary>
        /// <param name="columns">The number of columns in the grid.</param>
        /// <param name="rows">The number of rows in the grid.</param>
        /// <remarks><see cref="Grid"/> objects initialize their memory buffers from the given column and row values.  A grid can
        /// be compiled any number of times without needing to allocate new buffers.</remarks>
        public Grid (int columns, int rows)
        {
            _columns = columns;
            _rows = rows;

            int pointCount = (columns + 1) * (rows + 1);
            int vsegCount = (columns + 1) * rows;
            int hsegCount = columns * (rows + 1);

            _vertexBuffer = new VertexPositionColorTexture[pointCount * 4];
            _indexBuffer = new short[(pointCount * 2 + vsegCount * 2 + hsegCount * 2) * 3];
        }

        /// <summary>
        /// Compiles the grid's geometry from the given <see cref="Pen"/> and overall dimensions into a <see cref="DrawCache"/> object.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/> to render the grid's lines with.</param>
        /// <param name="left">The ledge edge of the grid.</param>
        /// <param name="top">The top edge of the grid.</param>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <returns>A <see cref="DrawCache"/> with the compiled geometry.</returns>
        /// <remarks>Only the most recently compiled <see cref="DrawCache"/> is valid for any given <see cref="Grid"/>.</remarks>
        public DrawCache Compile (Pen pen, float left, float top, float width, float height)
        {
            short vIndex = 0;
            short iIndex = 0;

            float w2 = pen.Width / 2;

            for (int y = 0; y <= _rows; y++) {
                for (int x = 0; x <= _columns; x++) {
                    float px = left + (width / _columns) * x;
                    float py = top + (height / _rows) * y;

                    AddVertex(_vertexBuffer, vIndex + 0, px - w2, py - w2, pen.ColorAt(0, 0, 0));
                    AddVertex(_vertexBuffer, vIndex + 1, px + w2, py - w2, pen.ColorAt(0, 0, 0));
                    AddVertex(_vertexBuffer, vIndex + 2, px - w2, py + w2, pen.ColorAt(0, 0, 0));
                    AddVertex(_vertexBuffer, vIndex + 3, px + w2, py + w2, pen.ColorAt(0, 0, 0));

                    AddQuad(_indexBuffer, iIndex, vIndex);

                    vIndex += 4;
                    iIndex += 6;
                }
            }

            for (int y = 0; y < _rows; y++) {
                for (int x = 0; x <= _columns; x++) {
                    int baseV1 = (y * (_columns + 1) + x) * 4;
                    int baseV2 = ((y + 1) * (_columns + 1) + x) * 4;

                    AddQuad(_indexBuffer, iIndex, (short)(baseV1 + 2), (short)(baseV1 + 3), (short)(baseV2 + 0), (short)(baseV2 + 1));

                    iIndex += 6;
                }
            }

            for (int y = 0; y <= _rows; y++) {
                for (int x = 0; x < _columns; x++) {
                    int baseV1 = (y * (_columns + 1) + x) * 4;
                    int baseV2 = baseV1 + 4;

                    AddQuad(_indexBuffer, iIndex, (short)(baseV1 + 1), (short)(baseV2 + 0), (short)(baseV1 + 3), (short)(baseV2 + 2));

                    iIndex += 6;
                }
            }

            _serial[0]++;

            DrawCache cache = new DrawCache();
            cache.AddUnit(new GridCacheUnit(_vertexBuffer, _indexBuffer, pen.Brush.Texture, _serial));

            return cache;
        }

        private void AddVertex (VertexPositionColorTexture[] buffer, int index, float x, float y, Color c)
        {
            buffer[index] = new VertexPositionColorTexture(new Vector3(x, y, 0), c, new Vector2(0, 0));
        }

        private void AddQuad (short[] buffer, short iIndex, short vIndex)
        {
            buffer[iIndex + 0] = (short)(vIndex + 2);
            buffer[iIndex + 1] = (short)(vIndex + 0);
            buffer[iIndex + 2] = (short)(vIndex + 1);
            buffer[iIndex + 3] = (short)(vIndex + 2);
            buffer[iIndex + 4] = (short)(vIndex + 1);
            buffer[iIndex + 5] = (short)(vIndex + 3);
        }

        private void AddQuad (short[] buffer, short index, short tl, short tr, short bl, short br)
        {
            buffer[index + 0] = bl;
            buffer[index + 1] = tl;
            buffer[index + 2] = tr;
            buffer[index + 3] = bl;
            buffer[index + 4] = tr;
            buffer[index + 5] = br;
        }

        private class GridCacheUnit : DrawCacheUnit
        {
            private int[] _sourceSerial;
            private int _serial;

            public GridCacheUnit (VertexPositionColorTexture[] vertexBuffer, short[] indexBuffer, CCTexture2D texture, int[] serial)
                : base(vertexBuffer, indexBuffer, texture)
            {
                _sourceSerial = serial;
                _serial = serial[0];
            }

            public override bool IsValid
            {
                get { return _serial == _sourceSerial[0]; }
            }
        }
    }
}
