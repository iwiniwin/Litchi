namespace Litchi
{
    public interface ISingleton
    {
        /// <summary>
        /// 单例初始化
        /// </summary>
        void OnSingletonInit();
    }


    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        public static T instance
        {
            get
            {
                return SingletonProperty<T>.instance;
            }
        }

        public virtual void OnSingletonInit()
        {

        }
        
        /// <summary>
        /// 资源释放
        /// </summary>
        public static void Dispose()
        {
            SingletonProperty<T>.Dispose();
        }
    }

    public static class SingletonProperty<T> where T : class, ISingleton, new()
    {
        private static T m_Instance;

        private static readonly object m_Lock = new object();

        public static T instance
        {
            get
            {
                lock(m_Lock)
                {
                    if(m_Instance == null)
                    {
                        m_Instance = new T();
                        m_Instance.OnSingletonInit();
                    }
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public static void Dispose()
        {
            m_Instance = null;
        }
    }
}