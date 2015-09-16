using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LilyPath
{
    /// <summary>
    /// A <see cref="Brush"/> that represents a solid color.
    /// </summary>
    public class SolidColorBrush : Brush
    {
        /// <summary>
        /// Creates a new <see cref="SolidColorBrush"/> from the given <see cref="GraphicsDevice"/> and <see cref="Color"/>.
        /// </summary>
        /// <param name="color">A color.</param>
        /// <remarks>The <see cref="Brush.Alpha"/> property of the brush is initialized 
        /// to the alpha value of the color.</remarks>
        public SolidColorBrush (Color color)
            : base(color.A / 255f)
        {
            Color = color;
        }

        /// <summary>
        /// The color of the brush.
        /// </summary>
        public new Color Color
        {
            get { return base.Color; }
            private set { base.Color = value; }
        }
    }
}
