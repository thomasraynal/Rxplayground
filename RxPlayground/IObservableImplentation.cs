using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public class IObservableImplentation : IObservable<int>
    {
        private const int _upTo = 10;

        public IDisposable Subscribe(IObserver<int> observer)
        {
            Enumerable.Range(0, _upTo).ForEach((item) =>  observer.OnNext(item));

            observer.OnCompleted();

            return Disposable.Empty;
        }
    }
}
