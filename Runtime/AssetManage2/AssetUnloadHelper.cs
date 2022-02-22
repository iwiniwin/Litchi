using UnityEngine;

namespace Litchi.AssetManage2
{
    public static class AssetUnloadHelper
    {
        /// <summary>
        /// 资源卸载
        /// </summary>
        public static void UnloadAsset(Object asset)
        {
            if(asset is GameObject)
            {

            }
            else
            {
                Resources.UnloadAsset(asset);
            }
        }
    }
}