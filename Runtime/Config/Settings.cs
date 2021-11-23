using System;
using UnityEngine;
using Litchi.IO;
using Litchi.AssetManage;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Litchi.Config
{
    public static class Settings
    {
        public static AssetManageSettings s_AssetManage;
        public static AssetManageSettings assetManage
        {
            get
            {
                if(s_AssetManage == null)
                {
                    s_AssetManage = LoadFromJSON<AssetManageSettings>(Global.kAssetManageSettingsPath);
                }
                return s_AssetManage;
            }
        }

        public static T LoadFromJSON<T>(string jsonPath) where T : new()
        {
            if(VFileSystem.Exists(jsonPath))
            {
                string content = VFileSystem.ReadAllText(Global.kAssetManageSettingsPath);
                try
                {
                    return JsonUtility.FromJson<T>(content);
                }
                catch(Exception e)
                {
                    Logger.LogError(string.Format("read {0} failed : {1}", jsonPath, e.ToString()));
                }
            }
            return new T();
        }

        public static T LoadFromScriptObject<T>(string path) where T : ScriptableObject
        {
            if(VFileSystem.Exists(path))
            {
                return AssetManager.LoadAsset<T>(path);                
            }
            else
            {
                return ScriptableObject.CreateInstance<T>();
            }
        }

        public static void SaveToJSON(object o, string jsonPath)
        {
            string content = JsonUtility.ToJson(o);
            VFileSystem.WriteAllText(jsonPath, content);
        }

        public static void SaveToScriptableObject(ScriptableObject so, string path)
        {
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}