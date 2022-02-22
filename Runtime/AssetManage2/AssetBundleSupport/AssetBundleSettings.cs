using System;

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
    }
}