using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
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

        public static void ReplaySubject()
        {
            var cleanUp = new CompositeDisposable();

            var subject = new ReplaySubject<int>(20);

            cleanUp.Add(subject.LogToConsole());

            for (var i = 0; i < 25; i++) { subject.OnNext(i); }

            cleanUp.Add(subject.LogToConsole());

            cleanUp.Dispose();
        }


        public static void Publish()
        {
            var stream = Observable.Create<int>((obs) =>
            {
                for (var i = 0; i < 3; i++) obs.OnNext(i);

                obs.OnCompleted();

                return Disposable.Empty;

            });


            var cold = stream.Publish();


            var cleanUp = new CompositeDisposable
            {
                cold.LogToConsole(),
                cold.LogToConsole()
            };

            cold.Connect();

            Thread.Sleep(1000);

            cleanUp.Add(cold.LogToConsole());

            var cold2 = stream.PublishLast();

            cleanUp.Add(cold2.LogToConsole());
            cleanUp.Add(cold2.LogToConsole());

            cold2.Connect();

            Thread.Sleep(1000);

            cleanUp.Add(cold2.LogToConsole());

            cleanUp.Dispose();
        }


        public static void RefCount()
        {
            var stream = Observable
                .Interval(TimeSpan.FromMilliseconds(200))
                .Do(time => Log.Information("FROM obs {0}", time));


            var cold = stream
                .Publish()
                .RefCount();

            var cleanUp = new CompositeDisposable
            {
                cold.LogToConsole(),
                cold.LogToConsole()
            };

            Thread.Sleep(1000);
            cleanUp.Dispose();


            var lastObs = cold.LogToConsole();

            Thread.Sleep(1000);

            lastObs.Dispose();
        }


        public static void Agregate()
        {
            var stream = Observable.Create<int>((obs) =>
            {
                for (var i = 0; i < 10; i++) obs.OnNext(i);

                obs.OnCompleted();

                return Disposable.Empty;

            });


            var cleanUp = new CompositeDisposable
            {
            stream
                .Aggregate((x, y) => x + y)
                .LogToConsole(),

            stream
                .Scan((x, y) => x + y)
                .LogToConsole(),
            };

            cleanUp.Dispose();
        }

        public static void Combine()
        {
            var stream = Observable.Create<int>((obs) =>
            {
                for (var i = 0; i < 10; i++) obs.OnNext(i);

                obs.OnCompleted();

                return Disposable.Empty;

            });

            var stream2 = Observable.Create<int>((obs) =>
            {
                for (var i = 0; i < 10; i++) obs.OnNext(i);

                obs.OnCompleted();

                return Disposable.Empty;

            });

            var taskStream = Observable.Create<Task<int>>((obs) =>
            {
                var rand = new Random();

                Enumerable.Range(0, 10)
                          .Select(val => Task.Delay(TimeSpan.FromSeconds(rand.Next(0, 1))).ContinueWith(tsk => val))
                          .ForEach(task => obs.OnNext(task));

                obs.OnCompleted();

                return Disposable.Empty;

            });

            var cleanUp = new CompositeDisposable
            {

            stream
                .Zip(stream2,(x, y) => x + y)
                .LogToConsole()
            };

            cleanUp.Add(stream
                .CombineLatest(stream2, (x, y) => x + y)
                .LogToConsole());

            taskStream
                 .Switch()
                 .LogToConsole();


            cleanUp.Dispose();

        }

        public static void Buffer()
        {
     
            var stream = Observable.Create<int>((obs) =>
            {
                for (var i = 0; i < 10; i++) obs.OnNext(i);

                obs.OnCompleted();

                return Disposable.Empty;

            });

            stream
                 .Buffer(2, 1)
                 .Select(buffer => buffer.Sum())
                 .LogToConsole();
        }

        public static void ErrorHandling()
        {
            var obsError = Observable.Throw<String>(new Exception("failed"));
            var obs = Observable.Create<String>((observer) =>
            {
                observer.OnNext("a");
                observer.OnNext("b");
                observer.OnNext("c");
                observer.OnNext("d");
                observer.OnCompleted();
                return Disposable.Empty;
            });

            var obsError2 = Observable.Create<String>((observer) =>
            {
                observer.OnNext("RETRY");
                observer.OnError(new Exception("failed"));
                return Disposable.Empty;
            });

            obsError
                .OnErrorResumeNext(obs)
                .LogToConsole();

            obsError2
                .Retry(3)
                .Finally(() =>
                {
                    Log.Logger.Information("FINALLY");
                })
                .LogToConsole();
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();


            //CurrentThreadScheduler
            //NewThreadScheduler
            //ThreadPoolScheduler
            //TaskPoolScheduler
            //ImmediateScheduler
            //EventLoopScheduler
            //DispatcherScheduler

            ErrorHandling();


            Console.Read();


        }
    }
}
