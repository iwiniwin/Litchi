using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace Litchi.AssetManage
{
    public class AssetBundleLoader : IAssetLoader
    {
        private Dictionary<string, string> m_AssetBundleMap;
        public AssetBundleLoader()
        {
            m_AssetBundleMap = AssetBundleUtility.LoadAssetBundleMap("Test/str.xml");
        }

        public Object LoadAsset(string path, Type type)
        {
            string bundleName = TryGetAssetBundleName(path);
            if(bundleName == null)
            {
                return null;
            }
            var bundle = LoadBundle(bundleName);
            var asset = bundle.LoadAsset(path);
            return asset;
        }

        public AssetRequest LoadAssetAsync(string path, Type type)
        {
            var assetRequest = new AssetRequest();
            Timer.instance.StartCoroutine(LoadAssetAsync(path, type, assetRequest));
            return assetRequest;
        }

        private System.Collections.IEnumerator LoadAssetAsync(string path, Type type, AssetRequest assetRequest)
        {
            string bundleName = TryGetAssetBundleName(path);
            if(bundleName == null)
            {
                assetRequest.isDone = true;
                yield break;
            }
            string bundlePath = GetBundlePath(bundleName);
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return assetBundleCreateRequest;
            AssetBundleRequest assetBundleRequest = assetBundleCreateRequest.assetBundle.LoadAssetAsync(path);
            yield return assetBundleRequest;
            assetRequest.asset = assetBundleRequest.asset;
            assetRequest.isDone = true;
        }

        public void UnloadAsset(Object asset)
        {
            
        }

        public void UnloadAllAssets()
        {
            // AssetDatabase.unload
        }

        public AssetBundle LoadBundle(string bundleName)
        {
            string bundlePath = GetBundlePath(bundleName);
            // todomark 加载依赖bundle
            return AssetBundle.LoadFromFile(bundlePath);
        }

        public string GetBundlePath(string bundleName)
        {
            return "Test/" + bundleName;
        }

        public string TryGetAssetBundleName(string assetPath)
        {
            string bundleName;
            m_AssetBundleMap.TryGetValue(assetPath, out bundleName);
            return bundleName;
        }
    }
}