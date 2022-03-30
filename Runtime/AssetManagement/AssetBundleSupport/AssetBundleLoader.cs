using System.Collections.Generic;
using UnityEngine;

namespace Litchi.AssetManagement
{
    public interface IAssetBundleLoader
    {
        AssetBundle Load(ulong hash);

        AssetBundleCreateRequest LoadAsync(ulong hash);

        void TryUnload(ulong hash);
    }

    public class AssetBundleLoader : IAssetBundleLoader 
    {
        private IAssetBundleDataManifest m_BundleDataManifest;

        private Dictionary<string, AssetBundleData> m_BundleDataDict = new Dictionary<string, AssetBundleData>();
        // 待卸载的AssetBundle缓存
        private LinkedList<string> m_UnloadBundleDataCache = new LinkedList<string>();

        public AssetBundleLoader(IAssetBundleDataManifest assetBundleDataManifest)
        {
            m_BundleDataManifest = assetBundleDataManifest;
        }

        public AssetBundle Load(ulong hash)
        {
            string bundleID = m_BundleDataManifest.GetBundleID(hash);
            if(bundleID == null) return null;
            AssetBundle assetBundle = null;
            AssetBundleData bundleData = LoadAssetBundleData(bundleID);
            if(bundleData != null)
            {
                assetBundle = bundleData.assetBundle;
            }
            return assetBundle;
        }

        public AssetBundleCreateRequest LoadAsync(ulong hash)
        {
            string bundleID = m_BundleDataManifest.GetBundleID(hash);
            if(bundleID == null) return null;
            // marktodo
            // 处理依赖的 LoadAsync
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync("vvv");
            return request;
        }

        public void TryUnload(ulong hash)
        {

        }

        public void TryUnloadAssetBundleData(ulong hash)
        {
            string bundleID = m_BundleDataManifest.GetBundleID(hash);
            if(bundleID == null) return;

            AssetBundleData bundleData = null;
            if(!m_BundleDataDict.TryGetValue(bundleID, out bundleData))
            {
                return;
            }

            string[] dependencies = m_BundleDataManifest.GetDirectDependencies(bundleID);
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
                AddToUnloadCache(bundleData);
            }
        }

        private void AddToUnloadCache(AssetBundleData bundleData)
        {
            if(bundleData == null || !bundleData.Unloadable()) return;
            if(m_UnloadBundleDataCache.Contains(bundleData.bundleID)) return;
            // marktodo
            const int kMaxCacheBundleCount = 20;
            if(m_UnloadBundleDataCache.Count >= kMaxCacheBundleCount)
            {
                bundleData.Retain();
                UpdateToBeUnloadedDataList(true);
                bundleData.Release();
            }
            m_UnloadBundleDataCache.AddLast(bundleData.bundleID);
        }

        private void UpdateToBeUnloadedDataList(bool force)
        {
            if(m_UnloadBundleDataCache.Count == 0) return;
            string bundleID = m_UnloadBundleDataCache.First.Value;

            AssetBundleData bundleData = null;
            m_BundleDataDict.TryGetValue(bundleID, out bundleData);

            if(bundleData == null || !bundleData.Unloadable())
            {
                m_UnloadBundleDataCache.RemoveFirst();
                return;
            }

            if(force || bundleData.TimeOut())
            {
                m_UnloadBundleDataCache.RemoveFirst();
                bundleData.Unload(false);
                m_BundleDataDict.Remove(bundleID);
            }
        }

        public AssetBundleData LoadAssetBundleData(string bundleID)
        {
            AssetBundleData bundleData = null;
            if(!m_BundleDataDict.TryGetValue(bundleID, out bundleData))
            {
                string[] dependencies = m_BundleDataManifest.GetDirectDependencies(bundleID);
                foreach (var depend in dependencies)
                {
                    LoadAssetBundleData(depend);
                }
                var assetBundle = LoadAssetBundle(bundleID);
                if(assetBundle != null)
                {
                    // bundleData = new AssetBundleData(bundleID, assetBundle);
                    // m_BundleDataDict.Add(bundleID, bundleData);
                }
            }
            bundleData?.Retain();
            return bundleData;
        }

        protected virtual AssetBundle LoadAssetBundle(string bundleID)
        {
            string path = m_BundleDataManifest.GetPath(bundleID);
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

    }
}