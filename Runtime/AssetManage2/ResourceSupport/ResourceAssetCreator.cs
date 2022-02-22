namespace Litchi.AssetManage2
{
    public class ResourceAssetCreator : IAssetCreator
    {
        public bool Match(AssetSearchKey key)
        {
            return true;
        }

        public IAsset Create(AssetSearchKey key)
        {
            var asset = ResourceAsset.Allocate(key.assetName);
            asset.assetType = key.assetType;
            return asset;
        }
    }
}