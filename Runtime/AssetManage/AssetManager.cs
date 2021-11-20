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
            // 加载Settings
            if(VFileSystem.Exists(Global.kAssetManageSettingsPath))
            {
                m_Settings = AssetManageSettings.LoadFromString(VFileSystem.ReadAllText(Global.kAssetManageSettingsPath));
            }
            else
            {
                m_Settings = new AssetManageSettings();
            }

            switch(m_Settings.assetLoaderType)
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