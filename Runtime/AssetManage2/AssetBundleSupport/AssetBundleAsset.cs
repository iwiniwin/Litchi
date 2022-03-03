using UnityEngine;
using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage2
{
    public class AssetBundleAsset : Asset
    {
        protected string[] m_AssetBundleArray;
        protected AssetBundleRequest m_AssetBundleRequest;
        private string m_PassInAssetBundleName;

        // marktodo
        public static AssetBundleAsset Allocate(string name, string assetBundleName, Type assetType)
        {
            var asset = ObjectPool<AssetBundleAsset>.instance.Allocate();
            if(asset != null)
            {
                asset.assetName = name;
                asset.m_PassInAssetBundleName = assetBundleName;
                asset.assetType = assetType;
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

            Object obj = null;
            bool simulate = false;
            if(simulate && !string.Equals(assetName, "assetbundlemanifest"))  // marktodo 大小写
            {
                // var key = AssetSearchKey.Allocate(null, assetBundleName, typeof(AssetBundle));
                // var bundleLoader = AssetManager.instance.GetAsset<AssetBundleLoader>(key);
                // key.Recycle();

                // var assetPaths = AssetBundlePathHelper.GetAssetPaths(bundleLoader.assetName, assetName);
                // if(assetPaths.Length == 0)
                // {
                //     Logger.LogError(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                //     OnLoadFailed();
                //     return false;
                // }
            }
            else
            {
                var key = AssetSearchKey.Allocate(assetBundleName, null, typeof(AssetBundle));
                var bundleLoader = AssetManager.instance.GetAsset<AssetBundleLoader>(key);
                key.Recycle();
                if(bundleLoader == null || !bundleLoader.assetBundle)
                {
                    Logger.LogError(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}, Not Find AssetBundle : {2}", assetType.FullName, assetName, assetBundleName));
                    return false;
                }

                RetainDependentAssets();

                state = AssetState.Loading;

                if(assetType != null)
                {
                    UnityEngine.Debug.Log("[Lei] aaaaaa" + " " + assetName + "   " + assetType);
                    obj = bundleLoader.assetBundle.LoadAsset(assetName, assetType);
                    UnityEngine.Debug.Log("[Lei] obj" + " " + obj);
                }
                else
                {
                    obj = bundleLoader.assetBundle.LoadAsset(assetName);
                }

                // if(bundleLoader == null || !bundleLoader.assetbund)
            }

            ReleaseDependentAssets();

            if(obj == null)
            {
                Logger.LogError(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                OnLoadFailed();
                return false;
            }

            asset = obj;
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
            //     Logger.LogError(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> From Resources : {1}", assetType.FullName, m_Path));
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
            ObjectPool<AssetBundleAsset>.instance.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            return 1;  // marktodo
        }

        protected void InitAssetBundleName()
        {
            m_AssetBundleArray = null;
            var key = AssetSearchKey.Allocate(assetName, m_PassInAssetBundleName, assetType);
            var config = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(key);
            key.Recycle();
            if(config == null)
            {
                Logger.LogError(string.Format("[AssetBundleAsset] Not Find AssetData For Asset : {0}", assetName));
                return;
            }

            var name = config.assetBundleName;
            if(string.IsNullOrEmpty(name))
            {
                Logger.LogError(string.Format("[AssetBundleAsset] Not Find AssetData In Config({0}) : {1}", config.assetBundleIndex, m_PassInAssetBundleName));
                return;
            }

            m_AssetBundleArray = new string[]{name};
            assetBundleName = name;
        }

        public override string[] GetDependencies()
        {
            return m_AssetBundleArray;
        }
    }
}