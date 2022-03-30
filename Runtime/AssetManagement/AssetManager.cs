using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetManager
    {
        public static Func<string, Type, AssetData> assetDataCreator { get; set; } = CreateAssetData;

        public static T Load<T>(string path) where T : Object
        {
            T result = TryLoad<T>(path);
            if(result == null)
            {
                Logger.Error("加载失败：" + path);
            }
            return result;
        }

        public static AssetLoadRequest LoadAsync<T>(string path, AssetLoadPriority priority = AssetLoadPriority.Normal) where T : Object
        {
            var data = AssetDataManager.instance.LoadAsync(path, typeof(T), priority, assetDataCreator);
            return new AssetLoadRequest(data);
        }

        private static T TryLoad<T>(string path) where T : Object
        {
            var data = AssetDataManager.instance.Load(path, typeof(T), assetDataCreator);
            return data.asset as T;
        }

        private static T TryLoad<T>(ulong pathHash) where T : Object
        {
            // return AssetLoaderFactory.Get()
            // return AssetDataManager.instance.load(pathHash)
            return default(T);
        }

        private static AssetData CreateAssetData(string path, Type type)
        {
            // return new ResourcesAssetData();
            return new AssetBundleAssetData();
        }

        public static void Unload(Object asset)
        {
            AssetDataManager.instance.Unload(asset);
        }

        public static void UnloadUnusedAssets()
        {
            // marktodo
            // 前面资源被zero ref，则调用UnloadUnusedAssets的时候就可以被清理了？
            Resources.UnloadUnusedAssets();
        }

        public static void UnloadAssetBundle()
        {

        }
    }
}