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
    }
}
