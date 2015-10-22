using System;
using System.Collections.Generic;
using WebEmailExtractor.Logging;
using WebEmailExtractor.Utilities;
using WebEmailExtractor.WebEmailExtraction.ConfigProvider;
using WebEmailExtractor.WebEmailExtraction.EmailExtraction;
using WebEmailExtractor.WebEmailExtraction.FileHandling;
using WebEmailExtractor.WebEmailExtraction.Http;
using WebEmailExtractor.WebEmailExtraction.MarkupAggregation;

namespace WebEmailExtractor.WebEmailExtraction
{
    public class ExtractionManager
    {

        private readonly char _csvDelimiter;
        private readonly string _emailRegex;
        private readonly string _hrefRegex;
        private readonly string[] _invalidSiteLinkPatterns;

        protected readonly ExtractionRequest Request;
        protected readonly ExtractionResponse Response;
        protected readonly IHttpAgent HttpAgent;
        protected readonly VerboseLogger VerboseLogger;


        public ExtractionManager(
            ExtractionRequest request, 
            IExtractionConfigProvider configProvider,
            IHttpAgent httpAgent,
            VerboseLogger verboseLogger)
        {
            Request = request;
            HttpAgent = httpAgent;
            VerboseLogger = verboseLogger;

            _csvDelimiter = configProvider.GetCsvDelimiter();
            _emailRegex = configProvider.GetEmailRegex();
            _hrefRegex = configProvider.GetHrefRegex();
            _invalidSiteLinkPatterns = configProvider.GetInvalidSiteLinkPatterns();

            Response = new ExtractionResponse();
        }


        public ExtractionResponse RunExtraction()
        {
            List<InputFileItem> inputItemCollection;
            var fileReader = new InputFileReader(_csvDelimiter);

            try
            {
                inputItemCollection = fileReader.GetInputItemCollection(Request.InputFilePath);
            }
            catch (Exception ex)
            {
                Response.GeneralException = ex.GetInnerMostException();
                return Response;
            }

            var successCnt = 0;
            var markupAggregator = new MarkupAggregator(
                HttpAgent, VerboseLogger, _hrefRegex, _invalidSiteLinkPatterns);
            var emailExtractor = new EmailExtractor(_emailRegex);
            var fileWriter = new OutputFileWriter(_csvDelimiter, Request.OutputDirectory);

            foreach (var inputItem in inputItemCollection)
            {
                VerboseLogger.Log($"Extracting from {inputItem.SiteUrl} url...");

                try
                {
                    ProcessInputFileItem(inputItem, markupAggregator, emailExtractor, fileWriter);

                    VerboseLogger.Log($"Extraction completed for {inputItem.SiteUrl} url...");

                    successCnt++;
                }
                catch (Exception ex)
                {
                    var eEx = new ExtractionException("", ex) { InputUrl = inputItem.SiteUrl };

                    VerboseLogger.Log("Extraction failed. Moving to next url...");

                    Response.ExtractionExceptions = Response.ExtractionExceptions ?? new List<ExtractionException>();
                    Response.ExtractionExceptions.Add(eEx);
                }
            }

            Response.SuccessfulExtractions = successCnt;

            return Response;
        }

        private void ProcessInputFileItem(InputFileItem inputItem, MarkupAggregator markupAggregator, 
            EmailExtractor emailExtractor, OutputFileWriter fileWriter)
        {
            var aggregatedSiteMarkup = markupAggregator.AggregateMarkupFromRootUrl(inputItem.SiteUrl);

            VerboseLogger.Log($"...collected #{aggregatedSiteMarkup.Count} unique pages");

            var extractResult = emailExtractor.ExtractEmailsFromMarkup(inputItem.SiteUrl, aggregatedSiteMarkup);

            VerboseLogger.Log($"...identified #{extractResult.Emails.Count} unique emails " +
                              $"and has mailto tag = {extractResult.HasMailto}");

            fileWriter.WriteExtractResult(extractResult);
        }
    }

}
