using UnityEngine;
using System.Collections.Generic;

namespace Litchi.AssetManagement
{
    public interface IAssetBundleDataReader
    {
        string GetBundleID(ulong hash);
        string[] GetDirectDependencies(string bundleID);
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
    }
}