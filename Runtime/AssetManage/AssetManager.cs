using Litchi.IO;
using Litchi.Config;

namespace Litchi.AssetManage
{
    public static class AssetManager
    {
        private static IAssetLoader m_Loader;
        private static AssetManageSettings m_Settings;

        public static void Init()
        {
            switch(Settings.assetManage.assetLoaderType)
            {
                case AssetLoaderType.AssetBundle:
                    m_Loader = new AssetBundleLoader();
                    break;
                case AssetLoaderType.Resources:
                    m_Loader = new ResourcesLoader();
                    break;
                default:
                    m_Loader = new AssetDatabaseLoader();
                    break;
            }
        }
    }
}