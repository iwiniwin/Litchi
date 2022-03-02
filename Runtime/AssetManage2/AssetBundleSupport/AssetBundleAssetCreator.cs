namespace Litchi.AssetManage2
{
    public class AssetBundleAssetCreator : IAssetCreator
    {
        public bool Match(AssetSearchKey key)
        {
            return true;
        }

        public IAsset Create(AssetSearchKey key)
        {
            return AssetBundleAsset.Allocate(key.assetName, key.assetBundleName, key.assetType);
        }
    }
}