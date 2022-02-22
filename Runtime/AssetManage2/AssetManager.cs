using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Litchi.AssetManage2
{
    public class AssetManager : MonoSingleton<AssetManager>
    {

        private static bool m_Inited;
        public void Init()
        {
            if(m_Inited) return;
            m_Inited = true;

            ObjectPool<ResourceAsset>.instance.Init(40, 20);
        }

        public void InitAssetBundleConfig()
        {
            var simulate = false;
            if(simulate)
            {

            }
            else
            {
                AssetBundleSettings.AssetBundleConfigFile.Reset();

                var outResult = new List<string>();

                var hotfix = false;  // 进行过热更
                if(hotfix)
                {

                }
                else
                {
                    var fileInfos = FileUtility.GetFiles(AssetBundlePathHelper.PersistentDataPath, ResData.FileName);
                    outResult.AddRange(fileInfos.Select(fileInfo => fileInfo.Name));
                }

                foreach (var outRes in outResult)
                {
                    AssetBundleSettings.AssetBundleConfigFile.LoadFromFile(outRes);
                }
            }
        }

        // private AssetTable m_Table = new AssetTable();
        // marktodo
        private Dictionary<string, IAsset> m_Table = new Dictionary<string, IAsset>();

        public IAsset GetAsset(AssetSearchKey key, bool createNew = false)
        {
            // var asset = m_Table.GetAssetBySearchKey(key);
            IAsset asset;
            m_Table.TryGetValue(key.ToString(), out asset);
            if(asset != null) return asset;

            if(!createNew)
            {
                return null;
            }

            asset = AssetFactory.Create(key);
            if(asset != null)
            {
                // m_Table.Add(asset);
                m_Table.Add(key.ToString(), asset);
            }
            return asset;
        }
    }
}