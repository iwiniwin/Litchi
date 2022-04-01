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

        public Object Load(string path, Type type)
        {
            Object asset = null;
            AssetBundle assetBundle = m_AssetBundleLoader.Load(path);
            if(assetBundle != null)
            {
                asset = assetBundle.LoadAsset(m_BundleDataManifest.GetPathName(path), type);
            }
            m_AssetBundleLoader.TryUnload(path);
            return asset;
        }

        public IEnumerator LoadAsync(string path, Type type, Action<Object> assetSetter)
        {
            // var agent = loadRequest.agent;

            // AssetBundleCreateRequest bundleCreateRequest = m_AssetBundleLoader.LoadAsync(agent.path);

            // yield return bundleCreateRequest;

            // AssetBundle assetBundle = bundleCreateRequest.assetBundle;
            // if(assetBundle != null)
            // {
            //     string name = m_BundleDataManifest.GetPathName(agent.path);
            //     AssetBundleRequest request = assetBundle.LoadAssetAsync(name, agent.type);
            //     request.priority = (int)loadRequest.priority;
            //     yield return request;
            //     agent.asset = request.asset;
            // }

            // m_AssetBundleLoader.TryUnload(agent.path);
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