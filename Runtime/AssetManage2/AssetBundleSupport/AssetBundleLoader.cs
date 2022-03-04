using UnityEngine;
using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage2
{
    public class AssetBundleLoader : Asset
    {
        private bool m_UnloadFlag = true;
        private string[] m_DependAssetList;
        private AsyncOperation m_AssetBundleCreateRequest;

        public AssetBundle assetBundle
        {
            get { return (AssetBundle) asset; }
            set { asset = value; }
        }

        // marktodo
        public static AssetBundleLoader Allocate(string name)
        {
            var asset = ObjectPool<AssetBundleLoader>.instance.Allocate();
            if(asset != null)
            {
                asset.assetName = name;
                asset.assetType = typeof(AssetBundle);
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
            state = AssetState.Loading;

            var simulate = true;
            if(simulate)
            {

            }
            else
            {
                var url = AssetBundleSettings.GetAssetBundlePath(assetName);
                AssetBundle bundle = AssetBundle.LoadFromFile(url);
                m_UnloadFlag = true;
                if(bundle == null)
                {
                    Logger.LogError(string.Format("[AssetBundleLoader] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                    OnLoadFailed();
                    return false;
                }
                assetBundle = bundle;
            }

            state = AssetState.Ready;
            
            return true;
        }

        public override void LoadAsync()
        {
            // if(!CanLoad())
            // {
            //     return;
            // }
            // if(string.IsNullOrEmpty(assetName))
            // {
            //     return;
            // }
            // state = AssetState.Loading;
            
            // LoadTaskManager.instance.PushTask(this);
            // LoadTaskManager.instance.StartNextTask();
        }

        public override IEnumerator DoAsync(Action onFinish)
        {
            // if(refCount <= 0)
            // {
            //     OnLoadFailed();
            //     onFinish();
            //     yield break;
            // }
            // ResourceRequest resourceRequest = null;
            // // 不能直接使用Resources.LoadAsync(m_Path, assetType)代替，assetType禁止传入null，会报错ArgumentNullException: Value cannot be null.
            // if(assetType != null)
            // {
            //     resourceRequest = Resources.LoadAsync(m_Path, assetType);
            // }
            // else
            // {
            //     resourceRequest = Resources.LoadAsync(m_Path);
            // }
            // m_ResourceRequest = resourceRequest;
            // yield return resourceRequest;
            // m_ResourceRequest = null;
            
            // if(!resourceRequest.isDone)
            // {
            //     Logger.LogError(string.Format("[AssetBundleLoader] Failed to Load Asset<{0}> From Resources : {1}", assetType.FullName, m_Path));
            //     OnLoadFailed();
            //     onFinish();
            //     yield break;
            // }
            // asset = resourceRequest.asset;
            // state = AssetState.Ready;
            // onFinish();
            yield break;
        }

        // marktodo 这个方法是否应该是自己的
        public override void Recycle()
        {
            ObjectPool<AssetBundleLoader>.instance.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            return 1;  // marktodo
        }

        protected void InitAssetBundleName()
        {
            m_DependAssetList = AssetBundleSettings.AssetBundleConfigFile.GetAllDependencies(assetName);
        }
    }
}