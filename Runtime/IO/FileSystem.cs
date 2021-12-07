using System.IO;

namespace Litchi.IO
{
    public static class FileSystem
    {
        public static void EnsureParentDirExists(string path) {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }
    }
}