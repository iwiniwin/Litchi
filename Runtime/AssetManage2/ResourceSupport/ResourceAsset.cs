using UnityEngine;
using System;
using System.Collections;

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

            // 不能直接使用Resources.Load(m_Path, assetType)代替，assetType禁止传入null，会报错ArgumentNullException: Value cannot be null.
            if(assetType != null)
            {
                asset = Resources.Load(m_Path, assetType);
            }
            else
            {
                asset = Resources.Load(m_Path);
            }

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
            
            LoadTaskManager.instance.PushTask(this);
            LoadTaskManager.instance.StartNextTask();
        }

        public override IEnumerator DoAsync(Action onFinish)
        {
            if(refCount <= 0)
            {
                OnLoadFailed();
                onFinish();
                yield break;
            }
            ResourceRequest resourceRequest = null;
            // 不能直接使用Resources.LoadAsync(m_Path, assetType)代替，assetType禁止传入null，会报错ArgumentNullException: Value cannot be null.
            if(assetType != null)
            {
                resourceRequest = Resources.LoadAsync(m_Path, assetType);
            }
            else
            {
                resourceRequest = Resources.LoadAsync(m_Path);
            }
            m_ResourceRequest = resourceRequest;
            yield return resourceRequest;
            m_ResourceRequest = null;
            
            if(!resourceRequest.isDone)
            {
                Logger.LogError(string.Format("[ResourceAsset] Failed to Load Asset<{0}> From Resources : {1}", assetType.FullName, m_Path));
                OnLoadFailed();
                onFinish();
                yield break;
            }
            asset = resourceRequest.asset;
            state = AssetState.Ready;
            onFinish();
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