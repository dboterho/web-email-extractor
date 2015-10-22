namespace WebEmailExtractor.WebEmailExtraction.ConfigProvider
{
    public interface IExtractionConfigProvider
    {
        char GetCsvDelimiter();
        string GetEmailRegex();
        string GetHrefRegex();
        string[] GetInvalidSiteLinkPatterns();

    }
}
