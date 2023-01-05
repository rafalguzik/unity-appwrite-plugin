using System;

namespace Lowscope.AppwritePlugin.Utils
{
    public class AppwriteException : Exception
    {
        public enum Error
        {
            NotAuthorized,
            NotFound,
            UnknownError
        }

        private Error _error;

        public AppwriteException(Error error, string? message) : base(message)
        {
            _error = error;
        }

        public AppwriteException(Error error): base(error.ToString())
        {
            _error = error;
        }
    }
}

