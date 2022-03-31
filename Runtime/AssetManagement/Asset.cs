using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public abstract class Asset : RefCounter
    {
        public ulong hash { get; protected set; }
        public Type type { get; protected set; }
        public AssetLoadPriority priority { get; protected set; }

        public Object asset { get; private set; }
        public bool isDone { get; private set; }

        public float progress { get; protected set; }

        public Action<Object> completed = delegate {};

        public abstract void Load();

        public abstract void LoadAsync();

        public abstract void Update();

        protected void OnLoadCompleted(Object asset)
        {
            this.asset = asset;
            this.isDone = true;
            completed(this.asset);
        }

        public virtual void Reset(ulong hash, Type type, AssetLoadPriority priority)
        {
            this.asset = null;
            this.isDone = false;
            this.progress = 0;
            this.hash = hash;
            this.type = type;
            this.priority = priority;
            this.completed = delegate {};
        }
    }
}