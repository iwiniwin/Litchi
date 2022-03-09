using System;

namespace Litchi
{
    // todomark SafeObjectPool 支持线程安全的pool，替换Logger里的m_DataPool

    public class ObjectPool<T> : Pool<T>, ISingleton where T : IPoolable, new()
    {
        void ISingleton.OnSingletonInit()
        {

        }

        public ObjectPool()
        {
            m_Factory = new DefaultObjectFactory<T>();
        }

        public static ObjectPool<T> instance
        {
            get
            {
                return SingletonProperty<ObjectPool<T>>.instance;
            }
        }

        public void Dispose()
        {
            SingletonProperty<ObjectPool<T>>.Dispose();
        }

        public void Init(int maxCount, int initCount)
        {
            maxCacheCount = maxCount;
            if(maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if(curCount < initCount)
            {
                for (int i = curCount; i < initCount; i++)
                {
                    Recycle(Allocate());
                }
            }
        }

        public int maxCacheCount
        {
            get { return m_MaxCount; }
            set 
            {
                m_MaxCount = value;
                if(m_CacheStack != null)
                {
                    if(m_MaxCount > 0)
                    {
                        if(m_MaxCount < m_CacheStack.Count)
                        {
                            int removeCount = m_CacheStack.Count - m_MaxCount;
                            while(removeCount > 0)
                            {
                                m_CacheStack.Pop();
                                -- removeCount;
                            }
                        }
                    }
                }
            }
        }

        public override T Allocate()
        {
            var result = base.Allocate();
            result.isRecycled = false;
            return result;
        }

        public override bool Recycle(T obj)
        {
            if(obj == null || obj.isRecycled)
            {
                return false;
            }

            if(m_MaxCount > 0)
            {
                if(m_CacheStack.Count >= m_MaxCount)
                {
                    obj.OnRecycled();
                    return false;
                }
            }

            obj.isRecycled = true;
            obj.OnRecycled();
            m_CacheStack.Push(obj);
            return true;
        }
    }
}