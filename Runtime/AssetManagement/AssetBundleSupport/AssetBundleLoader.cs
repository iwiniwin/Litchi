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
        private IBundleManifest m_BundleDataManifest;

        private Dictionary<string, Bundle> m_BundleDataDict = new Dictionary<string, Bundle>();
        // 待卸载的AssetBundle缓存
        private LinkedList<string> m_UnloadBundleDataCache = new LinkedList<string>();

        public AssetBundleLoader(IBundleManifest BundleManifest)
        {
            m_BundleDataManifest = BundleManifest;
        }

        public AssetBundle Load(ulong hash)
        {
            string bundleID = m_BundleDataManifest.GetBundleID(hash);
            if(bundleID == null) return null;
            AssetBundle assetBundle = null;
            Bundle bundleData = LoadBundle(bundleID);
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

        public void TryUnloadBundle(ulong hash)
        {
            string bundleID = m_BundleDataManifest.GetBundleID(hash);
            if(bundleID == null) return;

            Bundle bundleData = null;
            if(!m_BundleDataDict.TryGetValue(bundleID, out bundleData))
            {
                return;
            }

            string[] dependencies = m_BundleDataManifest.GetDirectDependencies(bundleID);
            Bundle dependBundleData = null;
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

        private void AddToUnloadCache(Bundle bundleData)
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

            Bundle bundleData = null;
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

        public Bundle LoadBundle(string bundleID)
        {
            Bundle bundleData = null;
            if(!m_BundleDataDict.TryGetValue(bundleID, out bundleData))
            {
                string[] dependencies = m_BundleDataManifest.GetDirectDependencies(bundleID);
                foreach (var depend in dependencies)
                {
                    LoadBundle(depend);
                }
                var assetBundle = LoadAssetBundle(bundleID);
                if(assetBundle != null)
                {
                    // bundleData = new Bundle(bundleID, assetBundle);
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
                Logger.ErrorFormat("[BundleAssetLoader] load assetbundle failed, path = {0}", path);
            }
            else
            {
                bundle.name = bundleID;
            }
            return bundle;
        }

    }
}