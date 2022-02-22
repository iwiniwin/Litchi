using UnityEngine;

namespace Litchi
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        protected static T m_Instance;

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

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            m_Instance = null;
        }

        public virtual void OnSingletonInit()
        {

        }
    }
}