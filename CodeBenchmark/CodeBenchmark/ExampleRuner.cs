using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BeetleX;
namespace CodeBenchmark
{
    class ExampleRuner
    {
        public ExampleRuner(ExampleInfo exampleInfo, int concurrent, Benchmark benchmark)

        {
            mBenchmark = benchmark;
            ExampleInfo = exampleInfo;
            mConcurrent = concurrent;
        }

        public int Time { get; set; }

        public int RunTime { get; set; }

        public ExampleInfo ExampleInfo { get; set; }

        private int mConcurrent;

        private List<IExample> examples = new List<IExample>();

        private Statistics mSuccess = new Statistics("Success");

        private Statistics mError = new Statistics("Error");

        private bool mOnRuning = false;

        private Benchmark mBenchmark;

        public Statistics Success => mSuccess;

        public Statistics Error => mError;

        public Exception Exception { get; set; }

        public Status Status { get; set; }

        public bool Initialize(Benchmark benchmark)
        {
            mBenchmark = benchmark;
            try
            {
                for (int i = 0; i < mConcurrent; i++)
                {
                    var item = ExampleInfo.Create(benchmark);
                    examples.Add(item);
                }
                benchmark.AddLog(BeetleX.EventArgs.LogType.Info, $"{ExampleInfo.Example.Name} initialize success");
                return true;
            }
            catch (Exception e_)
            {
                benchmark.AddLog(BeetleX.EventArgs.LogType.Error, $"{ExampleInfo.Example.Name} initialize error {e_.Message}@{e_.StackTrace}");
                Exception = e_;
                Status = Status.Error;
                return false;
            }
        }

        public void Start()
        {
            mOnRuning = true;
            Status = Status.Runing;
            mBenchmark.AddLog(BeetleX.EventArgs.LogType.Info, $"{ExampleInfo.Example.Name} starting");
            foreach (var item in examples)
            {
                Task.Run(() => OnRunExamples(item));
            }
        }

        public void Completed()
        {

            Status = Status.Completed;
            mOnRuning = false;
            mBenchmark.AddLog(BeetleX.EventArgs.LogType.Info, $"{ExampleInfo.Example.Name} completed");
        }

        public void Prepare()
        {
            Status = Status.Preparation;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var task = examples[i % examples.Count].Execute();
                    task.Wait();
                }
                catch (Exception e_)
                {
                    mBenchmark.AddLog(BeetleX.EventArgs.LogType.Error, $"{ExampleInfo.Example.Name} preparing error {e_.Message}@{e_.StackTrace}");
                    return;
                }
            }
            mBenchmark.AddLog(BeetleX.EventArgs.LogType.Info, $"{ExampleInfo.Example.Name} prepare success");
        }

        private async void OnRunExamples(IExample example)
        {
            while (mOnRuning)
            {
                var start = TimeWatch.GetElapsedMilliseconds();
                try
                {
                    await example.Execute();
                    mSuccess.Add(TimeWatch.GetElapsedMilliseconds() - start);
                }
                catch (Exception e_)
                {
                    mError.Add(TimeWatch.GetElapsedMilliseconds() - start);
                    mBenchmark.AddLog(BeetleX.EventArgs.LogType.Error, $"{e_.Message}@{e_.StackTrace}");
                }
            }
        }

        public void Stop()
        {
            if (Status == Status.Runing || Status == Status.None)
            {
                foreach (var item in examples)
                {
                    item.Dispose();
                }
                Status = Status.Stop;
                mOnRuning = false;
                mBenchmark.AddLog(BeetleX.EventArgs.LogType.Warring, $"{ExampleInfo.Example.Name} stoped");
            }
        }

        public void RefreshStati()
        {
            Success.GetData();
            Error.GetData();
        }
    }


}
