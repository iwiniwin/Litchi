using System;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public interface IAssetLoader
    {
        Object LoadAsset(string path, Type type);

        AssetRequest LoadAssetAsync(string path, Type type);
        
        // void UnloadAsset();
        // void UnloadAllAssets();

        // void LoadScene();
        // void LoadSceneAsync();
        // void UnloadSceneAsync();
    }
}