using System;
using System.Configuration;

namespace WebEmailExtractor.WebEmailExtraction.ConfigProvider
{
    class AppConfigProvider : IExtractionConfigProvider
    {
        public char GetCsvDelimiter()
        {
            return char.Parse(ConfigurationManager.AppSettings["CsvDelimiter"]);
        }

        public string GetEmailRegex()
        {
            return ConfigurationManager.AppSettings["EmailRegex"];
        }

        public string GetHrefRegex()
        {
            return ConfigurationManager.AppSettings["HrefRegex"];
        }

        public string[] GetInvalidSiteLinkPatterns()
        {
            var invalidSiteLinkPatterns = ConfigurationManager.AppSettings["InvalidSiteLinkPatterns"];

            return invalidSiteLinkPatterns.Split('|');
        }
    }
}
