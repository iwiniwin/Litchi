using UnityEngine;
using System.Collections.Generic;

namespace Litchi.AssetManagement
{
    public static class AssetManifest
    {
        private static Dictionary<ulong, string> m_HashToPath = new Dictionary<ulong, string>(); 

        public static ulong GetPathHash(string path)
        {
            ulong hash = HashUtility.FNV(path);

            // if(IsAssetBundleResource(hash))  // 不缓存？
            // {
            //     return 
            // }

            if(!m_HashToPath.ContainsKey(hash))
            {
                m_HashToPath.Add(hash, path);
            }
            return hash;
        }

        public static string GetHashPath(ulong hash)
        {
            string path = null;
            m_HashToPath.TryGetValue(hash, out path);
            return path;
        }
    }
}