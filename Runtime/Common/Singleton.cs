namespace Litchi
{
    public abstract class Singleton<T> where T : new()
    {
        protected static T m_Instance;

        protected static object m_Lock;

        public static T instance
        {
            get
            {
                lock(m_Lock)
                {
                    if(m_Instance == null)
                    {
                        m_Instance = new T();
                    }
                }
                return m_Instance;
            }
        }
    }
}