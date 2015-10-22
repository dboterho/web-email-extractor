using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebEmailExtractor.Tests.WebEmailExtraction.Fakes;
using WebEmailExtractor.WebEmailExtraction.Http;
using WebEmailExtractor.WebEmailExtraction.MarkupAggregation;

namespace WebEmailExtractor.Tests.WebEmailExtraction.MarkupAggregatorTests
{
    public class AggregateMarkupFromRootUrlTests
    {

        private const string _hrefRegex = "href=[\'\"]?([^\'\" >]+)";
        private const string _invalidSiteLinkPatterns = ".css|.js|.jpg|.png|.gif|.ico|href|tel:|mailto:";

        private Mock<IHttpAgent> MockHttpAgent;
        private VerboseLoggerFake VerboseLoggerFake;

        [SetUp]
        public void GivenValidMarkupReturned()
        {
            MockHttpAgent = new Mock<IHttpAgent>();
            VerboseLoggerFake = new VerboseLoggerFake(true, s => {});
        }

        public MarkupAggregator GetMarkupAggregator()
        {
            return new MarkupAggregator(
                MockHttpAgent.Object,
                VerboseLoggerFake,
                _hrefRegex,
                _invalidSiteLinkPatterns.Split('|'));
        }

        [Test]
        public void WhenNoLinksArePresent_ThenNoChildPageMarkupIsCollected()
        {
            var url = "http://www.test.com";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<br />");

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(1, aggMarkup.Count);

            var markup1 = aggMarkup.First();

            Assert.AreEqual("<br />", markup1);
        }

        [Test]
        public void WhenOneChildLinkIsPresent_ThenOneChildPageMarkupIsCollected()
        {
            var url = "http://www.test.com";
            var urlChild1 = "http://www.test.com/child1";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"/child1\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Returns("<br />");

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(2, aggMarkup.Count);

            var markup2 = aggMarkup.Skip(1).First();

            Assert.AreEqual("<br />", markup2);
        }

        [Test]
        public void WhenTwoChildLinksArePresent_ThenTwoChildPageMarkupsAreCollected()
        {
            var url = "http://www.test.com";
            var urlChild1 = "http://www.test.com/child1";
            var urlChild2 = "http://www.test.com/child2";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"/child1\"></a><br /><a href=\"/child2\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Returns("<br />");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild2))
                .Returns("<br /><br />");

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(3, aggMarkup.Count);

            var markup2 = aggMarkup.Skip(1).First();
            var markup3 = aggMarkup.Skip(2).First();

            Assert.AreEqual("<br />", markup2);
            Assert.AreEqual("<br /><br />", markup3);
        }

        [Test]
        public void WhenAChildLinkIsAnAbsoluteUrl_ThenTheCorrectTargetUrlIsConstructed()
        {
            var url = "http://www.test.com";
            var urlChild1 = "http://www.test.com/child1";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"http://www.test.com/child1\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Returns("<br />");

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(2, aggMarkup.Count);

            var markup2 = aggMarkup.Skip(1).First();

            Assert.AreEqual("<br />", markup2);
        }

        [Test]
        public void WhenAChildLinkIsAnAbsoluteUrlOnHttps_ThenTheCorrectTargetUrlIsConstructed()
        {
            var url = "http://www.test.com";
            var urlChild1 = "https://www.test.com/child1";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"https://www.test.com/child1\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Returns("<br />");

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(2, aggMarkup.Count);

            var markup2 = aggMarkup.Skip(1).First();

            Assert.AreEqual("<br />", markup2);
        }

        [Test]
        public void WhenAChildLinkContainsAHashTag_ThenTheCorrectTargetUrlIsConstructed()
        {
            var url = "http://www.test.com";
            var urlChild1 = "http://www.test.com/child1";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"http://www.test.com/child1#page=1\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Returns("<br />");

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(2, aggMarkup.Count);

            var markup2 = aggMarkup.Skip(1).First();

            Assert.AreEqual("<br />", markup2);
        }

        [Test]
        public void WhenAChildLinkContainsAnInvalidPattern_ThenTheLinkIsSkipped()
        {
            var url = "http://www.test.com";
            var urlChild1 = "http://www.test.com/child1.css";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"http://www.test.com/child1.css\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Verifiable();

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(1, aggMarkup.Count);

            MockHttpAgent.Verify(ha => ha.GetWebPageMarkup(urlChild1), Times.Never);
        }

        [Test]
        public void WhenAChildLinkIsTheSameAsTheHomeLink_ThenTheLinkIsSkipped()
        {
            var url = "http://www.test.com";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"/\"></a>")
                .Verifiable();

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(1, aggMarkup.Count);

            MockHttpAgent.Verify(ha => ha.GetWebPageMarkup(url), Times.Exactly(1));
        }

        [Test]
        public void WhenTwoDuplicateLinksExistWithOneHavingAHashTag_ThenThereAreNoDuplicateCalls()
        {
            var url = "http://www.test.com";
            var urlChild1 = "http://www.test.com/child1";

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(url))
                .Returns("<a href=\"/child1\"></a><br /><a href=\"/child1#page=1\"></a>");

            MockHttpAgent
                .Setup(ha => ha.GetWebPageMarkup(urlChild1))
                .Returns("<br />")
                .Verifiable();

            var aggMarkup = GetMarkupAggregator().AggregateMarkupFromRootUrl(url);

            Assert.AreEqual(2, aggMarkup.Count);

            var markup2 = aggMarkup.Skip(1).First();

            Assert.AreEqual("<br />", markup2);

            MockHttpAgent.Verify(ha => ha.GetWebPageMarkup(urlChild1), Times.Exactly(1)); 
        }


    }
}
