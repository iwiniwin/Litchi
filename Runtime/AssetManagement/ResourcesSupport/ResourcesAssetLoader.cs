using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class ResourcesAssetLoader : IAssetLoader 
    {
        public Object Load(ulong hash, Type type)
        {
            return Resources.Load(AssetDataManifest.GetHashPath(hash), type);
        }

        public AssetLoadRequest LoadAsync(ulong hash, Type type)
        {
            // var assetData = loadRequest.assetData;
            // string path = AssetDataManifest.GetHashPath(assetData.hash);
            // ResourceRequest request = Resources.LoadAsync(path, assetData.type);
            // request.priority = (int)loadRequest.priority;
            // yield return request;
            // assetData.asset = request.asset;
            return null;
        }
    }
}