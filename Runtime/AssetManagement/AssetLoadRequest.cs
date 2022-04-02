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

    public class AssetLoadRequest : CustomYieldInstruction
    {
        private AssetAgent m_Agent;

        public bool isDone => m_Agent.isDone;
        public float progress => m_Agent.progress;
        public Object asset => m_Agent.asset;

        public bool isInterrupt { get; set; } = false;

        public Action<Object> completed => m_Agent.completed;

        public AssetLoadRequest(AssetAgent agent)
        {
            m_Agent = agent;
        }
        
        public override bool keepWaiting => !isDone;

        public void Dispose()
        {

        }
    }
}