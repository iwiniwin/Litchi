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
    public class BundleAssetLoader : IAssetLoader 
    {
        private IBundleManifest m_BundleDataManifest;
        private IAssetBundleLoader m_AssetBundleLoader;

        public BundleAssetLoader()
        {
            m_BundleDataManifest = GetBundleManifest();
            m_AssetBundleLoader = GetAssetBundleLoader(m_BundleDataManifest);
        }

        public Object Load(ulong hash, Type type)
        {
            Object asset = null;
            AssetBundle assetBundle = m_AssetBundleLoader.Load(hash);
            if(assetBundle != null)
            {
                asset = assetBundle.LoadAsset(m_BundleDataManifest.GetPathName(hash), type);
            }
            m_AssetBundleLoader.TryUnload(hash);
            return asset;
        }

        public IEnumerator LoadAsync(ulong hash, Type type, Action<Object> assetSetter)
        {
            // var Asset = loadRequest.Asset;
            // string path = AssetManifest.GetHashPath(Asset.hash);

            // AssetBundleCreateRequest bundleCreateRequest = m_AssetBundleLoader.LoadAsync(Asset.hash);

            // yield return bundleCreateRequest;

            // AssetBundle assetBundle = bundleCreateRequest.assetBundle;
            // if(assetBundle != null)
            // {
            //     string name = m_BundleDataManifest.GetPathName(Asset.hash);
            //     AssetBundleRequest request = assetBundle.LoadAssetAsync(name, Asset.type);
            //     request.priority = (int)loadRequest.priority;
            //     yield return request;
            //     Asset.asset = request.asset;
            // }

            // m_AssetBundleLoader.TryUnload(Asset.hash);
            return null;
        }
         
        protected virtual IAssetBundleLoader GetAssetBundleLoader(IBundleManifest manifest)
        {
            return new AssetBundleLoader(manifest);
        }

        protected virtual IBundleManifest GetBundleManifest()
        {
            return new BundleManifest();
        }
    }
}