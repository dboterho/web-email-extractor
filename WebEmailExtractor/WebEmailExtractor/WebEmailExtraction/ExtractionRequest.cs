using WebEmailExtractor.Logging;

namespace WebEmailExtractor.WebEmailExtraction
{
    public class ExtractionRequest
    {
        public string InputFilePath { get; set; }
        public string OutputDirectory { get; set; }
        public VerboseLogger VerboseLogger { get; set; }

    }
}