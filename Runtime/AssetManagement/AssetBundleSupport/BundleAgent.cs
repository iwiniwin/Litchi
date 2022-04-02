using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class BundleAgent : TimeAssetAgent 
    {
        private AssetBundleCreateRequest m_Request;
        public AssetBundle assetBundle { get; private set; }
        private string[] m_DirectDependencies;
        private List<BundleAgent> m_DependBundleAgents = new List<BundleAgent>();

        protected override void OnLoad()
        {
            if(m_DirectDependencies.Length > 0)
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
            if(m_DirectDependencies.Length > 0)
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
            m_DirectDependencies = BundleManifestUtility.GetDirectDependencies(path);
            m_DependBundleAgents.Clear();
        }

        public static readonly long kBundleTimeOut = 10000;  // ms

        public override bool delayUnload => elapsedMilliseconds < kBundleTimeOut;
    }
}