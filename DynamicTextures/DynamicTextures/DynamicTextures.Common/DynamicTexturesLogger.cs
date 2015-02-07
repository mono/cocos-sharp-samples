using System;
using CocosSharp;

namespace DynamicTextures
{
    public class DynamicTexturesLogger : ICCLog
    {
        public DynamicTexturesLogger()
        {
        }

        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Log(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(format, args);
        }
    }
}

