using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeBenchmark
{
    class LoadRuner
    {

        public LoadRuner(Benchmark benchmark)
        {
            mBenchmark = benchmark;
        }

        private Benchmark mBenchmark;

        public IList<ExampleRuner> Examples { get; private set; } = new List<ExampleRuner>();

        public Status Status { get; set; }

        private DateTime? mEndTime;

        private System.Threading.Timer mTimer;

        private ExampleRuner mCurrent;

        private int mSeconds;

        private int mTimeCount;

        private Queue<ExampleRuner> mExamplesQueue = new Queue<ExampleRuner>();

        public void Start(int concurrent, int seconds, params ExampleInfo[] items)
        {
            if (mTimer != null)
                mTimer.Dispose();
            foreach (var item in Examples)
                item.Stop();
            Examples.Clear();
            mExamplesQueue.Clear();
            Status = Status.Runing;
            mSeconds = seconds;
            foreach(var item in items)
            {
                var runer = new ExampleRuner(item, concurrent, mBenchmark);
                runer.Time = seconds;
                mExamplesQueue.Enqueue(runer);
                Examples.Add(runer);
            }     
            Task.Run(()=> OnRunExample(mExamplesQueue.Dequeue()));
            
        }


        private void OnRunExample(ExampleRuner example)
        {         
            mCurrent = example;
            if (mCurrent.Initialize(mBenchmark))
            {
                mCurrent.Prepare();
                mCurrent.Start();
                mEndTime = DateTime.Now.AddSeconds(mSeconds);
                mTimeCount = 0;
                mTimer = new System.Threading.Timer(OnTimer, null, 96, 96);
            }
            else
            {
                if (mExamplesQueue.Count > 0)
                {
                    OnRunExample(mExamplesQueue.Dequeue());
                }
                else
                {
                    Status = Status.Completed;
                }
            }
        }

        private void OnTimer(object state)
        {
            mTimeCount++;
            if (mTimeCount > 0 && mTimeCount % 10 == 0)
            {
                if (DateTime.Now < mEndTime)
                {
                    mCurrent.RefreshStati();
                    mCurrent.RunTime++;
                }
                else
                {
                    mTimer.Dispose();
                    mCurrent.Completed();
                    if (mExamplesQueue.Count > 0)
                    {
                        OnRunExample(mExamplesQueue.Dequeue());
                    }
                    else
                    {
                        Status = Status.Completed;
                    }
                }
            }
        }

        public void Stop()
        {
            Status = Status.Stop;
            foreach (var item in Examples)
                item.Stop();
            if (mTimer != null)
            {
                mTimer.Dispose();
                mTimer = null;
            }
        }
    }
}
