using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Litchi.AssetManage2
{
    public class AssetBundlePathHelper
    {
        public static string PersistentDataPath
        {
            get;
        } = Application.persistentDataPath + "/";

        public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
        {

        }

        public static string[] GetAssetPaths(string assetBundleName, string assetName)
        {
#if UNITY_EDITOR
            return AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
#else
            return null;
#endif
        }

        public static Object LoadAssetAtPath(string assetPath, Type assetType)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#else
            return null;
#endif
        }

        public static T LoadAssetAtPath<T>(string assetPath) where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
        }

        // marktodo
        public static string GetPlatformName()
        {
            // return GetPlatformName
            return "Windows";
        }

        // marktodo
        public static string StreamingAssetsPath
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }
    }
}
