using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeBenchmark.ConsoleApp
{
    [System.ComponentModel.Category("TCP")]
    public class ReadLine : IExample
    {
        public async Task Execute()
        {
            var data = $"henryfan@{DateTime.Now}";
            var stream= await mClient.ReceiveFrom(s => s.WriteLine(data));
            stream.ReadLine();
           
        }

        private BeetleX.Clients.AsyncTcpClient mClient;

        public void Initialize(Benchmark benchmark)
        {
            mClient = BeetleX.SocketFactory.CreateClient<BeetleX.Clients.AsyncTcpClient>("192.168.2.19", 9012);
        }
    }
}
