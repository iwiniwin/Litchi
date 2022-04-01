using UnityEngine;
using System.Collections.Generic;

namespace Litchi.AssetManagement
{
    public interface IBundleManifest
    {
        string GetBundleID(string path);
        string[] GetDirectDependencies(string bundleID);
        string GetPath(string bundleID);
        string GetPathName(string path);
    }
    public class BundleManifest : IBundleManifest
    {
        public string GetBundleID(string path)
        {
            return "null";
        }

        public string[] GetDirectDependencies(string bundleID)
        {
            return new string[]{};
        }

        public string GetPath(string bundleID)
        {
            return "jjjj " + bundleID;
        }

        public string GetPathName(string path)
        {
            return null;
        }
    }
}