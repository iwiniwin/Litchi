using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetDataManager : MonoSingleton<AssetDataManager>
    {
        private Dictionary<ulong, AssetData> m_AssetDataCache = new Dictionary<ulong, AssetData>();
        private Dictionary<int, ulong> m_InstanceID2PathHash = new Dictionary<int, ulong>();

        private LinkedList<AssetData> m_WaitList = new LinkedList<AssetData>();
        public static int maxLoadingCount = 8;
        private LinkedList<AssetData> m_LoadingList = new LinkedList<AssetData>();

        public Object Load(string path, Type type)
        {
            ulong hash = AssetDataManifest.GetPathHash(path);
            AssetData assetData = null;
            if(m_AssetDataCache.TryGetValue(hash, out assetData))
            {
                // Logger.assert(type = type);   // marktodo
                assetData.Retain();
                return assetData.asset;
            }
            assetData = CreateAssetData(hash, type, AssetLoadPriority.Normal);
            assetData.Retain();
            assetData.Load();
            Logger.Assert(assetData.isDone);
            if(assetData.asset != null)
            {
                CacheAssetData(assetData);
            }
            return assetData.asset;
        }

        public AssetLoadRequest LoadAsync(string path, Type type, AssetLoadPriority priority)
        {
            ulong hash = AssetDataManifest.GetPathHash(path);
            // marktodo request管理
            AssetData assetData = null;
            if(m_AssetDataCache.TryGetValue(hash, out assetData))
            {
                // Logger.assert(type = type);   // marktodo
                // marktodo 测试是否需要模拟延迟一帧
                assetData.Retain();
                return new AssetLoadRequest(assetData);
            }

            // task.onCompleted += asset => {
                
            //     // marktodo 处理progress
            //     if(asset != null)
            //     {
            //         // assetData = new AssetData(hash, type, null);
            //         assetData.Retain();
            //         AddAssetData(assetData);
            //     }
            //     loadRequest.OnLoadCompleted(asset);
            // };
            // AssetLoadTaskManager.instance.StartTask(task);
            assetData = CreateAssetData(hash, type, priority);

            CacheAssetData(assetData);

            // marktodo 什么时候调用Retain
            assetData.Retain();
            // marktodo cache assetdata
            // AddAssetData(hash, assetData);
            AddToWaitList(assetData);
            // marktodo AssetLoadRequest对象池
            return new AssetLoadRequest(assetData);
        }

        public void CacheAssetData(AssetData assetData)
        {
            m_AssetDataCache.Add(assetData.hash, assetData);
            // AssetData existedData = null;
            // if(m_AssetDataCache.TryGetValue(hash, out existedData))
            // {
            //     existedData.MergeRefCount(assetData);
            //     return;
            // }
            // m_AssetDataCache.Add(hash, assetData);

            // int instanceID = assetData.asset.GetInstanceID();
            // if(m_InstanceID2PathHash.ContainsKey(instanceID))
            // {
            //     m_InstanceID2PathHash[instanceID] = hash;
            // }
            // else
            // {
            //     m_InstanceID2PathHash.Add(instanceID, hash);
            // }
        }

        public void AddToWaitList(AssetData assetData)
        {
            if(m_WaitList.Count == 0)
            {
                m_WaitList.AddFirst(assetData);
                return;
            }
            var cur = m_WaitList.First;
            while(cur != null)
            {
                if(assetData.priority > cur.Value.priority)
                {
                    m_WaitList.AddBefore(cur, assetData);
                }
                cur = cur.Next;
            }
        }

        public void Update()
        {
            while(m_WaitList.Count > 0 && m_LoadingList.Count < maxLoadingCount)
            {
                var first = m_WaitList.First;
                m_WaitList.RemoveFirst();
                first.Value.LoadAsync();
                m_LoadingList.AddLast(first);
            }
            var cur = m_LoadingList.First;
            while(cur != null)
            {
                var data = cur.Value;
                data.Update();
                if(data.isDone)
                {
                    m_LoadingList.Remove(cur);
                }
                cur = cur.Next;
            }
        }

        public AssetData CreateAssetData(ulong hash, Type type, AssetLoadPriority priority)
        {
            var data = new ResourcesAssetData();
            data.Reset(hash, type, priority);
            return data;
        }

        /////////////////////////////分割线//////////////////////////////////

        private Dictionary<Type, IAssetLoader> m_LoaderCache = new Dictionary<Type, IAssetLoader>();

        public IAssetLoader GetLoader(string path)
        {

            IAssetLoader loader = null;

            // marktodo
            var type = AssetLoaderType.AssetBundle;
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
                    loader = new AssetBundleAssetLoader();
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
                Logger.Error("未注册自定义加载器");  // marktodo  
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