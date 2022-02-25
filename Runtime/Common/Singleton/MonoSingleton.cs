using UnityEngine;

namespace Litchi
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        public static T instance
        {
            get
            {
                return MonoSingletonProperty<T>.instance;
            }
        }

        public virtual void Dispose()
        {
            MonoSingletonProperty<T>.Dispose(true);
        }

        protected virtual void OnDestroy()
        {
            // 仅释放引用
            MonoSingletonProperty<T>.Dispose(false);
        }

        public virtual void OnSingletonInit()
        {

        }
    }

    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
    {
        private static T m_Instance;

        public static T instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = Object.FindObjectOfType<T>();
                    if(m_Instance != null)
                    {
                        m_Instance.OnSingletonInit();
                    }
                }
                if(m_Instance == null)
                {
                    var obj = new GameObject(typeof(T).Name);
                    Object.DontDestroyOnLoad(obj);
                    m_Instance = obj.AddComponent<T>();
                    m_Instance.OnSingletonInit();
                }
                return m_Instance;
            }
        }

        public static void Dispose(bool destroy = true)
        {
            if(destroy && m_Instance != null)
            {
                Object.Destroy(m_Instance.gameObject);
            }
            m_Instance = null;
        }
    }
}