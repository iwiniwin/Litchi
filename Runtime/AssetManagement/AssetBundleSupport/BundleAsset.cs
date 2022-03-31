using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class BundleAsset : Asset 
    {
        private ResourceRequest m_Request;

        public override void Load()
        {
            AssetBundleDependencies dependencies = new AssetBundleDependencies();
            dependencies.Load();
            Logger.Assert(dependencies.isDone, "Load后没有设置isDone");
            if(dependencies.mainBundle == null)
            {
                OnLoadCompleted(null);
            }
            else
            {
                OnLoadCompleted(dependencies.mainBundle.LoadAsset("todoname"));
            }
        }

        public override void LoadAsync()
        {
            string path = AssetManifest.GetHashPath(hash);
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