using UnityEngine;
using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage2
{
    public class AssetBundleSceneAsset : AssetBundleAsset
    {
        private string m_PassInAssetBundleName;

        // marktodo
        public static AssetBundleSceneAsset Allocate(string name)
        {
            var asset = ObjectPool<AssetBundleSceneAsset>.instance.Allocate();
            if(asset != null)
            {
                asset.assetName = name;
                asset.InitAssetBundleName();
            }
            return asset;
        }

        public override bool LoadSync()
        {
            if(!CanLoad())
            {
                return false;
            }
            
            if(string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

            var key = AssetSearchKey.Allocate(assetBundleName);
            var bundleLoader = AssetManager.instance.GetAsset<AssetBundleLoader>(key);
            key.Recycle();

            if(bundleLoader == null || bundleLoader.assetBundle == null)
            {
                OnLoadFailed();
                return false;
            }
            state = AssetState.Ready;
            return true;
        }

        public override void LoadAsync()
        {
            LoadSync();
        }

        // marktodo 这个方法是否应该是自己的
        public override void Recycle()
        {
            ObjectPool<AssetBundleSceneAsset>.instance.Recycle(this);
        }
    }
}