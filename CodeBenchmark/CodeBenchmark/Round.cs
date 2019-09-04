using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBenchmark
{

    class Round
    {
        public string ID { get; set; }

        public int Concurrent { get; set; }

        public int Seconds { get; set; }

        public string Status { get; set; } = "None";

        public object[] Items { get; set; } = new object[0];

        public object[] Report { get; set; } = new object[0];

        public object[] Latency = new object[0];

        public object SelectItem { get; set; } = new object();

        public bool Show { get; set; } = true;

        public bool ShowItems { get; set; } = false;
    }


    class RoundRuner
    {

        public string ID { get; set; }

        public int Concurrent { get; set; }

        public int Seconds { get; set; }

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

        public Status Status { get; set; } = Status.None;

        public IList<ExampleRuner> Examples { get; private set; } = new List<ExampleRuner>();

        private DateTime? mEndTime;

        private System.Threading.Timer mTimer;

        private ExampleRuner mCurrent;

        private int mRunTime;

        public int Index { get; set; }

        public int RunTime => mRunTime;

        private Benchmark mBenchmark;

        private Queue<ExampleRuner> mExamplesQueue = new Queue<ExampleRuner>();


        public object GetLatency(string id)
        {
            return Examples.FirstOrDefault(e => e.ExampleInfo.ID == id)?.GetLatency();
        }

        public void Start(Benchmark benchmark, params ExampleInfo[] items)
        {
            Status = Status.Runing;
            mBenchmark = benchmark;
            if (mTimer != null)
                mTimer.Dispose();
            foreach (var item in Examples)
                item.Stop();
            Examples.Clear();
            mExamplesQueue.Clear();
            foreach (var item in items)
            {
                var runer = new ExampleRuner(item, Concurrent, mBenchmark);
                runer.Time = Seconds;
                mExamplesQueue.Enqueue(runer);
                Examples.Add(runer);
            }
            Task.Run(() => OnRunExample(mExamplesQueue.Dequeue()));

        }

        private void OnRunExample(ExampleRuner example)
        {
            System.Threading.Thread.Sleep(1000);
            mCurrent = example;
            if (mCurrent.Initialize(mBenchmark))
            {
                mCurrent.Prepare();
                mCurrent.Start();
                mEndTime = DateTime.Now.AddSeconds(this.Seconds);
                mRunTime = 0;
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
                    mTimer.Dispose();
                }
            }
        }

        public Action<RoundRuner> Completed { get; set; }

        private void OnTimer(object state)
        {
            mRunTime++;
            if (mRunTime > 0 && mRunTime % 10 == 0)
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
                        Completed?.Invoke(this);
                    }
                }
            }
        }

        public void Output(ExampleRpsDetail detail)
        {
            var item = Examples.FirstOrDefault(p => p.ExampleInfo.Name == detail.Name && p.ExampleInfo.Category == detail.Category);
            if(item !=null)
            {
                detail.Add(item.Success.Count,Concurrent, item.Success.AvgRps, item.Error.Count);
            }
        }

        public object GetRetportGroupByCategories()
        {
            return from a in Examples
                   group a by a.ExampleInfo.Category into g
                   select new { Category = g.Key, Items = Report(g.Key) };
        }

        public object GetStatus()
        {
            var examples = (from a in Examples
                            select new
                            {
                                a.ExampleInfo.ID,
                                a.ExampleInfo.Name,
                                a.ExampleInfo.Description,
                                a.ExampleInfo.Category,
                                a.Exception?.Message,
                                a.Time,
                                a.RunTime,
                                a.Success.MaxRps,
                                a.Success.AvgRps,
                                Errors = a.Error.Count,
                                Success = a.Success.Count,
                                Status = a.Status.ToString(),
                                TimePercent = a.Status == Status.Completed ? 100 : (int)(((double)a.RunTime / (double)a.Time) * 100),
                            }).ToArray();

            return examples;
        }



        public object[] Report(string category = null)
        {
            if (Examples.Count == 0)
                return new object[0];
            if (Status == Status.Stop || Status == Status.Completed)
            {
                var total = (from a in Examples
                             orderby a.Success.Count descending
                             select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.Count, ID = a.ExampleInfo.ID, Error = a.Error.Count }).ToArray();
                var rps = (from a in Examples
                           orderby a.Success.AvgRps descending
                           select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.AvgRps, ID = a.ExampleInfo.ID, Error = a.Error.Count }).ToArray();

                if (category != null)
                {
                    total = (from a in total where a.Category == category orderby a.Value descending select a).ToArray();
                    rps = (from a in rps where a.Category == category orderby a.Value descending select a).ToArray();
                }

                var totalMax = total.Max(m => m.Value);

                foreach (var item in total)
                    if (item.Value > 0 && totalMax > 0)
                        item.Percent = (int)(((double)item.Value / (double)totalMax) * 10000) / 100d;


                var rpsMax = rps.Max(m => m.Value);

                foreach (var item in rps)
                {
                    if (item.Value > 0 && rpsMax > 0)
                        item.Percent = (int)(((double)item.Value / (double)rpsMax) * 10000) / 100d;
                }

                foreach (var item in total)
                    item.Rps = rps.FirstOrDefault(p => p.ID == item.ID);


                return total;
            }
            else
            {
                return new object[0];
            }
        }
    }
}
