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

        private Dictionary<string, List<IAsset>> m_Table = new Dictionary<string, List<IAsset>>();

        public IAsset GetOrCreateAsset(AssetSearchKey key)
        {
            IAsset asset = GetAsset(key);
            if(asset != null) return asset;

            asset = AssetFactory.Create(key);
            if(asset != null)
            {
                var name = key.assetName;
                if(m_Table.ContainsKey(name))
                {
                    m_Table[name].Add(asset);
                }
                else
                {
                    var list = new List<IAsset>();
                    list.Add(asset);
                    m_Table.Add(name, list);
                }
            }
            return asset;
        }

        public IAsset GetAsset(AssetSearchKey key)
        {
            List<IAsset> assets;
            if(m_Table.TryGetValue(key.assetName, out assets))
            {
                if(key.assetType != null)
                {
                    assets = assets.Where(asset => asset.assetType == key.assetType).ToList();
                }
                if(key.assetBundleName != null)
                {
                    assets = assets.Where(asset => asset.assetBundleName == key.assetBundleName).ToList();
                }
                return assets.FirstOrDefault();
            }
            return null;
        }

        public T GetAsset<T>(AssetSearchKey key) where T : class, IAsset
        {
            return GetAsset(key) as T;
        }
    }

    // todomark 优化  不要直接用IEnumeratorTask
    public class LoadTaskManager : TaskManager<IEnumeratorTask>, ISingleton
    {
        public static new LoadTaskManager instance
        {
            get
            {
                return MonoSingletonProperty<LoadTaskManager>.instance;
            }
        }
    }
}