using System.Linq;
using System.Collections.Generic;

namespace Litchi.AssetManage2
{
    public interface IAssetCreator
    {
        bool Match(AssetSearchKey key);
        IAsset Create(AssetSearchKey key);
    }

    public class AssetFactory 
    {
        private static List<IAssetCreator> m_AssetCreators = new List<IAssetCreator>()
        {
            new AssetBundleAssetCreator(),
            new AssetBundleLoaderCreator(),
        };

        public static void AddAssetCreator(IAssetCreator creator)
        {
            m_AssetCreators.Add(creator);
        }

        public static AssetBundleSceneAssetCreator assetBundleSceneAssetCreator = new AssetBundleSceneAssetCreator();
        
        public static IAsset Create(AssetSearchKey key)
        {
            // return new ResourceAssetCreator().Create(key);
            // return new AssetBundleAssetCreator().Create(key);

            // marktodo AssetBundleLoaderFactor
            var creator = m_AssetCreators.Where(c => c.Match(key)).FirstOrDefault();
            UnityEngine.Debug.Log("[AssetFactory] select creator ..... " + " " + creator.GetType().FullName);
            var res = creator.Create(key);
            if(res == null)
            {
                Logger.Log("Failed to Create AssetLoader. Not Find By AssetSearchKey : " + key);
            }
            return res;            
        }
    }
}