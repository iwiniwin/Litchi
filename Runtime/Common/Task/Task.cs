using System;
using System.Collections;

namespace Litchi
{
    public interface ITask
    {
        
    }

    public interface IEnumeratorTask : ITask
    {
        IEnumerator DoAsync(Action onFinish);
    }
}
