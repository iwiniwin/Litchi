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
            return Resources.Load(AssetManifest.GetHashPath(hash), type);
        }

        public IEnumerator LoadAsync(ulong hash, Type type, Action<Object> assetSetter)
        {
            string path = AssetManifest.GetHashPath(hash);
            ResourceRequest request = Resources.LoadAsync(path, type);
            yield return request;
            assetSetter(request.asset);
        }
    }
}