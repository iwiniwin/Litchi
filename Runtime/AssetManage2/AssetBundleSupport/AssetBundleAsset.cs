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
            bool simulate = true;
            
            var key = AssetSearchKey.Allocate(assetBundleName, null, typeof(AssetBundle));
            var bundleLoader = AssetManager.instance.GetAsset<AssetBundleLoader>(key);
            key.Recycle();

            if(simulate && !string.Equals(assetName, "assetbundlemanifest"))  // marktodo 大小写
            {
                var assetPaths = AssetBundlePathHelper.GetAssetPaths(bundleLoader.assetName, assetName);
                if(assetPaths.Length == 0)
                {
                    Logger.Error(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                    OnLoadFailed();
                    return false;
                }

                RetainDependentAssets();

                state = AssetState.Loading;

                if(assetType != null)
                {
                    obj = AssetBundlePathHelper.LoadAssetAtPath(assetPaths[0], assetType);
                }
                else
                {
                    obj = AssetBundlePathHelper.LoadAssetAtPath<Object>(assetPaths[0]);
                }

            }
            else
            {
                if(bundleLoader == null || !bundleLoader.assetBundle)
                {
                    Logger.Error(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}, Not Find AssetBundle : {2}", assetType.FullName, assetName, assetBundleName));
                    // markdown 不调用OnLoadFailed？
                    return false;
                }

                RetainDependentAssets();

                state = AssetState.Loading;

                if(assetType != null)
                {
                    obj = bundleLoader.assetBundle.LoadAsset(assetName, assetType);
                }
                else
                {
                    obj = bundleLoader.assetBundle.LoadAsset(assetName);
                }
            }

            ReleaseDependentAssets();

            if(obj == null)
            {
                Logger.Error(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
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
            
            LoadTaskManager.instance.StartTask(this);
        }

        public override IEnumerator OnExecute()
        {
            if(refCount <= 0)
            {
                OnLoadFailed();
                yield break;
            }
            
            var key = AssetSearchKey.Allocate(assetBundleName, null, typeof(AssetBundle));
            var bundleLoader = AssetManager.instance.GetAsset<AssetBundleLoader>(key);
            key.Recycle();

            bool simulate = true;
            if(simulate && !string.Equals(assetName, "assetbundlemanifest"))  // marktodo 大小写
            {
                var assetPaths = AssetBundlePathHelper.GetAssetPaths(bundleLoader.assetName, assetName);
                if(assetPaths.Length == 0)
                {
                    Logger.Error(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                    OnLoadFailed();
                    yield break;
                }

                // 确保加载过程中依赖资源不被释放，目前只有AssetBundleAsset需要处理该情况
                RetainDependentAssets();

                state = AssetState.Loading;

                // 模拟异步 等一帧
                yield return new WaitForEndOfFrame();

                ReleaseDependentAssets();

                if(assetType != null)
                {
                    asset = AssetBundlePathHelper.LoadAssetAtPath(assetPaths[0], assetType);
                }
                else
                {
                    asset = AssetBundlePathHelper.LoadAssetAtPath<Object>(assetPaths[0]);
                }

            }
            else
            {
                if(bundleLoader == null || !bundleLoader.assetBundle)
                {
                    Logger.Error(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}, Not Find AssetBundle : {2}", assetType.FullName, assetName, assetBundleName));
                    OnLoadFailed();
                    yield break;
                }

                RetainDependentAssets();

                state = AssetState.Loading;

                AssetBundleRequest request = null;

                if(assetType != null)
                {
                    request = bundleLoader.assetBundle.LoadAssetAsync(assetName, assetType);
                }
                else
                {
                    request = bundleLoader.assetBundle.LoadAssetAsync(assetName);
                }

                m_AssetBundleRequest = request;
                yield return request;
                m_AssetBundleRequest = null;

                ReleaseDependentAssets();

                if(!request.isDone)
                {
                    Logger.Error(string.Format("[AssetBundleAsset] Failed to Load Asset<{0}> : {1}", assetType.FullName, assetName));
                    OnLoadFailed();
                    yield break;
                }

                asset = request.asset;
            }

            state = AssetState.Ready;
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
                Logger.Error(string.Format("[AssetBundleAsset] Not Find AssetData For Asset : {0}", assetName));
                return;
            }

            var name = config.assetBundleName;
            if(string.IsNullOrEmpty(name))
            {
                Logger.Error(string.Format("[AssetBundleAsset] Not Find AssetData In Config({0}) : {1}", config.assetBundleIndex, m_PassInAssetBundleName));
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