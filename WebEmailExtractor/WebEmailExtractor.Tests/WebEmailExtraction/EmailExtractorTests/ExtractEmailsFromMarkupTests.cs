using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebEmailExtractor.WebEmailExtraction.EmailExtraction;

namespace WebEmailExtractor.Tests.WebEmailExtraction.EmailExtractorTests
{
    public class ExtractEmailsFromMarkupTests
    {

        private const string _emailRegex = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";



        [SetUp]
        public void GivenAggregatedSiteMarkupExists()
        {
        }


        public EmailExtractor GetEmailExtractor()
        {
            return new EmailExtractor(_emailRegex);
        }


        [Test]
        public void WhenThereAreNoEmails_ThenOnlyTheSiteUrlIsReturned()
        {
            var result = GetEmailExtractor().ExtractEmailsFromMarkup(
                "http://www.test.com",
                new List<string>
                {
                    "<br /><br />",
                    "<img src=\"/somepath.jpg\"></img>"
                });

            Assert.AreEqual("http://www.test.com", result.SiteUrl);
            Assert.IsFalse(result.Emails.Any());
            Assert.IsFalse(result.HasMailto);
        }

        [Test]
        public void WhenThereIsOneEmail_ThenOneEmailIsReturnedWithTheSiteUrl()
        {
            var result = GetEmailExtractor().ExtractEmailsFromMarkup(
                "http://www.test.com",
                new List<string>
                {
                    "<br /><br />",
                    "<a href=\"mailto:test@email.com\">test@email.com</a>"
                });

            Assert.AreEqual("http://www.test.com", result.SiteUrl);
            Assert.AreEqual(1, result.Emails.Count);
            Assert.IsTrue(result.HasMailto);

            var email1 = result.Emails.First();

            Assert.AreEqual("test@email.com", email1);
        }

        [Test]
        public void WhenThereAreTwoEmails_ThenTwoEmailsAreReturnedWithTheSiteUrl()
        {
            var result = GetEmailExtractor().ExtractEmailsFromMarkup(
                "http://www.test.com",
                new List<string>
                {
                    "<br /><a href=\"mailto:test1@email.com\">click here</a><br />",
                    "<a href=\"mailto:test2@email.com\">test2@email.com</a>"
                });

            Assert.AreEqual("http://www.test.com", result.SiteUrl);
            Assert.AreEqual(2, result.Emails.Count);
            Assert.IsTrue(result.HasMailto);

            var email1 = result.Emails.First();

            Assert.AreEqual("test1@email.com", email1);

            var email2 = result.Emails.Skip(1).First();

            Assert.AreEqual("test2@email.com", email2);
        }

        [Test]
        public void WhenThereAreTwoDuplicateEmails_ThenOneEmailIsReturned()
        {
            var result = GetEmailExtractor().ExtractEmailsFromMarkup(
                "http://www.test.com",
                new List<string>
                {
                    "<br /><a href=\"mailto:test1@email.com\">click here</a><br />",
                    "<a href=\"mailto:test1@email.com\">click here</a>"
                });

            Assert.AreEqual("http://www.test.com", result.SiteUrl);
            Assert.AreEqual(1, result.Emails.Count);
            Assert.IsTrue(result.HasMailto);

            var email1 = result.Emails.First();

            Assert.AreEqual("test1@email.com", email1);
        }

    }
}
