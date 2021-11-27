using Litchi.IO;
using Litchi.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public static class AssetManager
    {
        private static IAssetLoader m_Loader;

        private static Dictionary<string, IAssetRef> m_LoadedAssets;

        public static void Init()
        {
            switch(Settings.assetManage.assetLoaderType)
            {
#if UNITY_EDITOR
                case AssetLoaderType.AssetDatabase:
                    m_Loader = new AssetDatabaseLoader();
                    break;
#endif
                case AssetLoaderType.Resources:
                    m_Loader = new ResourcesLoader();
                    break;
                default:
                    m_Loader = new AssetBundleLoader();
                    break;
            }
            m_LoadedAssets = new Dictionary<string, IAssetRef>();
        }

        public static T LoadAsset<T>(string path) where T : Object
        {
            return LoadAsset(path, typeof(T)) as T;
        }

        public static Object LoadAsset(string path, Type type)
        {
            Object asset = GetAsset(path);
            if(asset != null)
            {
                return asset;
            }
            asset = m_Loader.LoadAsset(path, type);
            if(asset == null)
            {
                return null;
            }
            CacheAsset(path, asset);
            return asset;
        }

        public static AssetRequest LoadAssetAsync(string path, Type type)
        {
            if(m_LoadedAssets.ContainsKey(path))
            {
                return AssetRequest.SimulateRequest((req) => {
                    req.asset = GetAsset(path);
                });
            }
            AssetRequest request = m_Loader.LoadAssetAsync(path, type);
            request.completed += (ar) => {
                if(ar.asset != null)
                {
                    CacheAsset(path, ar.asset);
                }
            };
            return request;
        }

        public static AssetRequest LoadAssetAsync<T>(string path)
        {
            return LoadAssetAsync(path, typeof(T));
        }

        public static void LoadAssetAsync(string path, Type type, Action<Object> ac)
        {
            Timer.instance.StartCoroutine(InternalLoadAssetAsync(path, type, ac));
        }

        public static void UnloadAsset(string path)
        {
            IAssetRef assetRef = GetAssetRef(path);
            if(assetRef != null)
            {
                assetRef.Release();
                if(assetRef.refCount == 0)
                {
                    m_LoadedAssets.Remove(path);
                    m_Loader.UnloadAsset(assetRef.asset);
                }
            }
        }

        public static void UnloadAllAssets()
        {
            foreach(var pair in m_LoadedAssets)
            {
                if(pair.Value.refCount > 0)
                {

                }
                else
                {
                    m_Loader.UnloadAsset(pair.Value.asset);
                }
            }
            m_Loader.UnloadAllAssets();
            m_LoadedAssets.Clear();
        }

        private static IEnumerator InternalLoadAssetAsync(string path, Type type, Action<Object> ac)
        {
            var request = LoadAssetAsync(path, type);
            yield return request;
            ac(request.asset);
        }

        public static Object GetAsset(string path)
        {
            IAssetRef assetRef = GetAssetRef(path);
            if(assetRef != null)
            {
                assetRef.Retain();
                return assetRef.asset;
            }
            return null;
        }

        private static IAssetRef GetAssetRef(string path)
        {
            IAssetRef assetRef;
            if(m_LoadedAssets.TryGetValue(path, out assetRef))
            {
                return assetRef;
            }
            return null;
        }

        private static void CacheAsset(string path, Object asset)
        {
            if(!m_LoadedAssets.ContainsKey(path))
            {
                m_LoadedAssets.Add(path, new AssetRef(path, asset));
            }
            m_LoadedAssets[path].Retain();
        }
    }
}