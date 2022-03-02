using System.Collections;
using System.Collections.Generic;

namespace Litchi.AssetManage2
{
    public class ResData : IResData
    {
        public static string FileName = "asset_bundle_config.bin";

        private Dictionary<string, List<IAsset>> m_AssetDataTable;

        private readonly List<AssetDataGroup> m_AssetDataGroups = new List<AssetDataGroup>();

        public string[] GetAllDependencies(string url)
        {
            return null;
        }

        public void LoadFromFile(string outRes)
        {

        }

        public void Reset()
        {

        }

        public IEnumerator LoadFromFileAsync(string outRes)
        {
            return null;
        }

        public AssetData GetAssetData(AssetSearchKey key)
        {
            // if(m_AssetDataTable == null)
            // {
            //     m_AssetDataTable = new Dictionary<string, List<IAsset>>();
            //     for (int i = m_AssetDataGroups.Length - 1; i >= 0 ; i--)
            //     {
            //         foreach (var assetData in m_AssetDataGroups[i].assetDatas)
            //         {
            //             m_AssetDataTable.Add(assetData);
            //         }
            //     }
            // }
            // return m_AssetDataTable.Get(key);
            return null;
        }

        public int AddAssetBundleName(string abName, string[] depends)
        {
            return 0;
        }
    }
}