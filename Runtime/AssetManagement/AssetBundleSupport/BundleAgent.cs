using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class BundleAgent : AssetAgent 
    {
        private AssetBundleCreateRequest m_Request;
        private List<BundleAgent> m_Dependencies = new List<BundleAgent>();
        public override void Load()
        {
            string[] dependencies = null;
            // BundleAgent BundleAgent = new BundleAgent();
            // BundleAgent.Load();
            // mainBundle = BundleAgent.assetBundle;

            if(dependencies != null && dependencies.Length > 0)
            {
                foreach(var depend in dependencies)
                {
                    // BundleAgent data = new BundleAgent();
                    // data.Load();
                    // 缓存data?
                    AssetAgentManager.instance.Load(depend, type, (vpath, type) => {
                        return new BundleAgent();
                    });
                }
            }
            assetBundle = AssetBundle.LoadFromFile("path");
            OnLoadCompleted(assetBundle);
        }

        public override void LoadAsync()
        {
            string[] dependencies = null;
            if(dependencies != null && dependencies.Length > 0)
            {
                foreach(var depend in dependencies)
                {
                    // BundleAgent data = new BundleAgent();
                    // data.Load();
                    // 缓存data?
                    var data = AssetAgentManager.instance.LoadAsync(depend, type, priority, (vpath, type) => {
                        return new BundleAgent();
                    }) as BundleAgent;
                    m_Dependencies.Add(data);
                }
            }
            m_Request = AssetBundle.LoadFromFileAsync("path");
        }

        public override void Update()
        {
            if(m_Request == null) return;
            progress = CalcProgress();
            if(IsLoadDependenciesDone() && m_Request.isDone)
            {
                assetBundle = m_Request.assetBundle;
                OnLoadCompleted(assetBundle);
            }
        }

        public bool IsLoadDependenciesDone()
        {
            foreach(var dependData in m_Dependencies)
            {
                if(!dependData.isDone)
                {
                    return false;
                }
            }
            return true;
        }

        public float CalcProgress()
        {
            float curProgress = m_Request.progress;
            foreach(var dependData in m_Dependencies)
            {
                curProgress += dependData.progress;
            }
            return curProgress / (m_Dependencies.Count + 1);
        }

        ///////////////////////////////////////分隔符///////////////////////////////////////

        public static readonly long kBundleTimeOut = 10000;  // ms

        public AssetBundle assetBundle { get; private set; }

        public string bundleID { get; private set; }

        private long m_LastUsedTime = Stopwatch.GetTimestamp();

        public long elapsedMilliseconds
        {
            get
            {
                return (Stopwatch.GetTimestamp() - m_LastUsedTime) * 1000 / Stopwatch.Frequency;
            } 
        }

        public BundleAgent()
        {
            // this.bundleID = bundleID;
            // this.assetBundle = assetBundle;
        }

        public override void Retain()
        {
            base.Retain();
            m_LastUsedTime = Stopwatch.GetTimestamp();
        }

        public override void Release()
        {
            base.Release();
            m_LastUsedTime = Stopwatch.GetTimestamp();
        }

        public bool TimeOut()
        {
            return elapsedMilliseconds > kBundleTimeOut;
        }

        public void Unload(bool unloadAllLoadedObjects)
        {
            if(assetBundle != null)
            {
                assetBundle.Unload(unloadAllLoadedObjects);
                assetBundle = null;
            }
        }

        // 是否可卸载
        public bool Unloadable()
        {
            return IsZeroRef();
        }
    }
}