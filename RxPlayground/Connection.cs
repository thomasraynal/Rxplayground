using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public class Connection : ObservableBase<Timestamped<long>>
    {
        private Remote _remote;

        public Connection (Remote remote)
        {
            _remote = remote;
        }

        protected override IDisposable SubscribeCore(IObserver<Timestamped<long>> observer)
        {
            return _remote.Subscribe(observer);
        }
    }
}
