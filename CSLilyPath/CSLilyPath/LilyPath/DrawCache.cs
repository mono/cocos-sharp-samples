using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace LilyPath
{
    /// <summary>
    /// An opaque object that represents pre-compiled low-level geometry.
    /// </summary>
    /// <remarks><see cref="DrawCache"/> objects can be rendered by a <see cref="DrawBatch"/>.</remarks>
    public class DrawCache
    {
        private List<DrawCacheUnit> _units = new List<DrawCacheUnit>();

        /// <summary>
        /// Gets whether the <see cref="DrawCache"/> is still valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                foreach (var unit in _units) {
                    if (!unit.IsValid)
                        return false;
                }

                return true;
            }
        }

        internal void AddUnit (DrawCacheUnit unit)
        {
            _units.Add(unit);
        }

        internal void Render (GraphicsDevice device, Texture2D defaultTexture)
        {
            foreach (var unit in _units)
                unit.Render(device, defaultTexture);
        }
    }

    internal class DrawCacheUnit
    {
        private Texture2D _texture;
        private VertexPositionColorTexture[] _vertexBuffer;
        private short[] _indexBuffer;

        public DrawCacheUnit (VertexPositionColorTexture[] vertexBuffer, short[] indexBuffer, Texture2D texture)
        {
            _texture = texture;
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
        }

        public DrawCacheUnit (VertexPositionColorTexture[] vertexBuffer, int vertexOffset, int vertexCount, short[] indexBuffer, int indexOffset, int indexCount, Texture2D texture)
        {
            if (vertexCount > vertexBuffer.Length - vertexOffset)
                throw new ArgumentException("vertexBuffer is too small for the given vertexOffset and vertexCount.");
            if (indexCount > indexBuffer.Length - indexOffset)
                throw new ArgumentException("indexBuffer is too small for the given indexOffset and indexCount.");

            _texture = texture;

            _vertexBuffer = new VertexPositionColorTexture[vertexCount];
            _indexBuffer = new short[indexCount];

            Array.Copy(vertexBuffer, vertexOffset, _vertexBuffer, 0, vertexCount);
            Array.Copy(indexBuffer, indexOffset, _indexBuffer, 0, indexCount);
        }

        public virtual bool IsValid
        {
            get { return true; }
        }

        public void Render (GraphicsDevice device, Texture defaultTexture)
        {
            device.Textures[0] = _texture ?? defaultTexture;
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertexBuffer, 0, _vertexBuffer.Length, _indexBuffer, 0, _indexBuffer.Length / 3);
        }
    }
}
