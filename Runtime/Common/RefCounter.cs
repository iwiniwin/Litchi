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
            refCount ++;
        }

        public void Release()
        {
            refCount --;
        }
    }
}