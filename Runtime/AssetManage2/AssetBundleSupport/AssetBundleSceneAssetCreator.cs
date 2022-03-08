namespace Litchi.AssetManage2
{
    public class AssetBundleSceneAssetCreator : IAssetCreator
    {
        public bool Match(AssetSearchKey key)
        {
            var assetData = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(key);
            if(assetData != null)
            {
                return assetData.assetType == (short)AssetLoadType.AssetBundleScene;
            }
            return false;
        }

        public IAsset Create(AssetSearchKey key)
        {
            return AssetBundleSceneAsset.Allocate(key.assetName);
        }
    }
}