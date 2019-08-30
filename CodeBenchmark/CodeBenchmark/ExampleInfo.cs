using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBenchmark
{
    
    class ExampleInfo
    {

        public string ID { get; set; } = Guid.NewGuid().ToString("N");

        public Type Example { get; internal set; }

        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public string Category { get; internal set; }

        public string Version { get; internal set; }

        public IExample Create(Benchmark benchmark)
        {
            IExample result = (IExample)Activator.CreateInstance(Example);
            return result;
        }
    }
}
