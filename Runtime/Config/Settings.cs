using Litchi.IO;
using Litchi.AssetManage;

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
                    if(VFileSystem.Exists(Global.kAssetManageSettingsPath))
                    {
                        s_AssetManage = AssetManageSettings.LoadFromString(VFileSystem.ReadAllText(Global.kAssetManageSettingsPath));
                    }
                    else
                    {
                        s_AssetManage = new AssetManageSettings();
                    }
                }
                return s_AssetManage;
            }
        }
    }
}