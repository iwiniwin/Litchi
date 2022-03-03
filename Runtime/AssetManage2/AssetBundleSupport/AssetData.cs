using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

namespace Litchi.AssetManage2
{
    [Serializable]
    public class AssetData
    {
        public string assetName { get; set; }
        public string assetBundleName  { get; set; }
        public int assetBundleIndex { get; set; }
        public short assetType { get; set; }
        public short assetObjectTypeCode  { get; set; } = 0;

        public string UUID
        {
            get 
            {
                return string.IsNullOrEmpty(assetBundleName) ? assetName : assetBundleName + assetName;
            }
        }

        public AssetData(string assetName, short assetType, int assetBundleIndex, string assetBundleName, short assetObjectTypeCode = 0)
        {
            this.assetName = assetName;
            this.assetType = assetType;
            this.assetBundleIndex = assetBundleIndex;
            this.assetBundleName = assetBundleName;
            this.assetObjectTypeCode = assetObjectTypeCode;
        }
    }

    public static partial class assetObjectTypeCode
    {
        public const short GameObject  = 1;
        public const short AudioClip   = 2;
        public const short Sprite      = 3;
        public const short Scene       = 4;
        public const short SpriteAtlas = 5;
        public const short Mesh        = 6;
        public const short Texture2D   = 7;
        public const short TextAsset   = 8;
        public const short AssetBundle   = 9;

        public static Type GameObjectType  = typeof(GameObject);
        public static Type AudioClipType   = typeof(AudioClip);
        public static Type SpriteType      = typeof(Sprite);
        public static Type SceneType       = typeof(Scene);
        public static Type SpriteAtlasType = typeof(SpriteAtlas);
        public static Type MeshType        = typeof(Mesh);
        public static Type Texture2DType   = typeof(Texture2D);
        public static Type TextAssetType   = typeof(TextAsset);
        public static Type AssetBundleType   = typeof(AssetBundle);

        public static short ToCode(this Type type)
        {
            if (type == GameObjectType)
            {
                return GameObject;
            }

            if (type == AudioClipType)
            {
                return AudioClip;
            }

            if (type == SpriteType)
            {
                return Sprite;
            }

            if (type == SceneType)
            {
                return Scene;
            }

            if (type == SpriteAtlasType)
            {
                return SpriteAtlas;
            }

            if (type == MeshType)
            {
                return Mesh;
            }

            if (type == Texture2DType)
            {
                return Texture2D;
            }

            if (type == TextAssetType)
            {
                return TextAsset;
            }

            if (type == AssetBundleType)
            {
                return AssetBundle;
            }

            return 0;
        }
    }
}