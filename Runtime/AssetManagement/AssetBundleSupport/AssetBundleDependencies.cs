using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleDependencies : AssetAgent 
    {
        public AssetBundle mainBundle { get; protected set; }

        public override void Load()
        {
            string[] dependencies = null;
            BundleAgent BundleAgent = new BundleAgent();
            BundleAgent.Load();
            mainBundle = BundleAgent.assetBundle;

            if(dependencies != null && dependencies.Length > 0)
            {
                foreach(var depend in dependencies)
                {
                    BundleAgent data = new BundleAgent();
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

        public override void Reset(string path, Type type, AssetLoadPriority priority)
        {
            base.Reset(path, type, priority);
        }
    }
}