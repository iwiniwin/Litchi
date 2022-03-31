using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    internal class AssetSystem : MonoSingleton<AssetSystem>
    {
        // public Func<string, Type, Asset> AssetCreator { get; set; } = CreateAsset;

        private Dictionary<ulong, Asset> m_AssetCache = new Dictionary<ulong, Asset>();
        private Dictionary<int, ulong> m_InstanceID2PathHash = new Dictionary<int, ulong>();

        private LinkedList<Asset> m_WaitList = new LinkedList<Asset>();
        public static int maxLoadingCount = 8;
        private LinkedList<Asset> m_LoadingList = new LinkedList<Asset>();

        public Asset Load(string path, Type type, Func<string, Type, Asset> AssetCreator)
        {
            ulong hash = AssetManifest.GetPathHash(path);
            Asset Asset = null;
            if(m_AssetCache.TryGetValue(hash, out Asset))
            {
                // Logger.assert(type = type);   // marktodo
                Asset.Retain();
                return Asset;
            }
            Asset = AssetCreator(path, type);
            Asset.Reset(hash, type, AssetLoadPriority.Normal);
            Asset.Retain();
            Asset.Load();
            Logger.Assert(Asset.isDone, "Load后没有设置isDone");
            if(Asset.asset != null)
            {
                CacheAsset(Asset);
            }
            return Asset;
        }

        public Asset LoadAsync(string path, Type type, AssetLoadPriority priority, Func<string, Type, Asset> AssetCreator)
        {
            ulong hash = AssetManifest.GetPathHash(path);
            // marktodo request管理
            Asset Asset = null;
            if(m_AssetCache.TryGetValue(hash, out Asset))
            {
                // Logger.assert(type = type);   // marktodo
                // marktodo 测试是否需要模拟延迟一帧
                Asset.Retain();
                return Asset;
            }

            // task.onCompleted += asset => {
                
            //     // marktodo 处理progress
            //     if(asset != null)
            //     {
            //         // Asset = new Asset(hash, type, null);
            //         Asset.Retain();
            //         AddAsset(Asset);
            //     }
            //     loadRequest.OnLoadCompleted(asset);
            // };
            // AssetLoadTaskManager.instance.StartTask(task);
            Asset = AssetCreator(path, type);
            Asset.Reset(hash, type, priority);

            CacheAsset(Asset);

            // marktodo 什么时候调用Retain
            Asset.Retain();
            // marktodo cache Asset
            // AddAsset(hash, Asset);
            AddToWaitList(Asset);
            // marktodo AssetLoadRequest对象池
            return Asset;
        }

        public void CacheAsset(Asset Asset)
        {
            m_AssetCache.Add(Asset.hash, Asset);
            // Asset existedData = null;
            // if(m_AssetCache.TryGetValue(hash, out existedData))
            // {
            //     existedData.MergeRefCount(Asset);
            //     return;
            // }
            // m_AssetCache.Add(hash, Asset);

            // int instanceID = Asset.asset.GetInstanceID();
            // if(m_InstanceID2PathHash.ContainsKey(instanceID))
            // {
            //     m_InstanceID2PathHash[instanceID] = hash;
            // }
            // else
            // {
            //     m_InstanceID2PathHash.Add(instanceID, hash);
            // }
        }

        public void AddToWaitList(Asset Asset)
        {
            if(m_WaitList.Count == 0)
            {
                m_WaitList.AddFirst(Asset);
                return;
            }
            var cur = m_WaitList.First;
            while(cur != null)
            {
                if(Asset.priority > cur.Value.priority)
                {
                    m_WaitList.AddBefore(cur, Asset);
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

            Asset Asset;
            if(m_AssetCache.TryGetValue(hash, out Asset))
            {
                Asset.Release();
            }
            else
            {
                // 未知资源
            }
        }

        public void UnloadUnusedAssets()
        {
            List<ulong> list = new List<ulong>();
            foreach (var itor in m_AssetCache)
            {
                Asset resourceData = itor.Value;
                if (resourceData == null || resourceData.IsZeroRef())
                {
                    list.Add(itor.Key);
                }
            }

            for (int i = 0; i < list.Count; ++i)
            {
                m_AssetCache.Remove(list[i]);
            }
        }

        // public static Asset CreateAsset(string path, Type type)
        // {
        //     // return new ResourcesAsset();
        //     return new BundleAsset();
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
                case AssetLoaderType.Assetbase:
                    // loader = GetBuiltinLoader<ResourcesLoader>();
                    break;
#endif
                case AssetLoaderType.Resources:
                    loader = GetBuiltinLoader<ResourcesAssetLoader>();
                    break;
                case AssetLoaderType.AssetBundle:
                    loader = new BundleAssetLoader();
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