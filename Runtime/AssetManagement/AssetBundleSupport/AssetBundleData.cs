using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleData : RefCounter 
    {
        public AssetBundle assetBundle { get; private set; }

        public AssetBundleData(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
        }
    }
}