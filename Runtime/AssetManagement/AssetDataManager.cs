using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public interface IAssetLoader 
    {
        Object Load(ulong hash, Type type);
        IEnumerator LoadAsync(AssetLoadHandle loadHandle);
    }

    public interface ICustomAssetLoader : IAssetLoader
    {
        bool Match(string path);
    }

    public enum AssetLoaderType
    {
        AssetBundle,
        Resources,
        AssetDatabase,
        Custom,
    }

    public class AssetDataManager : MonoSingleton<AssetDataManager>
    {
        private Dictionary<ulong, AssetData> m_PathHash2Data = new Dictionary<ulong, AssetData>();
        private Dictionary<int, ulong> m_InstanceID2PathHash = new Dictionary<int, ulong>();

        public Object Load(string path, Type type)
        {
            ulong hash = AssetDataManifest.GetPathHash(path);
            AssetData assetData = GetAssetData(hash);
            if(assetData != null)
            {
                // Logger.assert(type = type);   // marktodo
                assetData.Retain();
                return assetData.asset;
            }
            var loader = GetLoader(path);
            var asset = loader.Load(hash, type);
            if(asset != null)
            {
                assetData = new AssetData(hash, type, asset);
                assetData.Retain();
                AddAssetData(assetData);
            }
            return asset;
        }

        public AssetLoadHandle LoadAsync(string path, Type type, AssetLoadPriority priority)
        {
            AssetLoadHandle loadHandle = new AssetLoadHandle(priority);
            StartCoroutine(TryLoadAsync(loadHandle, path, type, priority));
            return loadHandle;
        }

        public IEnumerator TryLoadAsync(AssetLoadHandle loadHandle, string path, Type type, AssetLoadPriority priority)
        {
            ulong hash = AssetDataManifest.GetPathHash(path);
            AssetData assetData = GetAssetData(hash);
            if(assetData != null)
            {
                // Logger.assert(type = type);   // marktodo
                // marktodo 测试是否需要模拟延迟一帧
                assetData.Retain();
                loadHandle.assetData = assetData;
                loadHandle.isDone = true;
                yield break;
            }
            var loader = GetLoader(path);
            assetData = new AssetData(hash, type, null);
            loadHandle.assetData = assetData;
            yield return loader.LoadAsync(loadHandle);
            // marktodo 处理progress
            if(assetData.asset != null)
            {
                assetData.Retain();
                AddAssetData(assetData);
                loadHandle.isDone = true;
            }
        }

        private AssetData GetAssetData(ulong hash)
        {
            AssetData assetData = null;
            if(m_PathHash2Data.TryGetValue(hash, out assetData))
            {
                return assetData;
            }
            return null;
        }

        private void AddAssetData(AssetData assetData)
        {
            AssetData existedData = null;
            if(m_PathHash2Data.TryGetValue(assetData.hash, out existedData))
            {
                existedData.MergeRefCount(assetData);
                return;
            }
            m_PathHash2Data.Add(assetData.hash, assetData);

            int instanceID = assetData.asset.GetInstanceID();
            if(m_InstanceID2PathHash.ContainsKey(instanceID))
            {
                m_InstanceID2PathHash[instanceID] = assetData.hash;
            }
            else
            {
                m_InstanceID2PathHash.Add(instanceID, assetData.hash);
            }
        }

        private Dictionary<Type, IAssetLoader> m_LoaderCache = new Dictionary<Type, IAssetLoader>();

        public IAssetLoader GetLoader(string path)
        {

            IAssetLoader loader = null;

            var type = AssetLoaderType.Resources;
            switch(type)
            {
#if UNITY_EDITOR
                case AssetLoaderType.AssetDatabase:
                    // loader = GetBuiltinLoader<ResourcesLoader>();
                    break;
#endif
                case AssetLoaderType.Resources:
                    loader = GetBuiltinLoader<ResourcesAssetLoader>();
                    break;
                case AssetLoaderType.AssetBundle:
                    // loader = new AssetBundleLoader();
                    break;
                case AssetLoaderType.Custom:
                    loader = GetCustomLoader(path);
                    break;
            }
            return loader;
        }

        private List<ICustomAssetLoader> m_CustomLoaders;
        public void RegisterCustomLoader(ICustomAssetLoader loader)
        {
            if(m_CustomLoaders == null)
            {
                m_CustomLoaders = new List<ICustomAssetLoader>();
            }
            m_CustomLoaders.Add(loader);
        }

        private IAssetLoader GetBuiltinLoader<T>() where T : IAssetLoader, new()
        {
            IAssetLoader loader = null;
            if(!m_LoaderCache.TryGetValue(typeof(T), out loader))
            {
                loader = new T();
                m_LoaderCache.Add(typeof(T), loader);
            }
            return loader;
        }

        private ICustomAssetLoader GetCustomLoader(string path)
        {
            if(m_CustomLoaders == null || m_CustomLoaders.Count == 0)
            {
                Logger.LogError("未注册自定义加载器");  // marktodo  
                return null;
            }
            foreach (var loader in m_CustomLoaders)
            {
                if(loader.Match(path))
                {
                    return loader; 
                }
            }
            return null;
        }
    }
}