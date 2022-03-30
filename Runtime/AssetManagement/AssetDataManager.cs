using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    internal class AssetDataManager : MonoSingleton<AssetDataManager>
    {
        // public Func<string, Type, AssetData> assetDataCreator { get; set; } = CreateAssetData;

        private Dictionary<ulong, AssetData> m_AssetDataCache = new Dictionary<ulong, AssetData>();
        private Dictionary<int, ulong> m_InstanceID2PathHash = new Dictionary<int, ulong>();

        private LinkedList<AssetData> m_WaitList = new LinkedList<AssetData>();
        public static int maxLoadingCount = 8;
        private LinkedList<AssetData> m_LoadingList = new LinkedList<AssetData>();

        public AssetData Load(string path, Type type, Func<string, Type, AssetData> assetDataCreator)
        {
            ulong hash = AssetDataManifest.GetPathHash(path);
            AssetData assetData = null;
            if(m_AssetDataCache.TryGetValue(hash, out assetData))
            {
                // Logger.assert(type = type);   // marktodo
                assetData.Retain();
                return assetData;
            }
            assetData = assetDataCreator(path, type);
            assetData.Reset(hash, type, AssetLoadPriority.Normal);
            assetData.Retain();
            assetData.Load();
            Logger.Assert(assetData.isDone, "Load后没有设置isDone");
            if(assetData.asset != null)
            {
                CacheAssetData(assetData);
            }
            return assetData;
        }

        public AssetData LoadAsync(string path, Type type, AssetLoadPriority priority, Func<string, Type, AssetData> assetDataCreator)
        {
            ulong hash = AssetDataManifest.GetPathHash(path);
            // marktodo request管理
            AssetData assetData = null;
            if(m_AssetDataCache.TryGetValue(hash, out assetData))
            {
                // Logger.assert(type = type);   // marktodo
                // marktodo 测试是否需要模拟延迟一帧
                assetData.Retain();
                return assetData;
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
            assetData = assetDataCreator(path, type);
            assetData.Reset(hash, type, priority);

            CacheAssetData(assetData);

            // marktodo 什么时候调用Retain
            assetData.Retain();
            // marktodo cache assetdata
            // AddAssetData(hash, assetData);
            AddToWaitList(assetData);
            // marktodo AssetLoadRequest对象池
            return assetData;
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

        public void Unload(Object asset)
        {
            if(asset == null) return;
            ulong hash = 0;
            if(!m_InstanceID2PathHash.TryGetValue(asset.GetInstanceID(), out hash))
            {
                // 未知资源
                return;
            }

            AssetData assetData;
            if(m_AssetDataCache.TryGetValue(hash, out assetData))
            {
                assetData.Release();
            }
            else
            {
                // 未知资源
            }
        }

        public void UnloadUnusedAssets()
        {
            List<ulong> list = new List<ulong>();
            foreach (var itor in m_AssetDataCache)
            {
                AssetData resourceData = itor.Value;
                if (resourceData == null || resourceData.IsZeroRef())
                {
                    list.Add(itor.Key);
                }
            }

            for (int i = 0; i < list.Count; ++i)
            {
                m_AssetDataCache.Remove(list[i]);
            }
        }

        // public static AssetData CreateAssetData(string path, Type type)
        // {
        //     // return new ResourcesAssetData();
        //     return new AssetBundleAssetData();
        // }

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