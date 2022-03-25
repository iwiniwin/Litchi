using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    // marktodo 换位置，不应该在这个文件
    public enum AssetLoadPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
    } 

    public class AssetLoadRequest : CustomYieldInstruction
    {
        public bool isDone { get; internal set; }
        public float progress { get; internal set; }
        public bool isInterrupt { get; set; } = false;
        public Object asset { get; private set; }

        public ulong hash { get; private set; }
        public Type type { get; private set; }

        public AssetLoadPriority priority { get; private set; }

        public Action<AssetLoadRequest> onCompleted = delegate {};

        public AssetLoadRequest(ulong hash, Type type, AssetLoadPriority priority)
        {
            this.hash = hash;
            this.type = type;
            this.priority = priority;
        }
        
        public void Clear()
        {
            if(asset != null)
            {
                // unload marktodo
                asset = null;
            }
            isDone = false;
            progress = 0;
            isInterrupt = false;
        }

        internal void OnLoadCompleted(Object asset)
        {
            this.asset = asset;
            isDone = true;
            onCompleted(this);
        }

        public override bool keepWaiting => !isDone;
    }
}