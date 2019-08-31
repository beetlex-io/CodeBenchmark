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

        public object GetStatus()
        {
            var examples = (from a in benchmark.LoadRuner.Examples
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

            return new
            {
                Status = benchmark.LoadRuner.Status.ToString(),
                Items = examples
            };
        }

        [Post]
        public void Run(int concurrent, int seconds, List<string> examples)
        {
            if (benchmark.LoadRuner.Status == Status.Runing)
            {
                throw new Exception("Have some examples on testing!");
            }
            else
            {
                var items = from a in benchmark.Examples where examples.Contains(a.ID) select a;
                benchmark.LoadRuner.Start(concurrent, seconds, items.ToArray());
            }
        }


        public object GetLatency(string id)
        {
            return benchmark.LoadRuner.Examples.FirstOrDefault(e => e.ExampleInfo.ID == id)?.GetLatency();
        }

        public object Report()
        {
            var total = (from a in benchmark.LoadRuner.Examples
                         orderby a.Success.Count descending
                         select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.Count, ID = a.ExampleInfo.ID }).ToArray();
            var rps = (from a in benchmark.LoadRuner.Examples
                       orderby a.Success.AvgRps descending
                       select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.AvgRps, ID = a.ExampleInfo.ID }).ToArray();
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

            var result = from a in total
                         group a by a.Category into g
                         select new { g.Key, Items = g.ToArray().OrderByDescending(p=>p.Value)};

            return result;
        }

        public void Stop()
        {
            if (benchmark.LoadRuner.Status == Status.Runing)
                benchmark.LoadRuner.Stop();
        }
    }
}
