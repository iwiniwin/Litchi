using Litchi.IO;
using Litchi.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
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
            IAssetRef assetRef;
            if(!m_LoadedAssets.TryGetValue(path, out assetRef))
            {
                Object assetObject = m_Loader.LoadAsset(path, type);
                if(assetObject == null)
                {
                    return null;
                }
                assetRef = new AssetRef(path, assetObject);
            }
            assetRef.Retain();
            return assetRef.assetObject;
        }
    }
}