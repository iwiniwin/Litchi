namespace Litchi
{
    public interface IRefCounter
    {
        int refCount {get;}
        void Retain();
        void Release();
    }

    public class RefCounter : IRefCounter
    {
        public int refCount {get; protected set;}
        public void Retain()
        {
            ++ refCount;
        }

        public void Release()
        {
            -- refCount;
            if(refCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {

        }

        public bool IsZeroRef()
        {
            return refCount == 0;
        }

        public void MergeRefCount(IRefCounter counter)
        {
            refCount += counter.refCount;
        } 
    }
}