using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Litchi
{
    public enum LogLevel 
    {
        Error = 0,
        Warning,
        Info,
    }

    public partial class Logger
    {
        public static LogLevel logLevel = LogLevel.Info;

        public static void Info(object message)
        {
            if(!CheckLogLevel(LogLevel.Info)) return;
            Debug.Log(message);
        }

        public static void Info(object message, Object context)
        {
            if(!CheckLogLevel(LogLevel.Info)) return;
            Debug.Log(message, context);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Info)) return;
            Debug.LogFormat(format, args);
        }

        public static void InfoFormat(Object context, string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Info)) return;
            Debug.LogFormat(context, format, args);
        }

        public static void Warning(object message)
        {
            if(!CheckLogLevel(LogLevel.Warning)) return;
            Debug.LogWarning(message);
        }

        public static void Warning(object message, Object context)
        {
            if(!CheckLogLevel(LogLevel.Warning)) return;
            Debug.LogWarning(message, context);
        }

        public static void WarningFormat(string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Warning)) return;
            Debug.LogWarningFormat(format, args);
        }

        public static void WarningFormat(Object context, string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Warning)) return;
            Debug.LogWarningFormat(context, format, args);
        }

        public static void Error(object message)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.LogError(message);
        }

        public static void Error(object message, Object context)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.LogError(message, context);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Warning)) return;
            Debug.LogErrorFormat(format, args);
        }

        public static void ErrorFormat(Object context, string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Warning)) return;
            Debug.LogErrorFormat(context, format, args);
        }

        public static void Exception(Exception exception)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.LogException(exception);
        }

        public static void Exception(Exception exception, Object context)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.LogException(exception, context);
        }

        public static void Assert(bool condition)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.Assert(condition);
        }

        public static void Assert(bool condition, Object context)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.Assert(condition, context);
        }

        public static void Assert(bool condition, object message)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.Assert(condition, message);
        }

        public static void Assert(bool condition, string message)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.Assert(condition, message);
        }

        public static void Assert(bool condition, object message, Object context)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.Assert(condition, message, context);
        }

        public static void Assert(bool condition, string message, Object context)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.Assert(condition, message, context);
        }

        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.AssertFormat(condition, format, args);
        }

        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
        {
            if(!CheckLogLevel(LogLevel.Error)) return;
            Debug.AssertFormat(condition, context, format, args);
        }

        private static bool CheckLogLevel(LogLevel level)
        {
            if(logLevel >= level)
            {
                return true;
            }
            return false;
        }
    }
}