using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBenchmark
{
    class Round
    {
        public ExampleRuner ExampleRuner { get; set; }

        public int Concurrent { get; set; }

        public int Seconds { get; set; }

        public void Stop()
        {
            ExampleRuner?.Stop();
        }
    }
}
