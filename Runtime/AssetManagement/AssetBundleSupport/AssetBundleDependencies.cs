using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleDependencies : Asset 
    {
        public AssetBundle mainBundle { get; protected set; }

        public override void Load()
        {
            string[] dependencies = null;
            Bundle Bundle = new Bundle();
            Bundle.Load();
            mainBundle = Bundle.assetBundle;

            if(dependencies != null && dependencies.Length > 0)
            {
                foreach(var depend in dependencies)
                {
                    Bundle data = new Bundle();
                    data.Load();
                    // 缓存data?
                }
            }
        }

        public override void LoadAsync()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Reset(ulong hash, Type type, AssetLoadPriority priority)
        {
            base.Reset(hash, type, priority);
        }
    }
}