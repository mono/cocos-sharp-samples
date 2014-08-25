using CocosSharp;
using System;

namespace AirHockey.Store
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
			CCApplication.Create(new AppDelegate());
        }
    }
}
