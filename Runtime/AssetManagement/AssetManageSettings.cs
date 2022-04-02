using UnityEngine;
using System;

namespace Litchi.AssetManagement
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