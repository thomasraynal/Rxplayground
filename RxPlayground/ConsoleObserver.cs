using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public class ConsoleObserver<T> : IObserver<T>
    {
        public void OnCompleted()
        {
            Log.Information("COMPLETED");
        }

        public void OnError(Exception error)
        {
            Log.Error(error,string.Empty);
        }

        public void OnNext(T value)
        {
            Log.Information(String.Format("NEXT- {0}", value));
        }
    }
}
