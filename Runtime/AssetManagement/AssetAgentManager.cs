using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    internal class AssetAgentManager : MonoSingleton<AssetAgentManager>
    {
        private Dictionary<string, AssetAgent> m_AssetAgentCache = new Dictionary<string, AssetAgent>();
        private Dictionary<int, string> m_InstanceID2PathHash = new Dictionary<int, string>();

        private LinkedList<AssetAgent> m_WaitList = new LinkedList<AssetAgent>();
        public static int maxLoadingCount = 8;
        private LinkedList<AssetAgent> m_LoadingList = new LinkedList<AssetAgent>();

        public AssetAgent Load<T>(string path, Type type) where T : AssetAgent, new()
        {
            return Load(path, type, (p, t) => new T());
        } 

        public AssetAgent LoadAsync<T>(string path, Type type, AssetLoadPriority priority) where T : AssetAgent, new()
        {
            return LoadAsync(path, type, priority, (p, t) => new T());
        }

        public AssetAgent Load(string path, Type type, Func<string, Type, AssetAgent> agentCreator)
        {
            AssetAgent agent = null;
            if(m_AssetAgentCache.TryGetValue(path, out agent))
            {
                // Logger.assert(type = type);   // marktodo
                agent.Retain();
                return agent;
            }
            agent = agentCreator(path, type);
            agent.Reset(path, type, AssetLoadPriority.Normal);
            agent.Retain();
            agent.Load();
            Logger.Assert(agent.isDone, "Load后没有设置isDone");
            if(agent.asset != null)
            {
                CacheAsset(agent);
            }
            return agent;
        }

        public AssetAgent LoadAsync(string path, Type type, AssetLoadPriority priority, Func<string, Type, AssetAgent> agentCreator)
        {
            // marktodo request管理
            AssetAgent agent = null;
            if(m_AssetAgentCache.TryGetValue(path, out agent))
            {
                // Logger.assert(type = type);   // marktodo
                // marktodo 测试是否需要模拟延迟一帧
                agent.Retain();
                return agent;
            }

            // task.onCompleted += asset => {
                
            //     // marktodo 处理progress
            //     if(asset != null)
            //     {
            //         // AssetAgent = new AssetAgent(path, type, null);
            //         agent.Retain();
            //         AddAsset(agent);
            //     }
            //     loadRequest.OnLoadCompleted(asset);
            // };
            // AssetLoadTaskManager.instance.StartTask(task);
            agent = agentCreator(path, type);
            agent.Reset(path, type, priority);

            CacheAsset(agent);

            // marktodo 什么时候调用Retain
            agent.Retain();
            // marktodo cache AssetAgent
            // AddAsset(path, AssetAgent);
            AddToWaitList(agent);
            // marktodo AssetLoadRequest对象池
            return agent;
        }

        public void CacheAsset(AssetAgent agent)
        {
            m_AssetAgentCache.Add(agent.path, agent);
            // AssetAgent existedData = null;
            // if(m_AssetAgentCache.TryGetValue(path, out existedData))
            // {
            //     existedData.MergeRefCount(agent);
            //     return;
            // }
            // m_AssetAgentCache.Add(path, agent);

            // int instanceID = agent.asset.GetInstanceID();
            // if(m_InstanceID2PathHash.ContainsKey(instanceID))
            // {
            //     m_InstanceID2PathHash[instanceID] = path;
            // }
            // else
            // {
            //     m_InstanceID2PathHash.Add(instanceID, path);
            // }
        }

        public void AddToWaitList(AssetAgent agent)
        {
            if(m_WaitList.Count == 0)
            {
                m_WaitList.AddFirst(agent);
                return;
            }
            var cur = m_WaitList.First;
            while(cur != null)
            {
                if(agent.priority > cur.Value.priority)
                {
                    m_WaitList.AddBefore(cur, agent);
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
            string path;
            if(!m_InstanceID2PathHash.TryGetValue(asset.GetInstanceID(), out path))
            {
                // 未知资源
                return;
            }

            AssetAgent agent;
            if(m_AssetAgentCache.TryGetValue(path, out agent))
            {
                agent.Release();
            }
            else
            {
                // 未知资源
            }

            m_AssetAgentCache.Remove(agent.path);
        }

        public void UnloadUnusedAssets()
        {
            // List<ulong> list = new List<ulong>();
            // foreach (var itor in m_AssetAgentCache)
            // {
            //     AssetAgent resourceData = itor.Value;
            //     if (resourceData == null || resourceData.IsZeroRef())
            //     {
            //         list.Add(itor.Key);
            //     }
            // }

            // for (int i = 0; i < list.Count; ++i)
            // {
            //     m_AssetAgentCache.Remove(list[i]);
            // }
        }

        // public static AssetAgent CreateAsset(string path, Type type)
        // {
        //     // return new ResourcesAssetAgent();
        //     return new BundleAssetAgent();
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