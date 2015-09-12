using System;

namespace DynamicTextures
{
    public static class Utilities
    {
        /// <summary>
        /// Returns the next Power of Two for the given value. If x = 3, then this returns 4.
        /// If x = 4 then 4 is returned. If the value is a power of two, then the same value
        /// is returned.
        /// </summary>
        /// <param name="x">The base of the POT test</param>
        /// <returns>The next power of 2 (1, 2, 4, 8, 16, 32, 64, 128, etc)</returns>
        public static int NextPOT(int x)
        {
            x = x - 1;
            x = x | (x >> 1);
            x = x | (x >> 2);
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            return x + 1;
        }

        /// <summary>
        /// Returns the next Power of Two for the given value. If x = 3, then this returns 4.
        /// If x = 4 then 4 is returned. If the value is a power of two, then the same value
        /// is returned.
        /// </summary>
        /// <param name="x">The base of the POT test</param>
        /// <returns>The next power of 2 (1, 2, 4, 8, 16, 32, 64, 128, etc)</returns>
        public static int NextPOT(this float x)
        {
            return NextPOT((int)x);
        }
    }
}

