using System.Collections.Generic;

namespace WebEmailExtractor.WebEmailExtraction
{
    public class EmailExtractResult
    {
        public string SiteUrl { get; set; }
        public List<string> Emails { get; set; }
        public bool HasMailto { get; set; }
    }
}