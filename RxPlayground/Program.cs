using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RxPlayground
{
    class Program
    {
        enum State
        {
            INACTIVE,
            ACTIVE,
            ERROR
        };

        private static void ObservableImplentation()
        {
            var obs = new IObservableImplentation();
            obs.LogToConsole();
        }

        private static void Connection()
        {
            var remote = new Remote();
            var connection1 = new Connection(remote);
            var connection2 = new Connection(remote);

            var cleanUp = new CompositeDisposable
            {
                connection1.LogToConsole()
            };

            Thread.Sleep(3000);

            cleanUp.Add(connection2.LogToConsole());

            Thread.Sleep(3000);

            cleanUp.Dispose();
        }

        private static void Event()
        {

            var ev = new Event();

            var disp = ev.SelfObservable.Subscribe(arg =>
            {
                Log.Information("Event from FromEvent");
            });

            ev.Raise();

            Thread.Sleep(500);

            disp.Dispose();

        }

        private static void Text()
        {
            Observable.Using(
                () => File.OpenText("Lorem.txt"),
                stream => Observable.Generate(
                    stream,
                    s => !s.EndOfStream,
                    s => s, s => s.ReadLine()))
                    .LogToConsole();
        }

        private static void Error()
        {
            var errorObs = Observable.Create<string>((ob) =>
            {
                ob.OnNext("ok");
                throw new Exception("ko");
                return Disposable.Empty;
            });

            errorObs.Subscribe(
                (res) => Log.Information(res),
                (ex) => Log.Error(ex, string.Empty)
            );

        }

        private static void EventPattern()
        {
            var evp = new EventPattern();
            var cleanUp = new CompositeDisposable();

            var streamOn = evp.StreamOn
                                .TakeUntil(evp.StreamOff)
                                .Repeat()
                                .Subscribe(ob => Log.Information("Observable Stream - {0}", ob.EventArgs.Data));

            cleanUp.Add(streamOn);


            evp.Generate();

            Thread.Sleep(4000);

            evp.Stop();

            Thread.Sleep(2000);

            evp.Generate();

            Thread.Sleep(2000);

            evp.Stop();

            cleanUp.Dispose();

        }

        private void BehaviorSubject()
        {
            var subject = new BehaviorSubject<State>(State.INACTIVE);
            subject.LogToConsole();
            subject.OnNext(State.ACTIVE);
            subject.LogToConsole();
        }

        public void ReplaySubject()
        {
            var cleanUp = new CompositeDisposable();

            var subject = new ReplaySubject<int>(20);

            cleanUp.Add(subject.LogToConsole());

            for (var i = 0; i < 25; i++) { subject.OnNext(i); }

            cleanUp.Add(subject.LogToConsole());

            cleanUp.Dispose();
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();



            Console.Read();


        }
    }
}
