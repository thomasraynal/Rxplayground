using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public static class ObservableExtensions
    {
        public static IDisposable LogToConsole<T>(this IObservable<T> obs)
        {
            return obs.Subscribe(new ConsoleObserver<T>());
        }

    }
}
