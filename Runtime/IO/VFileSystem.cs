using System.IO;

namespace Litchi.IO
{
    public static class VFileSystem
    {
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        public static string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public static void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}