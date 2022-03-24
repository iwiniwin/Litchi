using System;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public interface IAssetLoader 
    {
        Object Load(ulong hash, Type type);
        AssetLoadRequest LoadAsync(ulong hash, Type type);
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
