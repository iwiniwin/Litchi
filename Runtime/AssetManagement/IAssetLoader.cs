using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public interface IAssetLoader 
    {
        Object Load(string path, Type type);
        IEnumerator LoadAsync(string path, Type type, Action<Object> assetSetter);
    }

    public interface ICustomAssetLoader : IAssetLoader
    {
        bool Match(string path);
    }

    public enum AssetLoaderType
    {
        AssetBundle,
        Resources,
        Assetbase,
        Custom,
    }
}
