using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public class Event
    {
        public delegate void EventHandler(string one, int two);
        public event EventHandler ThisIsAnEvent;
        public IObservable<Tuple<string, int>> SelfObservable { get; private set; }

        public void Raise()
        {
            ThisIsAnEvent.Invoke("one", 2);
        }

        public Event()
        {
            SelfObservable = Observable.FromEvent<EventHandler, Tuple<string, int>>(args => (str, @int) => args(Tuple.Create(str, @int)),
                handler => ThisIsAnEvent += handler,
                handler => ThisIsAnEvent -= handler);

            ThisIsAnEvent += (arg1, arg2) =>
            {
                Log.Information("Event from Raise Event");
            };

       
        }
    }
}
