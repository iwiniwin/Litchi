using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetManager
    {
        public static Func<string, Type, AssetAgent> assetAgentCreator { get; set; } = CreateAssetAgent;

        public static IBundleManifest bundleManifest { get => BundleManifestUtility.manifest; set => BundleManifestUtility.manifest = value; }

        public static T Load<T>(string path) where T : Object
        {
            return Load(path, typeof(T)) as T;
        }

        public static Object Load(string path, Type type)
        {
            return AssetAgentManager.instance.Load(path, type, assetAgentCreator).asset;
        }

        public static AssetLoadRequest LoadAsync<T>(string path, AssetLoadPriority priority = AssetLoadPriority.Normal) where T : Object
        {
            return LoadAsync(path, typeof(T), priority);
        }

        public static AssetLoadRequest LoadAsync(string path, Type type, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            var agent = AssetAgentManager.instance.LoadAsync(path, type, priority, assetAgentCreator);
            return new AssetLoadRequest(agent);
        }

        public static void Unload<T>(string path)
        {
            Unload(path, typeof(T));
        }

        public static void Unload(string path, Type type)
        {
            AssetAgentManager.instance.Unload(path, type);
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

        private static AssetAgent CreateAssetAgent(string path, Type type)
        {
            // return new ResourcesAssetAgent();
            return new BundleAssetAgent();
        }
    }
}