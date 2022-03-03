namespace Litchi.AssetManage2
{
    public interface IAssetCreator
    {
        bool Match(AssetSearchKey key);
        IAsset Create(AssetSearchKey key);
    }

    public class AssetFactory 
    {
        public static IAsset Create(AssetSearchKey key)
        {
            // return new ResourceAssetCreator().Create(key);
            return new AssetBundleAssetCreator().Create(key);

            // marktodo AssetBundleLoaderFactor
        }
    }
}