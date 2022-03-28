using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetBundleData : AssetData 
    {
        public override void Load()
        {
            
        }

        public override void LoadAsync()
        {
            
        }

        public override void Update()
        {
            
        }

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

        public AssetBundleData(string bundleID, AssetBundle assetBundle)
        {
            this.bundleID = bundleID;
            this.assetBundle = assetBundle;
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