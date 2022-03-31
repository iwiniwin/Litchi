using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetManager
    {
        public static Func<string, Type, Asset> AssetCreator { get; set; } = CreateAsset;

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
            var data = AssetSystem.instance.LoadAsync(path, typeof(T), priority, AssetCreator);
            return new AssetLoadRequest(data);
        }

        private static T TryLoad<T>(string path) where T : Object
        {
            var data = AssetSystem.instance.Load(path, typeof(T), AssetCreator);
            return data.asset as T;
        }

        private static T TryLoad<T>(ulong pathHash) where T : Object
        {
            // return AssetLoaderFactory.Get()
            // return AssetSystem.instance.load(pathHash)
            return default(T);
        }

        private static Asset CreateAsset(string path, Type type)
        {
            // return new ResourcesAsset();
            return new BundleAsset();
        }

        public static void Unload(Object asset)
        {
            AssetSystem.instance.Unload(asset);
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