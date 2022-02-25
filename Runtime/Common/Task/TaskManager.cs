using UnityEngine;
using System.Collections.Generic;

namespace Litchi
{
    public class TaskManager<T> : MonoSingleton<TaskManager<T>> where T : IEnumeratorTask
    {   
        // 由继承的子类实现instance
        // TaskManager<T>是泛型，MonoBehaviours不支持，无法用于AddComponent
        public static new TaskManager<T> instance
        {
            get
            {
                UnityEngine.Debug.LogError("error");
                return null;
            }
        }

        private LinkedList<T> m_TaskList = new LinkedList<T>();

        private int m_MaxCoroutineCount = 8;  // 最快协程大概在6到8之间
        private int m_CurrentCoroutineCount;

        public void PushTask(T task)
        {
            if(task == null) return;
            m_TaskList.AddLast(task);
        }

        public void StartNextTask()
        {
            if(m_TaskList.Count == 0)
            {
                return;
            }
            if(m_CurrentCoroutineCount >= m_MaxCoroutineCount)
            {
                return;
            }
            var task = m_TaskList.First.Value;
            m_TaskList.RemoveFirst();
            ++ m_CurrentCoroutineCount;
            StartCoroutine(task.DoAsync(OnDoFinish));
        }

        private void OnDoFinish()
        {
            -- m_CurrentCoroutineCount;
            StartNextTask();
        }
    }
}
