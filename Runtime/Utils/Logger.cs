using UnityEngine;

namespace Litchi
{
    public class Logger
    {
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}