using System;
using System.Diagnostics;

namespace CodeBenchmark.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Benchmark benchmark = new Benchmark();
            benchmark.Register(typeof(Program).Assembly);
            benchmark.Start();
            if(Environment.OSVersion.Platform== PlatformID.Win32NT)
                benchmark.OpenWeb();
            Console.Read();
        }
    }
}
