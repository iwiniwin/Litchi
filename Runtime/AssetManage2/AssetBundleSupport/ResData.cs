using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Litchi.AssetManage2
{
    /// <summary>
    /// 记录所有AssetBundle数据情况，由多个AssetDataGroup组成
    /// key -> AssetDataGroup
    /// AssetDataGroup由多个AssetBundleUnit和多个AssetData组成
    /// AssetBundleUnit记录AssetBundle的名称以及依赖
    /// AssetData记录资源名称，资源加载类型，以及在AssetBundleUnit数组中的索引，所属AssetBundle名称，资源对象类型
    /// </summary>
    public class ResData : IResData
    {
        public static string FileName = "asset_bundle_config.bin";

        private Dictionary<string, List<AssetData>> m_AssetDataTable;

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

        public int AddAssetBundleName(string name, string[] dependencies, out AssetDataGroup group)
        {
            group = null;
            if(string.IsNullOrEmpty(name)) return -1;
            var key = GetKeyFromAssetBundleName(name);
            if(key == null) return -1;
            group = GetAssetDataGroup(key);
            if(group == null)
            {
                group = new AssetDataGroup(key);
                Logger.Log("#Create Config Group : " + key);
                m_AssetDataGroups.Add(group);
            }
            return group.AddAssetBundleName(name, dependencies);
        }

        public IEnumerator LoadFromFileAsync(string outRes)
        {
            return null;
        }

        public AssetData GetAssetData(AssetSearchKey key)
        {
            if(m_AssetDataTable == null)
            {
                m_AssetDataTable = new Dictionary<string, List<AssetData>>();
                for (int i = m_AssetDataGroups.Count - 1; i >= 0 ; i--)
                {
                    foreach (var assetData in m_AssetDataGroups[i].assetDatas)
                    {
                        var assetName = assetData.assetName;
                        // m_AssetDataTable.Add(assetData);
                        if(m_AssetDataTable.ContainsKey(assetName))
                        {
                            m_AssetDataTable[assetName].Add(assetData);
                        }
                        else
                        {
                            var list = new List<AssetData>();
                            list.Add(assetData);
                            m_AssetDataTable.Add(assetName, list);
                        }
                    }
                }
            }
            var name = key.assetName;
            if(m_AssetDataTable.ContainsKey(name))
            {
                var datas = m_AssetDataTable[name];
                if(key.assetBundleName != null)
                {
                    datas = datas.Where(a => a.assetBundleName == key.assetBundleName).ToList();
                }
                if(key.assetType != null)
                {
                    var code = key.assetType.ToCode();
                    if(code == 0)
                    {

                    }
                    else
                    {
                        var newDatas = datas.Where(a => a.assetObjectTypeCode == code);
                        // 有可能是从旧的AssetBundle中加载出来的资源
                        if(newDatas.Any())
                        {
                            datas = newDatas.ToList();
                        }
                    }
                }
                return datas.FirstOrDefault();
            }
            return null;
        }

        private AssetDataGroup GetAssetDataGroup(string key)
        {
            for (int i = m_AssetDataGroups.Count - 1; i >= 0 ; i--)
            {
                if(m_AssetDataGroups[i].key.Equals(key))
                {
                    return m_AssetDataGroups[i];
                }
            }
            return null;
        }

        private static string GetKeyFromAssetBundleName(string name)
        {
            var index = name.IndexOf('/');
            if(index < 0) return name;
            var key = name.Substring(0, index);
            return key;
        }
    }
}