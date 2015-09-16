using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LilyPath
{
    /// <summary>
    /// Objects used to fill the interiors of shapes and paths.
    /// </summary>
    public abstract class Brush : IDisposable
    {
        #region Default Brushes

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Black { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Blue { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Brown { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Cyan { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkBlue { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkCyan { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkGray { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkGoldenrod { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkGreen { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkMagenta { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkOrange { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush DarkRed { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Goldenrod { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Gray { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Green { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush LightBlue { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush LightCyan { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush LightGray { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush LightGreen { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush LightPink { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush LightYellow { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Lime { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Magenta { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Orange { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Pink { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Purple { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Red { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Teal { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush White { get; private set; }

        /// <summary>A system-defined <see cref="Brush"/> object.</summary>
        public static Brush Yellow { get; private set; }

        static Brush ()
        {
            Black = new SolidColorBrush(Color.Black);
            Blue = new SolidColorBrush(Color.Blue);
            Brown = new SolidColorBrush(Color.Brown);
            Cyan = new SolidColorBrush(Color.Cyan);
            DarkBlue = new SolidColorBrush(Color.DarkBlue);
            DarkCyan = new SolidColorBrush(Color.DarkCyan);
            DarkGoldenrod = new SolidColorBrush(Color.DarkGoldenrod);
            DarkGray = new SolidColorBrush(Color.DarkGray);
            DarkGreen = new SolidColorBrush(Color.DarkGreen);
            DarkMagenta = new SolidColorBrush(Color.DarkMagenta);
            DarkOrange = new SolidColorBrush(Color.DarkOrange);
            DarkRed = new SolidColorBrush(Color.DarkRed);
            Goldenrod = new SolidColorBrush(Color.Goldenrod);
            Gray = new SolidColorBrush(Color.Gray);
            Green = new SolidColorBrush(Color.Green);
            LightBlue = new SolidColorBrush(Color.LightBlue);
            LightCyan = new SolidColorBrush(Color.LightCyan);
            LightGray = new SolidColorBrush(Color.LightGray);
            LightGreen = new SolidColorBrush(Color.LightGreen);
            LightPink = new SolidColorBrush(Color.LightPink);
            LightYellow = new SolidColorBrush(Color.LightYellow);
            Lime = new SolidColorBrush(Color.Lime);
            Magenta = new SolidColorBrush(Color.Magenta);
            Orange = new SolidColorBrush(Color.Orange);
            Pink = new SolidColorBrush(Color.Pink);
            Purple = new SolidColorBrush(Color.Purple);
            Red = new SolidColorBrush(Color.Red);
            Teal = new SolidColorBrush(Color.Teal);
            White = new SolidColorBrush(Color.White);
            Yellow = new SolidColorBrush(Color.Yellow);
        }

        #endregion

        private float _alpha;

        /// <summary>
        /// Initializes a new instance of a <see cref="Brush"/> class.
        /// </summary>
        protected Brush ()
        {
            Color = Color.White;
            Transform = Matrix.Identity;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Brush"/> class with a given alpha value.
        /// </summary>
        /// <param name="alpha">Alpha value of the brush.</param>
        protected Brush (float alpha)
            : this()
        {
            _alpha = alpha;
        }

        /// <summary>
        /// The alpha value of the brush.
        /// </summary>
        public virtual float Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        /// <summary>
        /// The color of the brush.
        /// </summary>
        protected internal Color Color { get; protected set; }

        /// <summary>
        /// The texture resource of the brush.
        /// </summary>
        protected internal Texture2D Texture { get; protected set; }

        /// <summary>
        /// Gets or sets the transformation to apply to brush.
        /// </summary>
        protected internal Matrix Transform { get; protected set; }

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Releases all resources used by the <see cref="Brush"/> object.
        /// </summary>
        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose (bool disposing)
        {
            if (!_disposed) {
                if (disposing)
                    DisposeManaged();
                DisposeUnmanaged();
                _disposed = true;
            }
        }

        /// <summary>
        /// Attempts to dispose unmanaged resources.
        /// </summary>
        ~Brush ()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="Brush"/>.
        /// </summary>
        protected virtual void DisposeManaged () { }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Brush"/>.
        /// </summary>
        protected virtual void DisposeUnmanaged () { }

        #endregion
    }
}
