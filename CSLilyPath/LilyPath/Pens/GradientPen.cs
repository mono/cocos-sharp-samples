using Microsoft.Xna.Framework;
using CocosSharp;
using LilyPath.Utility;

namespace LilyPath.Pens
{
    /// <summary>
    /// A <see cref="Pen"/> that blends two colors across its stroke width.
    /// </summary>
    public class GradientPen : Pen
    {
        private byte _r1;
        private byte _g1;
        private byte _b1;
        private byte _a1;

        private short _rdiff;
        private short _gdiff;
        private short _bdiff;
        private short _adiff;

        /// <summary>
        /// Creates a new <see cref="GradientPen"/> with the given colors and width.
        /// </summary>
        /// <param name="color1">The first pen color.</param>
        /// <param name="color2">The second pen color.</param>
        /// <param name="width">The width of the paths drawn by the pen.</param>
        public GradientPen (Color color1, Color color2, float width)
            : base(Color.White, width)
        {
            _r1 = color1.R;
            _g1 = color1.G;
            _b1 = color1.B;
            _a1 = color1.A;

            _rdiff = (short)(color2.R - _r1);
            _gdiff = (short)(color2.G - _g1);
            _bdiff = (short)(color2.B - _b1);
            _adiff = (short)(color2.A - _a1);
        }

        /// <summary>
        /// Creates a new <see cref="GradientPen"/> with the given colors and a width of 1.
        /// </summary>
        /// <param name="color1">The first pen color.</param>
        /// <param name="color2">The second pen color.</param>
        public GradientPen (Color color1, Color color2)
            : this(color1, color2, 1)
        { }

        /// <InheritDoc />
        protected internal override CCColor4B ColorAt (float widthPosition, float lengthPosition, float lengthScale)
        {
            return Lerp(widthPosition);
        }

        private CCColor4B Lerp (float amount)
        {
            Color c0 = Color.TransparentBlack;
            c0.R = (byte)(_r1 + _rdiff * amount);
            c0.G = (byte)(_g1 + _gdiff * amount);
            c0.B = (byte)(_b1 + _bdiff * amount);
            c0.A = (byte)(_a1 + _adiff * amount);

            return c0.ToCCColor4B();
        }
    }
}
