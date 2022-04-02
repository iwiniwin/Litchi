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

        private LinkedList<AssetAgent> m_WaitLoadList = new LinkedList<AssetAgent>();
        private static int maxDelayUnloadCount = 10;
        private LinkedList<AssetAgent> m_WaitUnloadList = new LinkedList<AssetAgent>();
        private LinkedList<AssetAgent> m_DelayUnloadList = new LinkedList<AssetAgent>();
        public static int maxLoadingCount = 8;
        private LinkedList<AssetAgent> m_LoadingList = new LinkedList<AssetAgent>();

        public T Load<T>(string path, Type type) where T : AssetAgent, new()
        {
            return Load(path, type, (p, t) => new T()) as T;
        } 

        public T LoadAsync<T>(string path, Type type, AssetLoadPriority priority) where T : AssetAgent, new()
        {
            return LoadAsync(path, type, priority, (p, t) => new T()) as T;
        }

        public AssetAgent Load(string path, Type type, Func<string, Type, AssetAgent> agentCreator)
        {
            AssetAgent agent = GetOrCreateAgent(path, type, AssetLoadPriority.Normal, agentCreator);
            agent.Retain();
            CacheAgent(agent);
            agent.Load();
            return agent;
        }

        public AssetAgent LoadAsync(string path, Type type, AssetLoadPriority priority, Func<string, Type, AssetAgent> agentCreator)
        {
            AssetAgent agent = GetOrCreateAgent(path, type, priority, agentCreator);
            agent.Retain();
            CacheAgent(agent);
            AddToWaitLoad(agent);
            return agent;
        }

        public void Unload(string path, Type type)
        {
            AssetAgent agent = GetAgent(path, type);
            if(agent == null)
            {
                // 未知资源
                return;
            }
            agent.Release();
            if(!agent.unloadable)
            {
                m_WaitUnloadList.AddLast(agent);
            }
        }

        public void Unload(AssetAgent agent)
        {
            Unload(agent.path, agent.type);
        }

        public void UnloadUnusedAssets()
        {

        }

        public void CacheAgent(AssetAgent agent)
        {
            AssetAgent existedAgent = null;
            if(m_AssetAgentCache.TryGetValue(agent.id, out existedAgent))
            {
                existedAgent.MergeRefCount(agent);
                return;
            }
            m_AssetAgentCache.Add(agent.id, agent);
        }

        public AssetAgent GetOrCreateAgent(string path, Type type, AssetLoadPriority priority, Func<string, Type, AssetAgent> agentCreator)
        {
            AssetAgent agent = GetAgent(path, type);
            if(agent != null)
            {
                return agent;
            }
            agent = agentCreator(path, type);
            agent.Init(path, type, priority);
            return agent;
        }

        public AssetAgent GetAgent(string path, Type type)
        {
            AssetAgent agent = null;
            m_AssetAgentCache.TryGetValue(AssetAgent.GenerateId(path, type), out agent);
            return agent;
        }

        public void AddToWaitLoad(AssetAgent agent)
        {
            if(m_WaitLoadList.Count == 0)
            {
                m_WaitLoadList.AddFirst(agent);
                return;
            }
            var cur = m_WaitLoadList.First;
            while(cur != null)
            {
                if(agent.priority > cur.Value.priority)
                {
                    m_WaitLoadList.AddBefore(cur, agent);
                }
                cur = cur.Next;
            }
        }

        public void Update()
        {
            while(m_WaitLoadList.Count > 0 && m_LoadingList.Count < maxLoadingCount)
            {
                var first = m_WaitLoadList.First;
                m_WaitLoadList.RemoveFirst();
                first.Value.LoadAsync();
                m_LoadingList.AddLast(first);
            }

            var curLoad = m_LoadingList.First;
            while(curLoad != null)
            {
                var agent = curLoad.Value;
                var cur = curLoad;
                curLoad = curLoad.Next;
                agent.Update();
                if(agent.isDone)
                {
                    m_LoadingList.Remove(cur);
                }
            }

            m_DelayUnloadList.Clear();
            var curUnload = m_WaitUnloadList.First;
            while(curUnload != null)
            {
                var agent = curUnload.Value;
                var cur = curUnload;
                curUnload = curUnload.Next;
                // if(busy) break;
                if(!agent.isDone) continue;
                if(!agent.unloadable)
                {
                    m_WaitUnloadList.Remove(cur);
                    continue;
                }
                if(agent.delayUnload)
                {
                    m_DelayUnloadList.AddLast(cur);
                    if(m_DelayUnloadList.Count > maxDelayUnloadCount)
                    {
                        var firstDelay = m_DelayUnloadList.First;
                        m_DelayUnloadList.RemoveFirst();
                        DoUnload(firstDelay);
                    }
                    continue;
                }
                DoUnload(cur);
            }
        }

        private void DoUnload(LinkedListNode<AssetAgent> node)
        {
            m_WaitUnloadList.Remove(node);
            node.Value.Unload();
            m_AssetAgentCache.Remove(node.Value.id);
        }
    }
}