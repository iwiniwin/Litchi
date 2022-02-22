using System.Collections;

namespace Litchi.AssetManage2
{
    public class ResData : IResData
    {
        public static string FileName = "asset_bundle_config.bin";

        public string[] GetAllDependencies(string url)
        {
            return null;
        }

        public void LoadFromFile(string outRes)
        {

        }

        public void Reset()
        {

        }

        public IEnumerator LoadFromFileAsync(string outRes)
        {
            return null;
        }

        public AssetData GetAssetData()
        {
            return null;
        }

        public int AddAssetBundleName(string abName, string[] depends)
        {
            return 0;
        }
    }
}