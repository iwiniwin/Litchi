using UnityEngine;
using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage
{
    public class AssetRequest : YieldInstruction
    {
        private bool m_IsDone;
        public bool isDone { 
            get
            {
                return m_IsDone;
            }
            internal set
            {
                m_IsDone = value;
                if(m_IsDone && completed != null)
                {
                    completed(this);
                }
            }
        }
        public Object asset { get; internal set;}
        public event Action<AssetRequest> completed;

        public static AssetRequest SimulateRequest(Action<AssetRequest> ac)
        {
            AssetRequest request = new AssetRequest();
            Timer.instance.StartCoroutine(SimulateAsync(request, ac));
            return request;
        }

        private static IEnumerator SimulateAsync(AssetRequest request, Action<AssetRequest> ac)
        {
            yield return null; 
            ac(request);
            request.isDone = true;
        }
    }
}