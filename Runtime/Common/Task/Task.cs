using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litchi
{
    public interface ITask
    {
        void OnStart();
        void OnShutdown();
    }

    public interface IUpdateTask : ITask
    {        
        void OnUpdate(float dt);
        bool finish { get; }
    }

    public interface IExecuteTask : ITask
    {
        IEnumerator OnExecute();
    }

    // public class TaskHandler : CustomYieldInstruction
    // {
    //     public ITask task { get; private set; }
    //     public TaskHandler(ITask task)
    //     {
    //         this.task = task;
    //     }

    //     public override bool keepWaiting 
    //     {
    //         get
    //         {
    //             return task.finish;
    //         }
    //     }
    // }

    public abstract class UpdateTask : IUpdateTask
    {
        private List<IUpdateTask> m_SubTaskList = new List<IUpdateTask>();
        private bool m_StartedSubTasks = false;

        protected void AddSubTask(IUpdateTask task)
        {
            m_SubTaskList.Add(task);
        }

        public abstract void OnStart();

        public abstract bool success { get; }

        public abstract bool finish { get; }

        public virtual void OnUpdate(float dt)
        {
            if(m_StartedSubTasks && m_SubTaskList.Count > 0)
            {
                for (int i = m_SubTaskList.Count - 1; i >= 0 ; i--)
                {
                    if(m_SubTaskList[i].finish)
                    {
                        m_SubTaskList.RemoveAt(i);
                    }
                }
                if(m_SubTaskList.Count <= 0) return; 
                foreach (var task in m_SubTaskList)
                {
                    task.OnUpdate(dt);
                }
            }
        }

        public virtual void OnShutdown()
        {
            if(m_StartedSubTasks)
            {
                foreach (var task in m_SubTaskList)
                {
                    task.OnShutdown();
                }
                m_SubTaskList.Clear();
            }
        }

        public void StartSubTask()
        {
            if(m_StartedSubTasks) return;
            
            foreach (var task in m_SubTaskList)
            {
                task.OnStart();
            }

            m_StartedSubTasks = true;
        }

        public bool IsFinishSubTasks()
        {
            return m_StartedSubTasks && m_SubTaskList.Count == 0;
        }
    }

    public abstract class ExecuteTask : IExecuteTask
    {
        private LinkedList<IExecuteTask> m_SubTaskList = new LinkedList<IExecuteTask>();

        protected void AddSubTask(IExecuteTask task)
        {
            m_SubTaskList.AddLast(task);
        }

        public virtual void OnStart() 
        {

        }

        public abstract IEnumerator OnExecute();

        public virtual void OnShutdown()
        {

        }

        public IEnumerator ExecuteSubTask()
        {
            var first = m_SubTaskList.First;
            
            while(first != null)
            {
                m_SubTaskList.RemoveFirst();
                var task = first.Value;
                task.OnStart();
                yield return task.OnExecute();
                first = m_SubTaskList.First;
            }
        }
    }
}
