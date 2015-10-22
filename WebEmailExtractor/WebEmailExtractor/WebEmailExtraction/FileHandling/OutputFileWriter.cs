using System;
using System.IO;

namespace WebEmailExtractor.WebEmailExtraction.FileHandling
{
    public class OutputFileWriter
    {

        protected readonly char DelimiterCharacter;
        protected readonly string OutputFilePath;


        public OutputFileWriter(
            char delimiterCharacter,
            string outputDirectory)
        {
            DelimiterCharacter = delimiterCharacter;

            OutputFilePath = $@"{outputDirectory}\ExtractionResult_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv";
        }


        public void WriteExtractResult(EmailExtractResult extractResult)
        {
            var emailsCombined = string.Join(";", extractResult.Emails);

            using (var output = new StreamWriter(OutputFilePath, true))
            {
                output.WriteLine($"{extractResult.SiteUrl}{DelimiterCharacter}" +
                                 $"{emailsCombined}{DelimiterCharacter}" +
                                 $"{extractResult.HasMailto}");

                output.Close();
            }
        }

    }
}
