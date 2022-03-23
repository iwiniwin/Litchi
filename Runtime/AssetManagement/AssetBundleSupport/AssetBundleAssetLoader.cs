using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// todo
/// path与path name对应
/// GetDirectDependencies仅有直接依赖
/// </summary>
namespace Litchi.AssetManagement
{
    public class AssetBundleAssetLoader : IAssetLoader 
    {
        private Dictionary<string, AssetBundleData> m_BundleDataDict = new Dictionary<string, AssetBundleData>();
        private LinkedList<string> m_ToBeUnloadedDataCache = new LinkedList<string>();

        public Object Load(ulong hash, Type type)
        {
            Object asset = null;
            string bundleID = GetAssetBundleDataReader().GetBundleID(hash);
            if(bundleID == null) return null;
            AssetBundleData bundleData = LoadAssetBundleData(bundleID);
            if(bundleData != null && bundleData.assetBundle != null)
            {
                asset = bundleData.assetBundle.LoadAsset(GetAssetBundleDataReader().GetPathName(hash), type);
            }
            TryUnloadAssetBundleData(hash);
            return asset;
        }

        public void TryUnloadAssetBundleData(ulong hash)
        {
            string bundleID = GetAssetBundleDataReader().GetBundleID(hash);
            if(bundleID == null) return;

            AssetBundleData bundleData = null;
            if(!m_BundleDataDict.TryGetValue(bundleID, out bundleData))
            {
                return;
            }

            string[] dependencies = GetAssetBundleDataReader().GetDirectDependencies(bundleID);
            AssetBundleData dependBundleData = null;
            // marktodo 依赖卸载不用递归？
            foreach (var depend in dependencies)
            {
                if(m_BundleDataDict.TryGetValue(depend, out dependBundleData))
                {
                    dependBundleData.Release();
                }
            }
            bundleData.Release();

            if(bundleData.Unloadable())
            {
                UnloadBundleData(bundleData);
            }
        }

        private void UnloadBundleData(AssetBundleData bundleData)
        {
            if(bundleData == null || !bundleData.Unloadable()) return;
            if(m_ToBeUnloadedDataCache.Contains(bundleData.bundleID)) return;
            // marktodo
            const int kMaxCacheBundleCount = 20;
            if(m_ToBeUnloadedDataCache.Count >= kMaxCacheBundleCount)
            {
                bundleData.Retain();
                UpdateToBeUnloadedDataList(true);
                bundleData.Release();
            }
            m_ToBeUnloadedDataCache.AddLast(bundleData.bundleID);
        }

        private void UpdateToBeUnloadedDataList(bool force)
        {
            if(m_ToBeUnloadedDataCache.Count == 0) return;
            string bundleID = m_ToBeUnloadedDataCache.First.Value;

            AssetBundleData bundleData = null;
            m_BundleDataDict.TryGetValue(bundleID, out bundleData);

            if(bundleData == null || !bundleData.Unloadable())
            {
                m_ToBeUnloadedDataCache.RemoveFirst();
                return;
            }

            if(force || bundleData.TimeOut())
            {
                m_ToBeUnloadedDataCache.RemoveFirst();
                bundleData.Unload(false);
                m_BundleDataDict.Remove(bundleID);
            }
        }

        public AssetBundleData LoadAssetBundleData(string bundleID)
        {
            AssetBundleData bundleData = null;
            if(!m_BundleDataDict.TryGetValue(bundleID, out bundleData))
            {
                string[] dependencies = GetAssetBundleDataReader().GetDirectDependencies(bundleID);
                foreach (var depend in dependencies)
                {
                    LoadAssetBundleData(depend);
                }
                var assetBundle = LoadAssetBundle(bundleID);
                if(assetBundle != null)
                {
                    bundleData = new AssetBundleData(bundleID, assetBundle);
                    m_BundleDataDict.Add(bundleID, bundleData);
                }
            }
            bundleData?.Retain();
            return bundleData;
        }

        protected virtual AssetBundle LoadAssetBundle(string bundleID)
        {
            string path = GetAssetBundleDataReader().GetPath(bundleID);
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            if(bundle == null)
            {
                Logger.ErrorFormat("[AssetBundleAssetLoader] load assetbundle failed, path = {0}", path);
            }
            else
            {
                bundle.name = bundleID;
            }
            return bundle;
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