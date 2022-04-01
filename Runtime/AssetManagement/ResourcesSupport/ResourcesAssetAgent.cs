using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class ResourcesAssetAgent : AssetAgent 
    {
        private ResourceRequest m_Request;

        public override void Load()
        {
            var asset = Resources.Load(path, type);
            OnLoadCompleted(asset);
        }

        public override void LoadAsync()
        {
            m_Request = Resources.LoadAsync(path, type);
        }

        public override void Unload()
        {
            
        }

        public override void Update()
        {
            if(m_Request == null) return;
            progress = m_Request.progress;
            if(m_Request.isDone)
            {
                OnLoadCompleted(m_Request.asset);
            }
        }

        public override void Init(string path, Type type, AssetLoadPriority priority)
        {
            base.Init(path, type, priority);
            m_Request = null;
        }
    }
}