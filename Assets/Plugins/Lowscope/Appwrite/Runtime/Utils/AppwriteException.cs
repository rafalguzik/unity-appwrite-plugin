using System;

namespace Lowscope.AppwritePlugin.Utils
{
    public class AppwriteException : Exception
    {
        public AppwriteException(string? message) : base(message)
        {

        }
    }
}

