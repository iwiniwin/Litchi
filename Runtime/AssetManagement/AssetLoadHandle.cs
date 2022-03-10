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

    public class AssetLoadHandle : CustomYieldInstruction
    {
        public bool isDone { get; internal set; }
        public float progress { get; internal set; }
        public bool isInterrupt { get; set; } = false;

        public AssetLoadPriority priority { get; private set; }

        private AssetData m_AssetData;
        public AssetData assetData
        {
            get
            {
                var temp = m_AssetData;
                // m_AssetData = null;  // marktodo
                return temp;
            }
            set
            {
                m_AssetData = value;
            }
        }

        public AssetLoadHandle(AssetLoadPriority priority)
        {
            this.priority = priority;
        }
        
        public void Clear()
        {
            if(m_AssetData != null)
            {
                // unload marktodo
                m_AssetData = null;
            }
            isDone = false;
            progress = 0;
            isInterrupt = false;
        }

        public override bool keepWaiting => !isDone;
    }
}