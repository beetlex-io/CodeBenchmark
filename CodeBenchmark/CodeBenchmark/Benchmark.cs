using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
namespace CodeBenchmark
{

    public class Benchmark
    {
        internal const string BENCHMARK_TAG = "BENCHMARK_TAG";

        public Benchmark()
        {
            mHttpApiServer = new BeetleX.FastHttpApi.HttpApiServer();
            mHttpApiServer.Options.LogLevel = BeetleX.EventArgs.LogType.Warring;
            mHttpApiServer.Options.LogToConsole = true;
            mHttpApiServer.Options.WriteLog = true;
            LoadRuner = new LoadRuner(this);
        }

        private ConcurrentDictionary<string, object> mProperties = new ConcurrentDictionary<string, object>();

        public object this[string name]
        {
            get
            {
                mProperties.TryGetValue(name, out object result);
                return result;
            }
            set
            {
                mProperties[name] = value;
            }
        }

        private List<ExampleInfo> mExamples = new List<ExampleInfo>();

        internal LoadRuner LoadRuner { get; set; }

        private BeetleX.FastHttpApi.HttpApiServer mHttpApiServer;

        public BeetleX.FastHttpApi.HttpApiServer HttpApiServer => mHttpApiServer;

        public Status Status { get; internal set; }

        internal IList<ExampleInfo> Examples => mExamples;

        public int Port { get; private set; }

        public void Register(params System.Reflection.Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                foreach (var item in assemblies)
                {
                    foreach (var type in item.GetTypes())
                    {
                        if (!type.IsAbstract && type.IsClass && type.GetInterface("CodeBenchmark.IExample") != null)
                        {
                            ExampleInfo info = new ExampleInfo();
                            
                            var category = type.GetCustomAttribute<System.ComponentModel.CategoryAttribute>();
                            info.ID = type.Assembly.GetName().Name.Replace('.','_') + "_" + type.Name;
                            info.Category = category == null ? type.Assembly.GetName().Name : category.Category;
                            info.Version = type.Assembly.GetName().Version.ToString();
                            info.Example = type;
                            info.Name = type.Name;
                            info.Description = type.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description;
                            mExamples.Add(info);
                        }
                    }
                }
            }
        }

        public void Start(int port = 9090)
        {
            Port = port;
            mHttpApiServer[BENCHMARK_TAG] = this;
            mHttpApiServer.Register(typeof(Benchmark).Assembly);
            mHttpApiServer.Options.Port = port;
            mHttpApiServer.Options.SetDebug();
            mHttpApiServer.Open();
        }
        public bool EnabledLog(BeetleX.EventArgs.LogType type)
        {
            return mHttpApiServer.EnableLog(type);
        }
        public void OpenWeb()
        {
            var ps = new ProcessStartInfo($"http://localhost:{Port}/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);

        }

        public void AddLog(BeetleX.EventArgs.LogType type, string messages)
        {
            mHttpApiServer.Log(type, messages);
        }

    }
}
