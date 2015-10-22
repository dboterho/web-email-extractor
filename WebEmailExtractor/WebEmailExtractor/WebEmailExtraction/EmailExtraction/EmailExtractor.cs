using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebEmailExtractor.WebEmailExtraction.EmailExtraction
{
    public class EmailExtractor
    {

        protected readonly string EmailRegex;


        public EmailExtractor(string emailRegex)
        {
            EmailRegex = emailRegex;
        }


        public EmailExtractResult ExtractEmailsFromMarkup(string siteUrl, 
            List<string> aggregatedSiteMarkup)
        {
            var emailRegex = new Regex(EmailRegex);
            var emails = new List<string>();
            var hasMailto = false;

            // iterate each markup string and collect all email matches
            foreach (var markup in aggregatedSiteMarkup)
            {
                if (!hasMailto)
                    hasMailto = markup.Contains("mailto:");

                ExtractEmails(emailRegex, markup, emails);
            }

            // remove duplicate emails
            var distinctEmails = emails.Distinct().ToList();

            return new EmailExtractResult
            {
                SiteUrl = siteUrl,
                Emails = distinctEmails,
                HasMailto = hasMailto
            };
        }

        private static void ExtractEmails(Regex emailRegex, string markup, List<string> emails)
        {
            var emailMatches = emailRegex.Matches(markup);

            foreach (Match emailMatch in emailMatches)
            {
                if (!emailMatch.Success)
                    continue;

                emails.Add(emailMatch.Value);
            }
        }
    }
}
