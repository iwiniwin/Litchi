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
    }

    public class BundleManifest : IBundleManifest
    {
        public string GetBundleId(string path)
        {
            return null;
        }

        public string[] GetDirectDependencies(string bundleId)
        {
            return null;
        }

    }
}