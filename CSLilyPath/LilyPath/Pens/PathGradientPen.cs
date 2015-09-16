using Microsoft.Xna.Framework;
using CocosSharp;
using LilyPath.Utility;

namespace LilyPath.Pens
{
    /// <summary>
    /// A <see cref="Pen"/> that blends two colors across the length of the stroked path.
    /// </summary>
    public class PathGradientPen : Pen
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
        /// <param name="startColor">The starting pen color.</param>
        /// <param name="endColor">The ending pen color.</param>
        /// <param name="width">The width of the paths drawn by the pen.</param>
        public PathGradientPen (Color startColor, Color endColor, float width)
            : base(Color.White, width)
        {
            _r1 = startColor.R;
            _g1 = startColor.G;
            _b1 = startColor.B;
            _a1 = startColor.A;

            _rdiff = (short)(endColor.R - _r1);
            _gdiff = (short)(endColor.G - _g1);
            _bdiff = (short)(endColor.B - _b1);
            _adiff = (short)(endColor.A - _a1);
        }

        /// <summary>
        /// Creates a new <see cref="GradientPen"/> with the given colors and a width of 1.
        /// </summary>
        /// <param name="startColor">The starting pen color.</param>
        /// <param name="endColor">The ending pen color.</param>
        public PathGradientPen (Color startColor, Color endColor)
            : this(startColor, endColor, 1)
        { }

        /// <InheritDoc />
        public override bool NeedsPathLength
        {
            get { return true; }
        }

        /// <InheritDoc />
        protected internal override CCColor4B ColorAt (float widthPosition, float lengthPosition, float lengthScale)
        {
            return Lerp(lengthPosition * lengthScale);
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
