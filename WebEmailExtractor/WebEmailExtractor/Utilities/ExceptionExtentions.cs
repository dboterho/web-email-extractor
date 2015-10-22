using System;

namespace WebEmailExtractor.Utilities
{
    public static class ExceptionExtentions
    {
        public static string GetInnerMostException(this Exception exception)
        {
            if (exception.InnerException != null)
                return exception.GetInnerMostException();

            return exception.Message;
        }
    }
}
