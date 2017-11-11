using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public class Remote : IObservable<Timestamped<long>>, IDisposable
    {
        private IDisposable _cleanUp;

        private List<IObserver<Timestamped<long>>> _observers;

        public Remote()
        {
            _observers = new List<IObserver<Timestamped<long>>>();

            _cleanUp = Observable.Defer(() =>
                Observable.Interval(TimeSpan.FromSeconds(1)))
                .Timestamp()
                .Subscribe(time =>
                {
                    _observers.ForEach((observer) => observer.OnNext(time));
                });
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        public IDisposable Subscribe(IObserver<Timestamped<long>> observer)
        {
            _observers.Add(observer);

            return Disposable.Create(() =>
            {
                _observers.Remove(observer);
            });
        }
    }
}
