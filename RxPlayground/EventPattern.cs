using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;

namespace RxPlayground
{
    public class DaatEventArg : EventArgs
    {
        public long Data { get; set; }
    }

    public class EventPattern
    {
        public delegate void DataStreamHandler(object sender, DaatEventArg data);
        public event DataStreamHandler DataStream;

        public delegate void StopDataStreamHandler(object sender, DaatEventArg data);
        public event StopDataStreamHandler StopDataStream;

        private IDisposable _stream;
        private IObservable<EventPattern<DaatEventArg>> _streamOn;
        private IObservable<EventPattern<DaatEventArg>> _streamOff;

        public IObservable<EventPattern<DaatEventArg>> StreamOn
        {
            get
            {
               return _streamOn;
            }
        }

        public IObservable<EventPattern<DaatEventArg>> StreamOff
        {
            get
            {
                return _streamOff;
            }
        }

        public void Generate()
        {
            _stream = Observable.Interval(TimeSpan.FromMilliseconds(500))
                                .Subscribe(time => DataStream.Invoke(this, new DaatEventArg() { Data = time }));
        }

        public void Stop()
        {
            _stream.Dispose();

            StopDataStream.Invoke(this, new DaatEventArg() { Data = -1 });
        }

        public EventPattern()
        {
            _streamOn = Observable.FromEventPattern<DaatEventArg>(this, "DataStream");
            _streamOff = Observable.FromEventPattern<DaatEventArg>(this, "StopDataStream");

            DataStream += (sender,arg) =>
            {
                Log.Information("Raise Event Stream {0}", arg.Data);
            };

            StopDataStream += (sender, arg) =>
            {
                Log.Information("Stop Event Stream {0}");
            };
        }
    }
}
