using System;

namespace Lowscope.AppwritePlugin.Utils
{
    public class AppwriteException : Exception
    {
        public AppwriteException(string message, ErrorType errorType) : base(message)
        {
            this.errorType = errorType;
        }

        public AppwriteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AppwriteException(string message, long responseCode) : base(message)
        {
            ResponseCode = responseCode;
        }

        public long ResponseCode { get; }
        public ErrorType errorType { get; }
    }
}

