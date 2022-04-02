using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public interface IBundleManifest
    {
        string GetBundleId(string path);
        string[] GetDirectDependencies(string bundleId);
        string GetAssetName(string path);
    }

    public class DefaultBundleManifest : IBundleManifest
    {
        public string GetBundleId(string path)
        {
            return null;
        }

        public string[] GetDirectDependencies(string bundleId)
        {
            return null;
        }

        public string GetAssetName(string path)
        {
            return "";
        }
    }

    public class BundleManifestUtility
    {
        public static IBundleManifest manifest { get; set; } = new DefaultBundleManifest();

        public static string GetBundleId(string path)
        {
            return manifest.GetBundleId(path);
        }

        public static string[] GetDirectDependencies(string bundleId)
        {
            return manifest.GetDirectDependencies(bundleId);
        }

        public static string GetAssetName(string bundleId)
        {
            return manifest.GetAssetName(bundleId);
        }
    }
}