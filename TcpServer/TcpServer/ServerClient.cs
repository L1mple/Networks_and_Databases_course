using System;

using System.Net.Sockets;
using System.Threading;

namespace TcpServer {
    // Клиент. Зеркало клиента на стороне сервера. Отвечает за прием пакетов от клиента и отправку пакетов ему.
    class ServerClient {
        private Socket socketClient;
        private bool bRun;
        private Server server;
        private string sName;

        public ServerClient(Socket _socketClient, Server _server) {
            socketClient = _socketClient;
            server = _server;
            bRun = false;
            sName = "";
        }

        public string Name { get { return sName; } set { sName = value; } }

        // запуск потока приема данных от клиента
        public void Start() {
            bRun = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadReceive));
        }
        public void Stop() {
            bRun = false;
        }

        private void ThreadReceive(object ob) { // прием сообщений от клиента в отдельном потоке
            try {
                byte[] bytes = new byte[2048];
                while (bRun) {
                    // предполагаем получение нескольких пакетов одновременно
                    int nRecvCount = socketClient.Receive(bytes);

                    int iPos = 0;
                    while (iPos < nRecvCount) { // итерация - разбор очередного пакета в принятой последовательности
                        Packet packet = new Packet();
                        packet.LoadHeader(bytes, iPos);       // прочесть заголовок
                        packet.FromBytes(bytes, iPos, false); //          тело
                        iPos += packet.Size;

                        server.ProcessPacket(packet, this);
                    }
                }
            }
            catch (SocketException e) {
                Console.WriteLine("function:\tServerClient::ThreadReceive()");
                Console.WriteLine("error code:\t" + e.SocketErrorCode);
                Console.WriteLine("error text:\t" + e.Message);

                server.DisconnectClient(this);
                Console.WriteLine("client disconnected: " + Name);
            }
        }

        public void Send(Packet packet) {
            try {
                byte[] bytes = packet.ToBytes();
                socketClient.Send(bytes);
            }
            catch (SocketException e) {
                Console.WriteLine("function:\tServerClient::Send()");
                Console.WriteLine("error code:\t" + e.SocketErrorCode);
                Console.WriteLine("error text:\t" + e.Message);
            }
        }
    }
}
