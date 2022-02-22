using UnityEngine;

namespace Litchi.AssetManage2
{
    public class ResourceAsset : Asset
    {
        private ResourceRequest m_ResourceRequest;

        private string m_Path;

        // marktodo
        public static ResourceAsset Allocate(string name)
        {
            var asset = ObjectPool<ResourceAsset>.instance.Allocate();
            if(asset != null)
            {
                asset.assetName = name;
                asset.m_Path = name;
            }
            return asset;
        }

        public override bool LoadSync()
        {
            if(!CanLoad())
            {
                return false;
            }
            if(string.IsNullOrEmpty(assetName))
            {
                return false;
            }
            state = AssetState.Loading;

            asset = Resources.Load(m_Path, assetType);

            if(asset == null)
            {
                Logger.LogError(string.Format("[ResourceAsset] Failed to Load Asset<{0}> From Resources : {1}", assetType.FullName, m_Path));
                OnLoadFailed();
                return false;
            }
            
            state = AssetState.Ready;
            return true;
        }

        public override void LoadAsync()
        {
            if(!CanLoad())
            {
                return;
            }
            if(string.IsNullOrEmpty(assetName))
            {
                return;
            }
            state = AssetState.Loading;
            // pushLoadTask  // marktodo
        }

        // marktodo 这个方法是否应该是自己的
        public override void Recycle()
        {
            ObjectPool<ResourceAsset>.instance.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            return 1;  // marktodo
        }
    }
}