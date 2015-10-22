using System.IO;

namespace WebEmailExtractor.Utilities
{
    public class FileUtility
    {
        public static bool FileExists(string dir)
        {
            return File.Exists(dir);
        }

        public static bool DirectoryExists(string dir)
        {
            return Directory.Exists(dir);
        }
    }
}
