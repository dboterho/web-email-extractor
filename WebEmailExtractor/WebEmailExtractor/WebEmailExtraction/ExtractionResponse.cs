using System.Collections.Generic;
using System.Linq;

namespace WebEmailExtractor.WebEmailExtraction
{
    public class ExtractionResponse
    {
        public string GeneralException { get; set; }
        public List<ExtractionException> ExtractionExceptions { get; set; }
        public int SuccessfulExtractions { get; set; }

        public bool HasExtractionException => ExtractionExceptions != null && ExtractionExceptions.Any();
    }
}