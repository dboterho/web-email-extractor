using System;

namespace WebEmailExtractor.WebEmailExtraction
{
    public class ExtractionException : Exception
    {

        public string InputUrl { get; set; }


        public ExtractionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
