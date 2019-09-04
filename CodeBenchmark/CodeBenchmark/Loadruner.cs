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

        private ExampleInfo[] mExampleInfos;

        public Status Status { get; set; }

        private int CurrentIndex { get; set; }

        private RoundRuner mCurrentRound;

        public List<Round> RoundCases { get; set; } = new List<Round>();

        public ExampleInfo[] ExampleInfos => mExampleInfos;

        public RoundRuner CurrentRound => mCurrentRound;

        public List<RoundRuner> Rounds { get; private set; } = new List<RoundRuner>();

        public void Start(List<Round> rounds, params ExampleInfo[] items)
        {
            mExampleInfos = items;
            CurrentIndex = 0;
            foreach (var item in Rounds)
                item.Stop();
            Rounds.Clear();
            int i = 1;
            foreach (var r in rounds)
            {
                RoundRuner roundRuner = new RoundRuner();
                roundRuner.Index = i;
                roundRuner.ID = r.ID;
                roundRuner.Completed = OnRoundCompleted;
                roundRuner.Seconds = r.Seconds;
                roundRuner.Concurrent = r.Concurrent;
                Rounds.Add(roundRuner);
                i++;
            }
            Task.Run(() => OnRun());
            Status = Status.Runing;
        }

        private void OnRun()
        {

            mCurrentRound = Rounds[CurrentIndex];
           
            mCurrentRound.Start(mBenchmark, mExampleInfos);
        }

        private void OnRoundCompleted(RoundRuner e)
        {
            CurrentIndex++;
            if(CurrentIndex >= Rounds.Count)
            {
                Status = Status.Completed;
            }
            else
            {
                System.Threading.Thread.Sleep(1000);
                OnRun();
            }
        }

        public void Stop()
        {
            Status = Status.Stop;
            foreach (var item in Rounds)
                item.Stop();
        }
    }
}
