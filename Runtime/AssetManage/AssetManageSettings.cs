using UnityEngine;
using System;

namespace Litchi.AssetManage
{
    public enum AssetLoaderType
    {
        AssetBundle,
        Resources,
        AssetDatabase
    }

    [Serializable]
    public class AssetManageSettings
    {
        public AssetLoaderType assetLoaderType = AssetLoaderType.AssetDatabase;
    }
}