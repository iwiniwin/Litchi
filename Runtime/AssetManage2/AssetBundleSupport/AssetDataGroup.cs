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
            SetSerializeData(data);
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
            AddAssetData(new AssetData(name, (short)AssetLoadType.AssetBundle, index, null));
            return index;
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

        public void AddAssetData(AssetData data)
        {
            if(m_AssetDataMap == null)
            {
                m_AssetDataMap = new Dictionary<string, AssetData>();
            }
            if(m_UUID4AssetData == null)
            {
                m_UUID4AssetData = new Dictionary<string, AssetData>();
            }

            string key = data.assetName;
            if(m_AssetDataMap.ContainsKey(key))
            {
                var searchKey = AssetSearchKey.Allocate(key);
                var old = GetAssetData(searchKey);
                searchKey.Recycle();
                try
                {
                    Logger.Info(string.Format("Already Add AssetData : {0} \n OldAB : {1}    NewAB : {2}", 
                        data.assetName, m_AssetBundleUnits[old.assetBundleIndex].name, m_AssetBundleUnits[data.assetBundleIndex].name));
                }
                catch(Exception e)
                {
                    Logger.Error(e);
                }
            }
            else
            {
                m_AssetDataMap.Add(key, data);
            }

            if(m_UUID4AssetData.ContainsKey(data.UUID))
            {
                var searchKey = AssetSearchKey.Allocate(data.assetName, data.assetBundleName);
                var old = GetAssetData(searchKey);
                searchKey.Recycle();

                try
                {
                    Logger.Info(string.Format("Already Add AssetData : {0} \n OldAB : {1}    NewAB : {2}", 
                        data.UUID, m_AssetBundleUnits[old.assetBundleIndex].name, m_AssetBundleUnits[data.assetBundleIndex].name));
                }
                catch(Exception e)
                {
                    Logger.Error(e);
                }
            }
            else
            {
                m_UUID4AssetData.Add(data.UUID, data);
            }
        }

        public bool GetAssetBundleName(string assetName, int index, out string result)
        {
            result = null;
            if(m_AssetBundleUnits == null) return false;
            if(index >= m_AssetBundleUnits.Count) return false;
            if(m_AssetDataMap.ContainsKey(assetName))
            {
                result = m_AssetBundleUnits[index].name;
                return true;
            }
            return false;
        }

        public AssetBundleUnit GetAssetBundleUnit(string assetName)
        {
            var key = AssetSearchKey.Allocate(assetName);
            AssetData data = GetAssetData(key);
            key.Recycle();

            if(data == null)
            {
                return null;
            }
            if(m_AssetBundleUnits == null)
            {
                return null;
            }
            return m_AssetBundleUnits[data.assetBundleIndex];
        }

        public bool GetAssetBundleDependencies(string assetName, out string[] result)
        {
            result = null;
            AssetBundleUnit unit = GetAssetBundleUnit(assetName);
            if(unit == null) return false;
            result = unit.dependencies;
            return true;
        }

        public SerializeData GetSerializeData()
        {
            var sd = new SerializeData();
            sd.key = key;
            sd.assetBundleUnits = m_AssetBundleUnits.ToArray();
            if(m_AssetDataMap != null)
            {
                AssetData[] array = new AssetData[m_AssetDataMap.Count];
                int index = 0;
                foreach (var item in m_AssetDataMap)
                {   
                    array[index ++] = item.Value;
                }
                sd.assetDatas = array;
            }
            return sd;
        }

        private void SetSerializeData(SerializeData data)
        {
            if(data == null) return;
            m_AssetBundleUnits = new List<AssetBundleUnit>(data.assetBundleUnits);

            if(data.assetDatas != null)
            {
                m_AssetDataMap = new Dictionary<string, AssetData>();
                foreach (var item in data.assetDatas)
                {
                    AddAssetData(item);
                }
            }
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