using System.Collections.Generic;
using System.IO;

namespace WebEmailExtractor.WebEmailExtraction.FileHandling
{
    public class InputFileReader
    {

        protected readonly char DelimiterCharacter;


        public InputFileReader(char delimiterCharacter)
        {
            DelimiterCharacter = delimiterCharacter;
        }


        public List<InputFileItem> GetInputItemCollection(string inputFilePath)
        {
            var uriList = new List<InputFileItem>();

            using (var reader = new StreamReader(File.OpenRead(inputFilePath)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                        continue;

                    var values = line.Split(DelimiterCharacter);

                    if (values.Length < 1)
                        continue;

                    uriList.Add(new InputFileItem
                    {
                        SiteUrl = FormatUrl(values[0])
                    });
                }

                reader.Close();
            }

            return uriList;
        }

        private static string FormatUrl(string url)
        {
            url = url.TrimEnd('/');
            return url;
        }

    }
}
