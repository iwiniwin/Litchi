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
        private Asset m_Asset;

        public bool isDone => m_Asset.isDone;
        public float progress => m_Asset.progress;
        public Object asset => m_Asset.asset;

        public bool isInterrupt { get; set; } = false;

        public Action<Object> completed = delegate {};

        public AssetLoadRequest(Asset Asset)
        {
            m_Asset = Asset;
            // marktodo = 还是 +=
            m_Asset.completed = completed;
        }
        
        public override bool keepWaiting => !isDone;
    }
}