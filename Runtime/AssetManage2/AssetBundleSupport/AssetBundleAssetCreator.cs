namespace Litchi.AssetManage2
{
    public class AssetBundleAssetCreator : IAssetCreator
    {
        public bool Match(AssetSearchKey key)
        {
            var assetData = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(key);
            if(assetData != null)
            {
                return assetData.assetType == (short)AssetLoadType.AssetBundleAsset;
            }
            // foreach (var subFile in AssetBundleSettings.sub)
            // {
                
            // }
            return false;
        }

        public IAsset Create(AssetSearchKey key)
        {
            return AssetBundleAsset.Allocate(key.assetName, key.assetBundleName, key.assetType);
        }
    }
}