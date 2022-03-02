using System;
using System.Collections;
using System.Collections.Generic;

namespace Litchi.AssetManage2
{
    public class AssetDataGroup
    {
        public string key { get; private set; }
        private List<AssetBundleUnit> m_AssetBundleUnits;
        private Dictionary<string, AssetData> m_AssetDataMap;
        private Dictionary<string, AssetData> m_UUID4AssetData;

        public IEnumerable<AssetData> assetDatas
        {
            get { return m_AssetDataMap.Values; }
        }

        public IEnumerable<AssetBundleUnit> assetBundleUnits
        {
            get { return m_AssetBundleUnits; }
        }

        public AssetDataGroup(string key)
        {
            this.key = key;
        }

        public AssetDataGroup(SerializeData data)
        {
            this.key = data.key;
            // SetSerializeData
        }

        public void Reset()
        {
            if(m_AssetBundleUnits != null)
            {
                m_AssetBundleUnits.Clear();
            }
            if(m_AssetDataMap != null)
            {
                m_AssetDataMap.Clear();
            }
        }

        public int AddAssetBundleName(string name, string[] dependencies)
        {
            if(string.IsNullOrEmpty(name)) return -1;
            if(m_AssetBundleUnits == null)
            {
                m_AssetBundleUnits = new List<AssetBundleUnit>();
            }
            var key = AssetSearchKey.Allocate(name);
            AssetData config = GetAssetData(key);
            key.Recycle();

            if(config != null)
            {
                return config.assetBundleIndex;
            }

            m_AssetBundleUnits.Add(new AssetBundleUnit(name, dependencies));

            int index = m_AssetBundleUnits.Count - 1;
            // AddAssetData(new AssetData(name, AssetLoadType.AssetBundle, index, null));
            // return index;

            return 0;
        }

        public AssetData GetAssetData(AssetSearchKey key)
        {
            AssetData data = null;
            if(key.assetBundleName != null && m_UUID4AssetData != null)
            {
                return m_UUID4AssetData.TryGetValue(key.assetBundleName + key.assetName, out data) ? data : null;
            }
            if(key.assetBundleName != null && m_AssetDataMap != null)
            {
                return m_AssetDataMap.TryGetValue(key.assetName, out data) ? data : null;
            }
            return data;
        }

        [Serializable]
        public class AssetBundleUnit
        {
            public string name;
            public string[] dependencies;

            public AssetBundleUnit(string name, string[] dependencies)
            {
                this.name = name;
                this.dependencies = dependencies;
            }

            public override string ToString()
            {
                var result = string.Format("asset bundle name : {0}", name);
                if(dependencies == null)
                {
                    return result;
                }
                foreach (var depend in dependencies)
                {
                    result += string.Format(" #:{0}", depend);
                }
                return result;
            }
        }

        [Serializable]
        public class SerializeData
        {
            public string key { get; set; }
            public AssetBundleUnit[] assetBundleUnits { get; set; }
            public AssetData[] assetDatas { get; set; }
        }
    }
}