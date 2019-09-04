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

        public ExampleRpsDetail GetRpsDetail()
        {
            var result = new ExampleRpsDetail();
            result.Category = Category;
            result.Name = Name;
            return result;
        }
    }

    class ExampleRpsDetail
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public long Best { get; set; }

        public long Count { get; set; }

        public long Errors { get; set; }

        public string Percent { get; set; }

        public List<RpsItem> Items = new List<RpsItem>();

        public void Add(long count, int Concurrent,int value,long error)
        {
            Count += count;
            Errors += error;
            if (value > Best)
                Best = value;
            Items.Add(new RpsItem { Concurrent= Concurrent, Value= value });
        }

        public class RpsItem
        {
            public int Concurrent { get; set; }

            public int Value { get; set; }
        }
    }

}
