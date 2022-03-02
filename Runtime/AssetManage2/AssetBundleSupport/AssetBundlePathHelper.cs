using System.Collections.Generic;
using UnityEngine;

namespace Litchi.AssetManage2
{
    public class AssetBundlePathHelper
    {
        public static string PersistentDataPath
        {
            get;
        } = Application.persistentDataPath + "/";

        public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
        {

        }

        // marktodo
        public static string GetPlatformName()
        {
            // return GetPlatformName
            return "Windows";
        }

        // marktodo
        public static string StreamingAssetsPath
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }
    }
}
