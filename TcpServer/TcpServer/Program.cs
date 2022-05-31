using System.Threading;

namespace TcpServer {
    class Program {
        public static void Main(string[] args) {
            Server server = new Server();
            server.Bind("127.0.0.1", 2020);
            server.Start();
            while (server.IsRun) Thread.Sleep(100);
        }        
    }
}
