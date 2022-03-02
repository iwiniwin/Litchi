using System;
using System.IO;

namespace Litchi.AssetManage2
{
    public class AssetBundleSettings
    {
        private static Func<IResData> mAssetBundleConfigFileFactory = () => new ResData();
        public static Func<IResData> AssetBundleConfigFileFactory
        {
            set { mAssetBundleConfigFileFactory = value; }
        }

        private static IResData mAssetBundleConfigFile = null;
        public static IResData AssetBundleConfigFile
        {
            get 
            {
                if(mAssetBundleConfigFile == null)
                {
                    mAssetBundleConfigFile = mAssetBundleConfigFileFactory();
                }
                return mAssetBundleConfigFile;
            }
            set
            {
                mAssetBundleConfigFile = value;
            }
        }

        public static string GetAssetBundlePath(string assetBundleName)
        {
            string relativePath = "AssetBundles/" + AssetBundlePathHelper.GetPlatformName() + "/" + assetBundleName;
            string path = AssetBundlePathHelper.PersistentDataPath + relativePath;
            if(File.Exists(path))
            {
                return path;
            }
            return AssetBundlePathHelper.StreamingAssetsPath + relativePath;
        }
    }
}