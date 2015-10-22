using System;
using WebEmailExtractor.Logging;
using WebEmailExtractor.Utilities;
using WebEmailExtractor.WebEmailExtraction;
using WebEmailExtractor.WebEmailExtraction.ConfigProvider;
using WebEmailExtractor.WebEmailExtraction.Http;

namespace WebEmailExtractor
{
    class Program
    {

        private static void Main(string[] args)
        {
            var inputFilePath = GetInputFilePath();
            var outputDirectory = GetOutputDirectory();
            var verboseEnabled = GetVerboseLoggingEnabled();

            var verboseLogger = new VerboseLogger(verboseEnabled, Console.WriteLine);
            var configProvider = new AppConfigProvider();
            var httpWebRequestAgent = new HttpWebRequestAgent(verboseLogger);

            var extractor = new ExtractionManager(new ExtractionRequest
            {
                InputFilePath = inputFilePath,
                OutputDirectory = outputDirectory,
                VerboseLogger = verboseLogger
            }, configProvider, httpWebRequestAgent, verboseLogger);

            var response = extractor.RunExtraction();

            if (!string.IsNullOrEmpty(response.GeneralException))
            {
                Console.WriteLine($"Process failed. Error: {response.GeneralException}.");
                Console.ReadKey();
                return;
            }

            if (response.HasExtractionException)
            {
                foreach (var extrctException in response.ExtractionExceptions)
                {
                    Console.WriteLine(
                        $"Extraction did not complete for url: {extrctException.InputUrl}. " +
                        $"Error: {extrctException}");
                }
                Console.ReadKey();
            }

            Console.WriteLine($"Process complete. {response.SuccessfulExtractions} website(s) successfully processed.");
            Console.ReadKey();

        }

        private static string GetInputFilePath()
        {
            Console.WriteLine("Provide the input file path...");
            var inputFilePath = Console.ReadLine();

            if (FileUtility.FileExists(inputFilePath))
                return inputFilePath;

            Console.WriteLine("File does not exist.");
            return GetInputFilePath();
        }

        private static string GetOutputDirectory()
        {
            Console.WriteLine("Provide the output file directory...");
            var outputDirectory = Console.ReadLine();

            if (FileUtility.DirectoryExists(outputDirectory))
                return outputDirectory;

            Console.WriteLine("Directory does not exist.");
            return GetOutputDirectory();
        }

        private static bool GetVerboseLoggingEnabled()
        {
            Console.WriteLine("Verbose logging? (Y/N)");
            var verboseLogKey = Console.ReadKey();
            Console.WriteLine("");
            var verboseEnabled = verboseLogKey.KeyChar == 'y' || verboseLogKey.KeyChar == 'Y';
            return verboseEnabled;
        }

    }

}
