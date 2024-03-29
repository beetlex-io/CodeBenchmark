﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBenchmark
{
    class ReportItem
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public long Value { get; set; }

        public  long Error { get; set; }

        public double Percent { get; set; }

        public ReportItem Rps { get; set; }
    }
}
