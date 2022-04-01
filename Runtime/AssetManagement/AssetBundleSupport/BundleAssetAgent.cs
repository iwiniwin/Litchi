using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class BundleAssetAgent : AssetAgent 
    {
        private AssetBundleRequest m_Request;
        private BundleAgent m_BundleAgent;
        private string m_AssetName;
        private string m_BundleId;

        public override void Load()
        {
            var bundleAgent = AssetAgentManager.instance.Load<BundleAgent>(m_BundleId, typeof(AssetBundle));
            if(bundleAgent.assetBundle == null)
            {
                OnLoadCompleted(null);
            }
            else
            {
                OnLoadCompleted(bundleAgent.assetBundle.LoadAsset(m_AssetName));
            }
        }

        public override void LoadAsync()
        {
            m_BundleAgent = AssetAgentManager.instance.LoadAsync<BundleAgent>(m_BundleId, typeof(AssetBundle), AssetLoadPriority.Normal);
        }

        public override void Unload()
        {
            
        }

        public override void Update()
        {
            if(m_BundleAgent == null) return;
            progress = CalcProgress();
            if(m_BundleAgent.isDone)
            {
                if(m_BundleAgent.assetBundle == null)
                {
                    // marktodo unload bundleagent ?
                    OnLoadCompleted(null);
                    return;
                }
                if(m_Request == null)
                {
                    m_Request = m_BundleAgent.assetBundle.LoadAssetAsync(m_AssetName, type);
                }
                if(m_Request.isDone)
                {
                    OnLoadCompleted(m_Request.asset);
                }
            }
        }

        public virtual float CalcProgress()
        {
            float progress = m_Request == null ? 0 : m_Request.progress;
            return (progress + m_BundleAgent.progress) / 2;
        }

        public override void Init(string path, Type type, AssetLoadPriority priority)
        {
            base.Init(path, type, priority);
            m_AssetName = "";
            m_BundleId = "";
            m_Request = null;
            m_BundleAgent = null;
        }
    }
}