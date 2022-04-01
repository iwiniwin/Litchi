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

        public virtual bool unloadable { get => IsZeroRef(); }

        public Action<Object> completed = delegate {};

        protected abstract void OnLoad();

        protected abstract void OnLoadAsync();

        protected abstract void OnUnload();

        protected abstract void OnUpdate();

        protected abstract void OnInit();

        public void Load()
        {
            OnLoad();
        }

        public void LoadAsync()
        {
            OnLoadAsync();
        }

        public void Unload()
        {
            OnUnload();
            this.asset = null;
        }

        public void Update()
        {
            OnUpdate();
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
            OnInit();
        }

        protected void OnLoadCompleted(Object asset)
        {
            this.asset = asset;
            this.isDone = true;
            completed(this.asset);
        }

        public static string GenerateId(string path, Type type)
        {
            return path + "_" + type.FullName;
        }
    }
}