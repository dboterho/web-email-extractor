using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebEmailExtractor.Logging;
using WebEmailExtractor.WebEmailExtraction.Http;

namespace WebEmailExtractor.WebEmailExtraction.MarkupAggregation
{
    public class MarkupAggregator
    {

        protected readonly IHttpAgent HttpAgent;
        protected readonly VerboseLogger VerboseLogger;
        protected readonly string HrefRegex;
        protected readonly string[] InvalidSiteLinkPatterns;


        public MarkupAggregator(
            IHttpAgent httpAgent,
            VerboseLogger verboseLogger,
            string hrefRegex,
            string[] invalidSiteLinkPatterns)
        {
            HttpAgent = httpAgent;
            VerboseLogger = verboseLogger;
            HrefRegex = hrefRegex;
            InvalidSiteLinkPatterns = invalidSiteLinkPatterns;
        }


        public List<string> AggregateMarkupFromRootUrl(string siteUrl)
        {
            var markupCollection = new List<string>();

            var homeMarkup = HttpAgent.GetWebPageMarkup(siteUrl);

            if (string.IsNullOrEmpty(homeMarkup))
                return markupCollection;

            markupCollection.Add(homeMarkup);

            // record called urls to prevent duplicates
            var capturedUrls = new List<string>();

            var hrefRegex = new Regex(HrefRegex);
            var hrefMatches = hrefRegex.Matches(homeMarkup);

            foreach (var hrefMatch in hrefMatches.Cast<Match>().Where(hrefUrl => hrefUrl.Success))
            {
                var matchUrl = hrefMatch.Value;

                var isAbsoluteUrl = matchUrl.Contains("http://") || matchUrl.Contains("https://");

                var link = ExtractHrefUrl(hrefMatch.Value, isAbsoluteUrl);

                VerboseLogger.LogVerbose($"processing website link {link}");

                if (IsInvalidInternalLink(link))
                    continue;

                var targetUrl = isAbsoluteUrl ? link : $"{siteUrl}{link}";

                // ignore already called urls
                if (capturedUrls.Contains(targetUrl))
                    continue;

                var webPageMarkup = HttpAgent.GetWebPageMarkup(targetUrl);

                if (!string.IsNullOrEmpty(webPageMarkup))
                    markupCollection.Add(webPageMarkup);

                capturedUrls.Add(targetUrl);
            }

            return markupCollection;
        }

        private string ExtractHrefUrl(string regexValue, bool isAbsolute)
        {
            // trim href attribute from start and end of regex string
            var hrefUrl = regexValue.Replace("href=\"", "").Replace("\"", "");
            hrefUrl = hrefUrl.Split('#')[0];

            // ensure that the path is relative to the root site url
            if (!isAbsolute)
                hrefUrl = !hrefUrl.StartsWith("/") ? $"/{hrefUrl}" : hrefUrl;

            return hrefUrl;
        }

        private bool IsInvalidInternalLink(string hrefUrl)
        {
            var invalidInternalLink = false;

            // the homepage link is not valid
            if (hrefUrl == "/")
                return false;

            // use patterns to weed out any unwanted links i.e. .css, .jpg
            foreach (var pattern in InvalidSiteLinkPatterns)
            {
                if (hrefUrl.Contains(pattern))
                    invalidInternalLink = true;
            }

            return invalidInternalLink;
        }

    }
}
