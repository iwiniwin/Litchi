using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class BundleAgent : AssetAgent 
    {
        private AssetBundleCreateRequest m_Request;
        private List<string> m_DirectDependencies = new List<string>();
        private List<BundleAgent> m_DependBundleAgents = new List<BundleAgent>();

        protected override void OnLoad()
        {
            if(m_DirectDependencies.Count > 0)
            {
                foreach(var dependBundleId in m_DirectDependencies)
                {
                    AssetAgentManager.instance.Load<BundleAgent>(dependBundleId, typeof(AssetBundle));
                }
            }
            assetBundle = AssetBundle.LoadFromFile(path);
            OnLoadCompleted(assetBundle);
        }

        protected override void OnLoadAsync()
        {
            if(m_DirectDependencies.Count > 0)
            {
                foreach(var dependBundleId in m_DirectDependencies)
                {
                    var dependAgent = AssetAgentManager.instance.Load<BundleAgent>(dependBundleId, typeof(AssetBundle));
                    m_DependBundleAgents.Add(dependAgent);
                }
            }
            m_Request = AssetBundle.LoadFromFileAsync(path);
        }

        protected override void OnUnload()
        {
            if(m_DependBundleAgents.Count > 0)
            {
                foreach(var dependAgent in m_DependBundleAgents)
                {
                    AssetAgentManager.instance.Unload(dependAgent);
                }
                m_DependBundleAgents.Clear();
            }

            if(assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
        }

        protected override void OnUpdate()
        {
            if(m_Request == null) return;
            progress = CalcProgress();
            if(IsDependenciesLoadDone() && m_Request.isDone)
            {
                assetBundle = m_Request.assetBundle;
                OnLoadCompleted(assetBundle);
            }
        }

        public virtual float CalcProgress()
        {
            float curProgress = m_Request.progress;
            foreach(var dependAgent in m_DependBundleAgents)
            {
                curProgress += dependAgent.progress;
            }
            return curProgress / (m_DependBundleAgents.Count + 1);
        }

        public bool IsDependenciesLoadDone()
        {
            foreach(var dependAgent in m_DependBundleAgents)
            {
                if(!dependAgent.isDone)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnInit()
        {
            m_DirectDependencies = null;  // marktodo getDirectDependencies
            m_DependBundleAgents.Clear();
        }

        ///////////////////////////////////////分隔符///////////////////////////////////////

        public static readonly long kBundleTimeOut = 10000;  // ms

        public AssetBundle assetBundle { get; private set; }

        public string bundleID { get; private set; }

        private long m_LastUsedTime = Stopwatch.GetTimestamp();

        public long elapsedMilliseconds
        {
            get
            {
                return (Stopwatch.GetTimestamp() - m_LastUsedTime) * 1000 / Stopwatch.Frequency;
            } 
        }

        public BundleAgent()
        {
            // this.bundleID = bundleID;
            // this.assetBundle = assetBundle;
        }

        // public override void Retain()
        // {
        //     base.Retain();
        //     m_LastUsedTime = Stopwatch.GetTimestamp();
        // }

        // public override void Release()
        // {
        //     base.Release();
        //     m_LastUsedTime = Stopwatch.GetTimestamp();
        // }

        public bool TimeOut()
        {
            return elapsedMilliseconds > kBundleTimeOut;
        }

        public void Unload(bool unloadAllLoadedObjects)
        {
            if(assetBundle != null)
            {
                assetBundle.Unload(unloadAllLoadedObjects);
                assetBundle = null;
            }
        }

        // 是否可卸载
        public bool Unloadable()
        {
            return IsZeroRef();
        }
    }
}