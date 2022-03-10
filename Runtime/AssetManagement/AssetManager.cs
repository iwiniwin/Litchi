using UnityEngine;

namespace Litchi.AssetManagement
{
    public class AssetManager
    {
        public static T Load<T>(string path) where T : Object
        {
            T result = TryLoad<T>(path);
            if(result == null)
            {
                Logger.LogError("加载失败：" + path);
            }
            return result;
        }

        public static AssetLoadHandle LoadAsync<T>(string path, AssetLoadPriority priority = AssetLoadPriority.Normal) where T : Object
        {
            return AssetDataManager.instance.LoadAsync(path, typeof(T), priority);
        }

        private static T TryLoad<T>(string path) where T : Object
        {
            // ulong pathHash = PathToHash(path);
            // return TryLoad<T>(pathHash);
            return AssetDataManager.instance.Load(path, typeof(T)) as T;
        }

        private static T TryLoad<T>(ulong pathHash) where T : Object
        {
            // return AssetLoaderFactory.Get()
            // return AssetDataManager.instance.load(pathHash)
            return default(T);
        }
    }
}