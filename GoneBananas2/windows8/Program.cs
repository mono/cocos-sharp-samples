using System;
using CocosSharp;

namespace GoneBananas
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        static void Main()
        {
            CCApplication.Create(new GoneBananasApplicationDelegate());
        }
    }
}
