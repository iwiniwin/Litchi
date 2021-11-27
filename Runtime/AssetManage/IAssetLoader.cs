using System;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public interface IAssetLoader
    {
        Object LoadAsset(string path, Type type);

        AssetRequest LoadAssetAsync(string path, Type type);
        
        void UnloadAsset(Object asset);
        void UnloadAllAssets();
    }
}