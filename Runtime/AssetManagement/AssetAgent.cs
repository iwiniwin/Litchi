using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public abstract class AssetAgent : RefCounter
    {
        public string path { get; protected set; }
        public Type type { get; protected set; }
        public AssetLoadPriority priority { get; protected set; }

        public string id { get; private set; }

        public Object asset { get; private set; }
        public bool isDone { get; private set; }

        public float progress { get; protected set; }

        public bool unloadable { get => IsZeroRef(); }

        public Action<Object> completed = delegate {};

        public abstract void Load();

        public abstract void LoadAsync();

        public abstract void Unload();

        public abstract void Update();

        protected void OnLoadCompleted(Object asset)
        {
            this.asset = asset;
            this.isDone = true;
            completed(this.asset);
        }

        public virtual void Init(string path, Type type, AssetLoadPriority priority)
        {
            this.asset = null;
            this.isDone = false;
            this.progress = 0;
            this.path = path;
            this.type = type;
            this.priority = priority;
            this.completed = delegate {};
            this.id = GenerateId(path, type);
        }

        public static string GenerateId(string path, Type type)
        {
            return path + "_" + type.FullName;
        }
    }
}