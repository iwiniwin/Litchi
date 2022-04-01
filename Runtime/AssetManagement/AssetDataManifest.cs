using UnityEngine;
using System.Collections.Generic;

namespace Litchi.AssetManagement
{
    public static class AssetManifest
    {
        private static Dictionary<string, string> m_HashToPath = new Dictionary<string, string>(); 

        public static string GetPathHash(string path)
        {

            // if(IsAssetBundleResource(path))  // 不缓存？
            // {
            //     return 
            // }

            if(!m_HashToPath.ContainsKey(path))
            {
                m_HashToPath.Add(path, path);
            }
            return path;
        }

        public static string GetHashPath(string path)
        {
            m_HashToPath.TryGetValue(path, out path);
            return path;
        }
    }
}