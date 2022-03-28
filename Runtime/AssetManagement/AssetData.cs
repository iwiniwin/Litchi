using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public abstract class AssetData : RefCounter
    {
        // public ulong hash { get; protected set; }
        // public Type type { get; protected set; }

        public Object asset { get; private set; }
        public bool isDone { get; private set; }

        public float progress { get; protected set; }

        public abstract void Load(ulong hash, Type type);

        public abstract void LoadAsync(ulong hash, Type type);

        public abstract void Update();

        protected void OnLoadCompleted(Object asset)
        {
            this.asset = asset;
            this.isDone = true;
        }

        public virtual void Reset()
        {
            this.asset = null;
            this.isDone = false;
            this.progress = 0;
        }
    }
}