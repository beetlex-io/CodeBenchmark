using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeetleX.FastHttpApi;

namespace CodeBenchmark
{
    [Controller]
    class Home : BeetleX.FastHttpApi.IController
    {
        private Benchmark benchmark;

        [BeetleX.FastHttpApi.NotAction]
        public void Init(HttpApiServer server, string path)
        {
            benchmark = (Benchmark)server[Benchmark.BENCHMARK_TAG];
        }

        public object ListExamples()
        {
            var result = from a in benchmark.Examples
                         group a by a.Category into g
                         select new
                         {
                             Show = true,
                             g.Key,
                             Selected = false,
                             Example = false,
                             g.ToArray()?[0].Version,
                             Items = from i in g.ToArray() orderby i.Name ascending select new { i.ID, i.Name, i.Description, Example = true, Selected = false }
                         };
            return result;

        }

        public object DelRoundCase(string id)
        {

            benchmark.LoadRuner.RoundCases.RemoveAll(p => p.ID == id);
            return benchmark.LoadRuner.RoundCases;
        }

        public object ClearRoundCase()
        {
            benchmark.LoadRuner.RoundCases.Clear();
            return benchmark.LoadRuner.RoundCases;
        }

        public object AddRoundCase(string id, int concurrent, int secconds)
        {

            benchmark.LoadRuner.RoundCases.Add(new Round
            {
                Concurrent = concurrent,
                ID = id,
                Seconds = secconds
            });
            return benchmark.LoadRuner.RoundCases;
        }

        public object GetRoundStatus(string id)
        {
            var a = benchmark.LoadRuner.Rounds.FirstOrDefault(f => f.ID == id);
            return new
            {
                a.ID,
                a.Concurrent,
                a.Seconds,
                Status = a.Status.ToString(),
                Items = a.GetStatus(),
                Report = a.Report(),
                Latency = new object[0]
            };
        }

        public object GetAllStatus()
        {
            var result = (from a in benchmark.LoadRuner.Rounds
                          where benchmark.LoadRuner.RoundCases.SingleOrDefault(p => p.ID == a.ID) != null
                          select new
                          {
                              a.ID,
                              a.Concurrent,
                              a.Seconds,
                              Status = a.Status.ToString(),
                              Show = true,
                              ShowItems = false,
                              Items = a.GetStatus(),
                              Report = a.Report(),
                              SelectItem = new object(),
                              Latency = new object[0]
                          }).ToArray();
            return result;
        }

        public object GetStatus()
        {
            //var examples = (from a in benchmark.LoadRuner.Examples
            //                select new
            //                {
            //                    a.ExampleInfo.ID,
            //                    a.ExampleInfo.Name,
            //                    a.ExampleInfo.Description,
            //                    a.ExampleInfo.Category,
            //                    a.Exception?.Message,
            //                    a.Time,
            //                    a.RunTime,
            //                    a.Success.MaxRps,
            //                    a.Success.AvgRps,
            //                    Errors = a.Error.Count,
            //                    Success = a.Success.Count,
            //                    Status = a.Status.ToString(),
            //                    TimePercent = a.Status == Status.Completed ? 100 : (int)(((double)a.RunTime / (double)a.Time) * 100),
            //                }).ToArray();

            var result = new
            {
                Status = benchmark.LoadRuner.Status.ToString(),
                Items = benchmark.LoadRuner.CurrentRound == null ? new object[0] : benchmark.LoadRuner.CurrentRound.GetStatus(),
                benchmark.LoadRuner.CurrentRound?.Index,
                RoundStatus = benchmark.LoadRuner.CurrentRound?.Status,
                benchmark.LoadRuner.CurrentRound?.ID,
                AllStatus = (from a in benchmark.LoadRuner.Rounds select new { a.ID, Status = a.Status.ToString() }).ToArray()


            };
            return result;

        }

        [Post]
        public void Run(List<Round> rounds, List<string> examples)
        {
            if (benchmark.LoadRuner.Status == Status.Runing)
            {
                throw new Exception("Have some examples on testing!");
            }
            else
            {
                var items = from a in benchmark.Examples where examples.Contains(a.ID) orderby a.Category select a;
                //benchmark.LoadRuner.Start(concurrent, seconds, items.ToArray());
                benchmark.LoadRuner.Start(rounds, items.ToArray());
            }
        }


        public object GetLatency(string roundid, string id)
        {
            //return benchmark.LoadRuner.Examples.FirstOrDefault(e => e.ExampleInfo.ID == id)?.GetLatency();

            return benchmark.LoadRuner.Rounds.FirstOrDefault(e => e.ID == roundid)?.GetLatency(id);
        }

        public object Report()
        {
            //var total = (from a in benchmark.LoadRuner.Examples
            //             orderby a.Success.Count descending
            //             select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.Count, ID = a.ExampleInfo.ID }).ToArray();
            //var rps = (from a in benchmark.LoadRuner.Examples
            //           orderby a.Success.AvgRps descending
            //           select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.AvgRps, ID = a.ExampleInfo.ID }).ToArray();
            //var totalMax = total.Max(m => m.Value);

            //foreach (var item in total)
            //    if (item.Value > 0 && totalMax > 0)
            //        item.Percent = (int)(((double)item.Value / (double)totalMax) * 10000) / 100d;


            //var rpsMax = rps.Max(m => m.Value);

            //foreach (var item in rps)
            //{
            //    if (item.Value > 0 && rpsMax > 0)
            //        item.Percent = (int)(((double)item.Value / (double)rpsMax) * 10000) / 100d;
            //}

            //foreach (var item in total)
            //    item.Rps = rps.FirstOrDefault(p => p.ID == item.ID);

            //var result = from a in total
            //             group a by a.Category into g
            //             select new { g.Key, Items = g.ToArray().OrderByDescending(p=>p.Value)};

            return benchmark.LoadRuner.CurrentRound.Report();
        }

        public object AllRpsReport()
        {
            var items = (from a in benchmark.LoadRuner.ExampleInfos select a.GetRpsDetail()).ToArray();
            foreach (var item in items)
            {
                foreach (var r in benchmark.LoadRuner.Rounds)
                    r.Output(item);

            }

            //(int)(((double)item.Value / (double)totalMax) * 10000) / 100d;
            var result = (from a in items
                          group a by a.Category into g
                          select new { g.Key, Items = g.ToArray() }).ToArray();
            foreach (var item in result)
            {
                long max = item.Items.Max(p => p.Best);
                foreach (var sitem in item.Items)
                {
                    sitem.Percent = ((int)((double)sitem.Best / (double)max * 10000) / 100d).ToString();
                }

            }
            List<ExampleRpsDetail> rpsDetails = new List<ExampleRpsDetail>();
            foreach (var item in result) {
                rpsDetails.Add(new ExampleRpsDetail { Category = item.Key });
                foreach (var sitem in item.Items.OrderByDescending(p=>p.Best))
                    rpsDetails.Add(sitem);
            }    
            return new { Times = from a in benchmark.LoadRuner.Rounds select a.Concurrent, Items = rpsDetails };
        }

        public void Stop()
        {
            if (benchmark.LoadRuner.Status == Status.Runing)
                benchmark.LoadRuner.Stop();
        }
    }
}
