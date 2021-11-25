#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
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
            Timer.instance.StartCoroutine(Test());
            return new AssetRequest();
        }

        public IEnumerator Test()
        {
            yield return null;
        }
    }

}

#endif