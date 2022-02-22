using System.IO;
using System.Collections.Generic;

namespace Litchi
{
    public static class FileUtility
    {
        public static FileInfo[] GetFiles(string dirName, string searchPattern = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var dir = new DirectoryInfo(dirName);
            var fileInfos = dir.GetFiles(searchPattern, searchOption);
            return fileInfos;
        }
    }
}