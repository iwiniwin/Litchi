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

    public enum AssetLoadStatus
    {
        Wait,
        Loading,
        Done,
    }

    public class AssetLoadRequest : CustomYieldInstruction
    {
        private AssetData m_AssetData;

        public bool isDone => m_AssetData.isDone;
        public float progress => m_AssetData.progress;
        public Object asset => m_AssetData.asset;

        public bool isInterrupt { get; set; } = false;

        public Action<Object> completed = delegate {};

        public AssetLoadRequest(AssetData assetData)
        {
            m_AssetData = assetData;
            // marktodo = 还是 +=
            m_AssetData.completed = completed;
        }
        
        public override bool keepWaiting => !isDone;
    }
}