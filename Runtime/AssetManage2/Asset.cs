using System;
using System.Collections;

namespace Litchi.AssetManage2
{
    public class Asset : RefCounter, IAsset, IPoolable
    {
        public string assetName { get; protected set; }

        public virtual string assetBundleName { get; protected set; }
        
        public Type assetType { get; set; } 
        public UnityEngine.Object asset { get; protected set; }

        private AssetState m_State;
        public AssetState state { 
            get { return m_State; } 
            set
            {
                m_State = value;
                if(m_State == AssetState.Ready)
                {
                    NotifyAssetLoadDone(true);
                }
            }
        }

        public float progress {
            get
            {
                switch(state)
                {
                    case AssetState.Loading:
                        return CalculateProgress();
                    case AssetState.Ready:
                        return 1;
                }
                return 0;
            }
        }

        protected virtual float CalculateProgress()
        {
            return 0;
        }

        public bool isRecycled { get; set; }

        private event Action<bool, IAsset> m_LoadDoneEvent;

        protected Asset(string assetName)
        {
            isRecycled = false;
            this.assetName = assetName;
        }

        public Asset()
        {
            isRecycled = false;
        }

        public void AddLoadDoneListener(Action<bool, IAsset> listener)
        {
            if(listener == null) return;
            if(state == AssetState.Ready)
            {
                listener(true, this);
                return;
            }
            m_LoadDoneEvent += listener;
        }

        public void RemoveLoadDoneListener(Action<bool, IAsset> listener)
        {
            if(listener == null) return;
            if(m_LoadDoneEvent == null) return;
            m_LoadDoneEvent -= listener;
        }

        protected void OnLoadFailed()
        {
            state = AssetState.Waiting;
            NotifyAssetLoadDone(false);
        }

        private void NotifyAssetLoadDone(bool result)
        {
            if(m_LoadDoneEvent != null)
            {
                m_LoadDoneEvent(result, this);
                m_LoadDoneEvent = null;
            }
        }

        protected bool CanLoad()
        {
            return state == AssetState.Waiting;
        }

        protected void HoldDependAsset()
        {

        }

        protected void UnHoldDependAsset()
        {

        }

        public virtual bool UnloadImage(bool flag)
        {
            return false;
        }

        public virtual bool LoadSync()
        {
            return false;
        }

        public virtual void LoadAsync()
        {

        }

        public virtual string[] GetDependencies()
        {
            return null;
        }

        public bool CheckDependenciesLoadDone()
        {
            var dependencies = GetDependencies();
            if(dependencies == null || dependencies.Length == 0)
            {
                return true;
            }
            for (int i = dependencies.Length - 1; i >= 0 ; i--)
            {
                // var 
            }
            return false;
        }

        public bool ReleaseAsset()
        {
            if(state == AssetState.Loading)
            {
                return false;
            }
            if(state != AssetState.Ready)
            {
                return true;
            }
            OnReleaseAsset();
            state = AssetState.Waiting;
            m_LoadDoneEvent = null;
            return true;
        }

        protected virtual void OnReleaseAsset()
        {
            if(asset != null)
            {
                AssetUnloadHelper.UnloadAsset(asset);
                asset = null;
            }
        }

        protected override void OnZeroRef()
        {
            if(state == AssetState.Loading)
            {
                return;
            }
            ReleaseAsset();
        }

        public virtual IEnumerator DoAsync(Action onFinish)
        {
            onFinish();
            yield break;
        }

        public virtual void Recycle()
        {

        }

        public virtual void OnRecycled()
        {
            assetName = null;
            m_LoadDoneEvent = null;
        }

        public override string ToString()
        {
            return string.Format("FullTypeName:{0}\t AssetName:{1}\t State:{2}\t RefCount:{3}", GetType().FullName, assetName, state, refCount);
        }
    }
}