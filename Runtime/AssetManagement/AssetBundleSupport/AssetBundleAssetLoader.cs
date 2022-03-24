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
        private IAssetBundleDataManifest m_BundleDataManifest;
        private IAssetBundleLoader m_AssetBundleLoader;

        public AssetBundleAssetLoader()
        {
            m_BundleDataManifest = GetAssetBundleDataManifest();
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

        public AssetLoadRequest LoadAsync(ulong hash, Type type)
        {
            // var assetData = loadRequest.assetData;
            // string path = AssetDataManifest.GetHashPath(assetData.hash);

            // AssetBundleCreateRequest bundleCreateRequest = m_AssetBundleLoader.LoadAsync(assetData.hash);

            // yield return bundleCreateRequest;

            // AssetBundle assetBundle = bundleCreateRequest.assetBundle;
            // if(assetBundle != null)
            // {
            //     string name = m_BundleDataManifest.GetPathName(assetData.hash);
            //     AssetBundleRequest request = assetBundle.LoadAssetAsync(name, assetData.type);
            //     request.priority = (int)loadRequest.priority;
            //     yield return request;
            //     assetData.asset = request.asset;
            // }

            // m_AssetBundleLoader.TryUnload(assetData.hash);
            return null;
        }
         
        protected virtual IAssetBundleLoader GetAssetBundleLoader(IAssetBundleDataManifest manifest)
        {
            return new AssetBundleLoader(manifest);
        }

        protected virtual IAssetBundleDataManifest GetAssetBundleDataManifest()
        {
            return new AssetBundleDataManifest();
        }
    }
}