using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleAssetData : AssetData 
    {
        private ResourceRequest m_Request;

        public override void Load()
        {
            
        }

        public override void LoadAsync()
        {
            string path = AssetDataManifest.GetHashPath(hash);
            m_Request = Resources.LoadAsync(path, type);
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

        public override void Reset(ulong hash, Type type, AssetLoadPriority priority)
        {
            base.Reset(hash, type, priority);
            m_Request = null;
        }
    }
}