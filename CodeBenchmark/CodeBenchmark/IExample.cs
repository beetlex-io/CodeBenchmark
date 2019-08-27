using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeBenchmark
{
    public interface IExample:IDisposable
    {

        void Initialize(Benchmark benchmark);

        Task Execute();
    }
}
