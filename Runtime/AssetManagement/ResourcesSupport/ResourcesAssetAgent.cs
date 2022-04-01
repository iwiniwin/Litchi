using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class ResourcesAssetAgent : AssetAgent 
    {
        private ResourceRequest m_Request;

        protected override void OnLoad()
        {
            var asset = Resources.Load(path, type);
            OnLoadCompleted(asset);
        }

        protected override void OnLoadAsync()
        {
            m_Request = Resources.LoadAsync(path, type);
        }

        protected override void OnUnload()
        {
            if(asset != null)
            {
                Resources.UnloadAsset(asset);
            }
        }

        protected override void OnUpdate()
        {
            if(m_Request == null) return;
            progress = m_Request.progress;
            if(m_Request.isDone)
            {
                OnLoadCompleted(m_Request.asset);
            }
        }

        protected override void OnInit()
        {
            m_Request = null;
        }
    }
}