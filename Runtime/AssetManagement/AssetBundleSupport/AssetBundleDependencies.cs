using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleDependencies : AssetData 
    {
        public AssetBundle mainBundle { get; protected set; }

        public override void Load()
        {
            string[] dependencies = null;
            AssetBundleData assetBundleData = new AssetBundleData();
            assetBundleData.Load();
            mainBundle = assetBundleData.assetBundle;

            if(dependencies != null && dependencies.Length > 0)
            {
                foreach(var depend in dependencies)
                {
                    AssetBundleData data = new AssetBundleData();
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