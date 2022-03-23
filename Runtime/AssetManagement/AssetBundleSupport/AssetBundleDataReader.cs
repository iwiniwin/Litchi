using UnityEngine;
using System.Collections.Generic;

namespace Litchi.AssetManagement
{
    public interface IAssetBundleDataReader
    {
        string GetBundleID(ulong hash);
        string[] GetDirectDependencies(string bundleID);
        string GetPath(string bundleID);
        string GetPathName(ulong hash);
    }
    public class AssetBundleDataReader : IAssetBundleDataReader
    {
        public string GetBundleID(ulong hash)
        {
            return null;
        }

        public string[] GetDirectDependencies(string bundleID)
        {
            return null;
        }

        public string GetPath(string bundleID)
        {
            return null;
        }

        public string GetPathName(ulong hash)
        {
            return null;
        }
    }
}