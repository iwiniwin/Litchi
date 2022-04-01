using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class ResourcesAssetLoader : IAssetLoader 
    {
        public Object Load(string path, Type type)
        {
            return Resources.Load(path, type);
        }

        public IEnumerator LoadAsync(string path, Type type, Action<Object> assetSetter)
        {
            ResourceRequest request = Resources.LoadAsync(path, type);
            yield return request;
            assetSetter(request.asset);
        }
    }
}