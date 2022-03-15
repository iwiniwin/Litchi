using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Litchi.AssetManage2
{
    public class ConfigFileUtility
    {
        public static ResData BuildEditorDataTable()
        {
            Logger.Info("Start Build Editor Data Table");
            var data = new ResData();
            AddAssetBundle2ResData(data);
            return data;
        }

        public static void AddAssetBundle2ResData(IResData assetBundleConfigFile, string[] names = null)
        {
#if UNITY_EDITOR
            // 删除资源数据库中所有未使用的assetBundle名称
            AssetDatabase.RemoveUnusedAssetBundleNames();
            // 获取资源数据库中所有的AssetBundle名称
            var assetBundleNames = names ?? AssetDatabase.GetAllAssetBundleNames();
            foreach (var assetBundleName in assetBundleNames)
            {
                // 获取其依赖的 AssetBundle 列表
                var dependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, false);
                AssetDataGroup group;
                var index = assetBundleConfigFile.AddAssetBundleName(assetBundleName, dependencies, out group);
                if(index < 0) continue;
                // 获取包含的所有资源的路径
                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                foreach (var item in assetPaths)
                {
                    // 获取路径下主资源对象的类型
                    var type = AssetDatabase.GetMainAssetTypeAtPath(item);
                    var code = type.ToCode();
                    group.AddAssetData(item.EndsWith(".unity") 
                        ? new AssetData(AssetPath2Name(item), (short)AssetLoadType.AssetBundleScene, index, assetBundleName, code)
                        : new AssetData(AssetPath2Name(item), (short)AssetLoadType.AssetBundleAsset, index, assetBundleName, code));
                }
            }
#endif
        }

        public static string AssetPath2Name(string assetPath)
        {
            var startIndex = assetPath.LastIndexOf("/") + 1;
            var endIndex = assetPath.LastIndexOf(".");
            if(endIndex > 0)
            {
                var length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length);
            }
            return assetPath.Substring(startIndex);
        }
    }
}