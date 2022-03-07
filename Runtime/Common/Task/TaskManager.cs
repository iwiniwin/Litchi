using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Litchi
{
    public class TaskManager<T> : MonoSingleton<T> where T : TaskManager<T>
    {   
        private static int m_MaxProcessTaskCount = 8;  // 最快协程大概在6到8之间 
        private Queue<ITask> m_ToDoTaskQueue = new Queue<ITask>();
        private ITask[] m_ProcessTasks = new ITask[m_MaxProcessTaskCount];

        public void StartTask(ITask task)
        {
            m_ToDoTaskQueue.Enqueue(task);
        }

        // public void ExecuteTask(IExecuteTask task)
        // {
        //     int index = FindEmptyIndex();
        //     if(index == -1) 
        //     {
        //         m_ToDoTaskQueue.Enqueue(task);
        //     }
        //     else
        //     {
        //         task.OnStart();
        //         yield return task.OnExecute();
        //         // ExecuteTask(i, task);
        //     }
        // }

        // public TaskHandlervv Ufff()
        // {

        // }

        // private int FindEmptyIndex()
        // {
        //     for (int i = 0; i < m_ProcessTasks.Length; i++)
        //     {
        //         if(m_ProcessTasks[i] == null)
        //         {
        //             return i;
        //         }
        //     }
        //     return -1;
        // }

        private void TryStartTask(int index)
        {
            if(m_ToDoTaskQueue.Count > 0)
            {
                var task = m_ToDoTaskQueue.Dequeue();
                if(task is IUpdateTask)
                {
                    StartTask(task as IUpdateTask);
                }
                else if(task is IExecuteTask)
                {
                    StartTask(index, task as IExecuteTask);
                }
            }
        }

        private void StartTask(int index, IUpdateTask task)
        {
            m_ProcessTasks[index] = task;
            task.OnStart();
        }

        private void StartTask(int index, IExecuteTask task)
        {
            m_ProcessTasks[index] = task;
            StartCoroutine(ExecuteTask(index, task as IExecuteTask));
        }

        private IEnumerator ExecuteTask(int index, IExecuteTask task)
        {
            task.OnStart();
            yield return task.OnExecute();
            m_ProcessTasks[index] = null;
        }

        private void Update()
        {
            // marktodo
            float dt = 0;
            for (int i = 0; i < m_ProcessTasks.Length; i++)
            {
                var task = m_ProcessTasks[i];
                if(task == null)
                {
                    TryStartTask(i);
                }
                else
                {
                    var updateTask = task as IUpdateTask;
                    if(updateTask != null)
                    {
                        if(updateTask.finish)
                        {
                            m_ProcessTasks[i] = null;
                        }
                        else
                        {
                            updateTask.OnUpdate(dt);
                        }
                    }
                }
            }
        }
    }
}
