using UnityEngine;

namespace Litchi.AssetManage2
{
    public class AssetBundleLoaderCreator : IAssetCreator
    {
        public bool Match(AssetSearchKey key)
        {
            return key.assetType == typeof(AssetBundle);
        }

        public IAsset Create(AssetSearchKey key)
        {
            return AssetBundleLoader.Allocate(key.assetName);
        }
    }
}