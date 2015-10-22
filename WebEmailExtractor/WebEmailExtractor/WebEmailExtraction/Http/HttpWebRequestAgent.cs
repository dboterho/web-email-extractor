using System;
using System.IO;
using System.Net;
using WebEmailExtractor.Logging;
using WebEmailExtractor.Utilities;

namespace WebEmailExtractor.WebEmailExtraction.Http
{
    public class HttpWebRequestAgent : IHttpAgent
    {

        protected readonly VerboseLogger VerboseLogger;


        public HttpWebRequestAgent(
            VerboseLogger verboseLogger)
        {
            VerboseLogger = verboseLogger;
        }


        public string GetWebPageMarkup(string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(new Uri(url));
                request.Timeout = 5000;
                request.Referer = "http://www.google.com";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.71 Safari/537.36";
                request.Method = WebRequestMethods.Http.Get;

                VerboseLogger.LogVerbose($"getting web page markup for url {url}");

                var response = request.GetResponse();
                var responseStream = response.GetResponseStream();

                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream))
                {
                    var markup = reader.ReadToEnd();

                    response.Close();

                    return markup;
                }
            }
            catch (Exception ex)
            {
                VerboseLogger.LogVerbose($"GET request failed. Error: {ex.GetInnerMostException()}");

                return "";
            }
        }

    }
}
