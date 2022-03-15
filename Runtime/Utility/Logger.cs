using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using Litchi.IO;

namespace Litchi
{
    public partial class Logger : MonoSingleton<Logger>
    {
        public class LogData
        {
            public string content;
            public string trace;
            public LogType type;
            public uint frame;
            public float time;
        }

        private string m_LogDirectory = "";
        private string m_LogFileName = "";
        private string m_LogFilePath;
        private StreamWriter m_StreamWriter;
        private ManualResetEvent m_ResetEvent;
        private Thread m_WriteThread;
        private ConcurrentQueue<LogData> m_DataQueue;
        private ConcurrentQueue<LogData> m_DataPool;
        private StringBuilder m_StringBuilder;
        private DateTime m_StartDateTime;
        private float m_StartUnscaledTime;
        public bool enableLog {get; set;}
        private volatile bool m_EnableWrite;

        private static char[] m_LogTypeNames = new char[]
        {
            'E',
            'A',
            'W',
            'L',
            'E'
        };

        public void Init()
        {
            enableLog = true;
            m_EnableWrite = true;
            m_StartDateTime = DateTime.Now;
#if UNITY_EDITOR
            m_LogDirectory = PathUtility.Combine(Application.dataPath.Replace("/Assets", ""), "Logs");
#else
            m_LogDirectory = PathUtility.Combine(Application.persistentDataPath, "Logs");
#endif
#if LOG_ENCRYPT
            m_LogFileName = string.Format("{0}E.log", m_StartDateTime.ToString("yyyy_MM_dd_HH_mm_ss"));
#else
            m_LogFileName = string.Format("{0}.log", m_StartDateTime.ToString("yyyy_MM_dd_HH_mm_ss"));
#endif
            m_LogFilePath = PathUtility.Combine(m_LogDirectory, m_LogFileName);
            if(!VFileSystem.Exists(m_LogDirectory))
            {
                Directory.CreateDirectory(m_LogDirectory);
            }
            else
            {
                // todomark 移除超出最大数量的文件
            }
            m_StartUnscaledTime = Time.unscaledTime;
            m_StreamWriter = new StreamWriter(m_LogFilePath);
            m_StringBuilder = new StringBuilder(1024 * 10);
            m_ResetEvent = new ManualResetEvent(false);
            m_DataQueue = new ConcurrentQueue<LogData>();
            m_DataPool = new ConcurrentQueue<LogData>();

            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;

            m_WriteThread = new Thread(DoWrite);
            m_WriteThread.Name = typeof(Logger).FullName;
            m_WriteThread.Start();
        }

        private void OnLogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            // todo 根据宏判断是否所有类型日志都要写入
            GenerateLog(GetLogData(logString, stackTrace, type, 0));  // todomark
        }

        private int m_WriteCountSinceLastFlush = 0;
        private const int kMaxWriteCountSingleFlush = 50;
        private void DoWrite()
        {
            while(m_EnableWrite)
            {
                m_ResetEvent.WaitOne();
                if(m_StreamWriter == null)
                {
                    break;
                }
                LogData data;
                if(m_DataQueue.Count > 0 && m_DataQueue.TryDequeue(out data))
                {
                    m_StringBuilder.Clear();
                    if(!string.IsNullOrEmpty(data.trace) && (data.type == LogType.Assert || data.type == LogType.Error || data.type == LogType.Exception))
                    {
                        m_StringBuilder.AppendFormat("[{0}][{1}][{2}]{3}\r\n{4}\r\n", GetTimeStamp(data.time), m_LogTypeNames[(int)data.type], data.frame, data.content, data.trace);
                    }
                    else
                    {
                        m_StringBuilder.AppendFormat("[{0}][{1}][{2}]{3}\r\n", GetTimeStamp(data.time), m_LogTypeNames[(int)data.type], data.frame, data.content);
                    }

                    // todomark 是否加密
                    m_StreamWriter.Write(m_StringBuilder);
                    if(m_WriteCountSinceLastFlush >= kMaxWriteCountSingleFlush || data.type == LogType.Assert || data.type == LogType.Error || data.type == LogType.Exception)
                    {
                        m_StreamWriter.Flush();
                        m_WriteCountSinceLastFlush = 0;
                    }
                    m_WriteCountSinceLastFlush ++;
                }
                m_ResetEvent.Reset();
                // 达到单个日志上限，不再打印日志
            }
        }

        private string GetTimeStamp(float time)
        {
            DateTime dt = m_StartDateTime.AddSeconds(time - m_StartUnscaledTime);
            return string.Format("{0}-{1}:{2}:{3}.{4}", dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }

        private void GenerateLog(LogData data)
        {
            if(!enableLog || !m_EnableWrite)
            {
                return;
            }
            m_DataQueue.Enqueue(data);
            m_ResetEvent.Set();
        }

        private LogData GetLogData(string logString, string stackTrace, LogType type, uint frame)
        {
            LogData data;
            if(!m_DataPool.TryDequeue(out data))
            {
                data = new LogData();
            }

            data.content = logString;
            data.trace = stackTrace;
            data.type = type;
            data.frame = frame;
            data.time = Time.unscaledDeltaTime;
            return data;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
            m_EnableWrite = false;
            if(m_ResetEvent != null)
            {
                m_ResetEvent.Set();
            }
            Thread.Sleep(1);
            if(m_StreamWriter != null)
            {
                m_StreamWriter.Flush();
                m_StreamWriter.Close();
                m_StreamWriter = null;
            }
            if(m_WriteThread != null)
            {
                m_WriteThread.Join(10);
                m_WriteThread = null;
            }
        }
    }
}