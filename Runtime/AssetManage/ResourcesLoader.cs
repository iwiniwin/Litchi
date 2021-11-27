using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public class ResourcesLoader : IAssetLoader
    {
        public Object LoadAsset(string path, Type type)
        {
            return Resources.Load(path, type);
        }

        public AssetRequest LoadAssetAsync(string path, Type type)
        {
            var assetRequest = new AssetRequest();
            var resourceRequest = Resources.LoadAsync(path, type);
            resourceRequest.completed += (asyncOperation) => {
                assetRequest.asset = (asyncOperation as ResourceRequest).asset;
                assetRequest.isDone = true;
            };
            return assetRequest;
        }

        public void UnloadAsset(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}