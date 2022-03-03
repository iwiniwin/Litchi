using System.Collections;

namespace Litchi.AssetManage2
{
    public interface IResData
    {
        string[] GetAllDependencies(string url);

        void LoadFromFile(string outRes);

        void Reset();

        IEnumerator LoadFromFileAsync(string outRes);

        AssetData GetAssetData(AssetSearchKey key);

        int AddAssetBundleName(string abName, string[] depends, out AssetDataGroup group);
    }
}