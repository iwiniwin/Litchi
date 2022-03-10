using UnityEngine;

namespace Litchi.AssetManagement
{
    public interface IAssetBundleLoader
    {
        AssetBundle Load(string bundleID);
    }

    public class AssetBundleLoader : IAssetBundleLoader 
    {
        public AssetBundle Load(string bundleID)
        {
            return null;
        }
    }
}