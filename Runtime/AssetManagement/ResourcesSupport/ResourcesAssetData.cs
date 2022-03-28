using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class ResourcesAssetData : AssetData 
    {
        private ResourceRequest m_Request;

        public override void Load(ulong hash, Type type)
        {
            var asset = Resources.Load(AssetDataManifest.GetHashPath(hash), type);
            OnLoadCompleted(asset);
        }

        public override void LoadAsync(ulong hash, Type type)
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

        public override void Reset()
        {
            base.Reset();
            m_Request = null;
        }
    }
}