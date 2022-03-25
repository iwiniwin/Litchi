using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public interface IAssetLoader 
    {
        Object Load(ulong hash, Type type);
        IEnumerator LoadAsync(ulong hash, Type type, Action<Object> assetSetter);
    }

    public interface ICustomAssetLoader : IAssetLoader
    {
        bool Match(string path);
    }

    public enum AssetLoaderType
    {
        AssetBundle,
        Resources,
        AssetDatabase,
        Custom,
    }
}
