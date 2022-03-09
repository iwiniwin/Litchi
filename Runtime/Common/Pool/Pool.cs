using System.Collections.Generic;

namespace Litchi
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    internal interface IPool<T>
    {
        /// <summary>
        /// 分配对象
        /// </summary>
        T Allocate();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">待回收对象</param>
        /// <returns>是否回收成功</returns>
        bool Recycle(T obj);
    }
    
    public interface IPoolable
    {
        void OnRecycled();
        bool isRecycled{ get; set; }
    }

    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Pool<T> : IPool<T>
    {
        protected readonly Stack<T> m_CacheStack = new Stack<T>();

        protected IObjectFactory<T> m_Factory;

        /// <summary>
        /// 池内对象数量
        /// </summary>
        /// <value></value>
        public int curCount 
        {
            get { return m_CacheStack.Count; }
        }

        protected int m_MaxCount = 12;

        public virtual T Allocate()
        {
            return m_CacheStack.Count == 0 ? m_Factory.Create() : m_CacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }

    /// <summary>
    /// 对象工厂接口
    /// </summary>
    public interface IObjectFactory<T>
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        T Create();
    }

    /// <summary>
    /// 默认对象工厂类
    /// </summary>
    internal class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }
}