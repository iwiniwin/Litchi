using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public class AssetBundleLoader : IAssetLoader
    {
        public Object LoadAsset(string path, Type type)
        {
            // assetName, bundleName = TryGetAssetBundleName();
            // bundle = LoadBundle(abName, path, abName)
            // asset = bundle.LoadAsset()
            // return asset;
            return null;
        }

        public AssetRequest LoadAssetAsync(string path, Type type)
        {
            return null;
        }

        public void UnloadAsset(Object asset)
        {
            
        }

        public void UnloadAllAssets()
        {
            // AssetDatabase.unload
        }
    }
}