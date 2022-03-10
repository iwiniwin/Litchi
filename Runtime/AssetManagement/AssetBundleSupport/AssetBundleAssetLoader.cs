using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleAssetLoader : IAssetLoader 
    {
        private Dictionary<string, AssetBundleData> m_BundleDataCache = new Dictionary<string, AssetBundleData>();

        public Object Load(ulong hash, Type type)
        {
            string bundleID = m_BundleDataReader?.GetBundleID(hash);
            if(bundleID == null) return null;
            AssetBundleData bundleData = LoadAssetBundleData(bundleID);
            if(bundleData != null && bundleData.assetBundle != null)
            {
                // bundleData.assetBundle.LoadAsset(name, type)
            }
            // Unload()
            return null;
        }

        public AssetBundleData LoadAssetBundleData(string bundleID)
        {
            AssetBundleData bundleData = null;
            if(!m_BundleDataCache.TryGetValue(bundleID, out bundleData))
            {
                string[] dependencies = m_BundleDataReader?.GetDirectDependencies(bundleID);
                foreach (var depend in dependencies)
                {
                    LoadAssetBundleData(depend);
                }
                var assetBundle = GetAssetBundleLoader().Load(bundleID);
                if(assetBundle != null)
                {
                    bundleData = new AssetBundleData(assetBundle);
                    m_BundleDataCache.Add(bundleID, bundleData);
                }
            }
            bundleData?.Retain();
            return bundleData;
        }

        public IEnumerator LoadAsync(AssetLoadHandle loadHandle)
        {
            var assetData = loadHandle.assetData;
            string path = AssetDataManifest.GetHashPath(assetData.hash);
            ResourceRequest request = Resources.LoadAsync(path, assetData.type);
            request.priority = (int)loadHandle.priority;
            yield return request;
            assetData.asset = request.asset;
        }
         
        private IAssetBundleLoader m_AssetBundleLoader;
        private IAssetBundleLoader GetAssetBundleLoader()
        {
            if(m_AssetBundleLoader == null)
            {
                m_AssetBundleLoader = CreateAssetBundleLoader();
            }
            return m_AssetBundleLoader;
        }

        private IAssetBundleDataReader m_BundleDataReader;
        private IAssetBundleDataReader GetAssetBundleDataReader()
        {
            if(m_BundleDataReader == null)
            {
                m_BundleDataReader = CreateAssetBundleDataReader();
            }
            return m_BundleDataReader;
        }

        protected virtual IAssetBundleLoader CreateAssetBundleLoader()
        {
            return new AssetBundleLoader();
        }

        protected virtual IAssetBundleDataReader CreateAssetBundleDataReader()
        {
            return new AssetBundleDataReader();
        }
    }
}