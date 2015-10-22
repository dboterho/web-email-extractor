using System.IO;

namespace WebEmailExtractor.WebEmailExtraction.FileHandling
{
    public static class PathValidator
    {

        public static bool DirectoryExists(string dir)
        {
            return File.Exists(dir);
        }

    }
}
