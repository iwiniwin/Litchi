using UnityEngine;
using System.Collections.Generic;

namespace Litchi.AssetManagement
{
    public interface IAssetBundleDataManifest
    {
        string GetBundleID(ulong hash);
        string[] GetDirectDependencies(string bundleID);
        string GetPath(string bundleID);
        string GetPathName(ulong hash);
    }
    public class AssetBundleDataManifest : IAssetBundleDataManifest
    {
        public string GetBundleID(ulong hash)
        {
            return "null";
        }

        public string[] GetDirectDependencies(string bundleID)
        {
            return new string[]{};
        }

        public string GetPath(string bundleID)
        {
            return "jjjj " + bundleID;
        }

        public string GetPathName(ulong hash)
        {
            return null;
        }
    }
}