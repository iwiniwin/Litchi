#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public class AssetDatabaseLoader : IAssetLoader
    {
        public Object LoadAsset(string path, Type type)
        {
            Object assetObject = AssetDatabase.LoadAssetAtPath(path, type);
            return assetObject;
        }

        public AssetRequest LoadAssetAsync(string path, Type type)
        {
            return AssetRequest.SimulateRequest((request) => {
                request.asset = LoadAsset(path, type);
            });
        }

        public void UnloadAsset(Object asset)
        {
            // SceneManager.LoadScene
        }

        public void UnloadAllAssets()
        {
            // AssetDatabase.unload
        }
    }

}

#endif