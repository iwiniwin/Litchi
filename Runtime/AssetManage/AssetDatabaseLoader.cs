#if UNITY_EDITOR
using System;
using System.Collections.Generic;
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
    }
}

#endif