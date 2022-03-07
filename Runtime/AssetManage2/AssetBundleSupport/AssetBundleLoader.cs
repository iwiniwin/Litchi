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
            if(!CanLoad())
            {
                return;
            }
            // if(string.IsNullOrEmpty(assetName))
            // {
            //     return;
            // }
            state = AssetState.Loading;
            
            LoadTaskManager.instance.StartTask(this);
        }

        public override IEnumerator OnExecute()
        {
            if(refCount <= 0)
            {
                OnLoadFailed();
                yield break;
            }
            var simulate = true;
            if(simulate)
            {
                yield return null;
            }
            else
            {
                var url = AssetBundleSettings.GetAssetBundlePath(assetName);
                // marktodo
                var isWebGL = false;
                if(isWebGL)
                {

                }
                else
                {
                    var request = AssetBundle.LoadFromFileAsync(url);

                    m_AssetBundleCreateRequest = request;
                    yield return request;
                    m_AssetBundleCreateRequest = null;

                    if(!request.isDone)
                    {
                        Logger.LogError(string.Format("[AssetBundleLoader] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                        OnLoadFailed();
                        yield break;
                    }
                    assetBundle = request.assetBundle;
                }
            }
            
            state = AssetState.Ready;
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