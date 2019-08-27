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
            var examples = from a in benchmark.LoadRuner.Examples
                           select new
                           {
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


                           };
            return new { Status = benchmark.LoadRuner.Status.ToString(), Items = examples };
        }

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

        public object Report()
        {
            var total = (from a in benchmark.LoadRuner.Examples orderby a.Success.Count descending
                        select new ReportItem { Category=a.ExampleInfo.Category, Name=a.ExampleInfo.Name, Value= a.Success.Count }).ToArray();
            var rps = (from a in benchmark.LoadRuner.Examples orderby a.Success.AvgRps descending
                        select new ReportItem { Category = a.ExampleInfo.Category, Name = a.ExampleInfo.Name, Value = a.Success.AvgRps }).ToArray();
            var totalMax = total.Max(m => m.Value);

            foreach (var item in total)
                item.Percent = (int)(((double)item.Value / (double)totalMax) * 100);
           

            var rpsMax = rps.Max(m => m.Value);

            foreach (var item in rps)
                item.Percent = (int)(((double)item.Value / (double)rpsMax) * 100);

            return new { total, rps };
        }

        public void Stop()
        {
            if (benchmark.LoadRuner.Status == Status.Runing)
                benchmark.LoadRuner.Stop();
        }
    }
}
