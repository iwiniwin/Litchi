using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetData : RefCounter
    {
        public ulong hash { get; internal set; }
        public Object asset { get; internal set; }
        public Type type { get; internal set; }

        public AssetData(ulong hash, Type type, Object asset)
        {
            this.hash = hash;
            this.type = type;
            this.asset = asset;
        }
    }
}