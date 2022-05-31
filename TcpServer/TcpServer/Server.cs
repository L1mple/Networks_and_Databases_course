using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpServer {
    // Сервер. Принимает подключения клиентов и регистрирует их во внутреннем списке.
    // Авторизует клиентов по имени: принимает запрос (пакет типа 3) от клиента с именем клиента и отправляет ему квитанцию подтверждения или отказа.
    // Разсылает обновленный перечень клиентов (пакет типа 2) всем клиентам после авторизации очередного клиента или отключения подключенного клиента. 
    // Служит посредником при передаче сообщениями между клиентами: при приеме пакета типа 1 от клиента, разыскивает получателя по имени в списке и направляет пакет ему.

    // Ключевые методы:
    //  - привязка порта для прослушивания                     void Bind(string sIp, int nPort)
    //  - прослушивание порта и регистрация новых подключений  void ThreadAcceptClients(object ob)
    //  - разбор принятых пакетов                              void ProcessPacket(Packet packet, ServerClient clientFrom)
    //    - тип 1 передача пакета адресату   
    //    - тип 2 разсылка обновленного списка клиентов после авторизации/отключения клиента
    //    - тип 3 авторизация клиента (клиент в случае успеха наконец получает предложенное им имя)
    //            сервер отправляет клиенту квитанцию-ответ : принял он предложенное имя или нет

    class Server {
        private Socket socketServer;
        // список клиентов, не словарь, поскольку клиент получает имя позже внесения в список 
        private List<ServerClient> listClients; // в списке могут быть неавторизованные клиенты (с пустыми именами)
        private bool bRun;

        public Server() {
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listClients = new List<ServerClient>();
            bRun = false;           
        }

        public void Bind(string sIp, int nPort) {
            EndPoint ep = new IPEndPoint(IPAddress.Parse(sIp), nPort);
            socketServer.Bind(ep);
            socketServer.Listen(20);
        }

        public bool IsRun { get { return bRun; } set { bRun = value; } }
        
        public void Start() {
            bRun = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadAcceptClients));
        }
        
        public void Stop() { bRun = false; }

        // поток для подключения новых клиентов
        private void ThreadAcceptClients(object ob) {
            try {
                while (bRun) {
                    Socket socketClient = socketServer.Accept();
                    ServerClient client = new ServerClient(socketClient, this);
                    client.Start();

                    listClients.Add(client);
                }
            }
            catch (SocketException e) {
                Console.WriteLine("function:\tServer::ThreadAcceptClients()");
                Console.WriteLine("error code:\t" + e.SocketErrorCode);
                Console.WriteLine("error text:\t" + e.Message);
            }
        }

        // разбор пакета от клиента
        public void ProcessPacket(Packet packet, ServerClient clientFrom) {
            switch (packet.Type) {
                case PacketType.SimpleMessage:
                    ProcessSimpleMessage(packet.GetItem(0), // получатель
                                         packet.GetItem(1), // текст
                                         clientFrom);       // отправитель  
                    break;
                case PacketType.Login:
                    ProcessLogin(packet.GetItem(0), // имя клиента
                                 clientFrom);       // отправитель
                    break;
            }
        }

        private void ProcessSimpleMessage(string sClientTo, string sMessage, ServerClient clientFrom) {
            ServerClient clientTo = GetClientByName(sClientTo);
            if (clientTo == null) return; // null - пустой указатель (пустой, не инициализированный объект)

                                          // в отправленном пакете в 0м поле указан адресат
                                          // сервер формирует собственный пакет, в котором указывает отправителя
            Packet packet = new Packet(PacketType.SimpleMessage); 
            packet.SetItem(0, clientFrom.Name);
            packet.SetItem(1, sMessage);
            clientTo.Send(packet);
        }

        private void ProcessLogin(string sNewName, ServerClient clientFrom)  {
            // Trim() - убирает пробелы в начале и в конце строки
            bool bOk = (sNewName != "") && (sNewName.Trim() == sNewName);
            if (bOk) bOk = (GetClientByName(sNewName) == null); // имя уникально

            Packet packetResult = new Packet(PacketType.Login);
            packetResult.SetItem(0, (bOk) ? "allow" : "deny");
            clientFrom.Send(packetResult);

            if (bOk) {
                clientFrom.Name = sNewName;
                SendNames2All();
            }
        }

        // отключение клиента (обработка закрытия сокета клиентом - изъятие из списка)
        public void DisconnectClient(ServerClient clientFrom) {
            listClients.Remove(clientFrom);
            SendNames2All();
        }

        private void SendNames2All() {
            List<string> listNames = ClientNameList;
            Packet packet2All = new Packet(PacketType.ClientList, listNames.Count);
            packet2All.FromItemList(listNames);

            foreach (ServerClient client in listClients)
                if (client.Name != "") client.Send(packet2All); // проверка необходима ибо в список попадают все клиенты, в том числе и неавторизованные (см. ThreadAcceptClients)
        }

        public ServerClient GetClientByName(string sName) {
            foreach (ServerClient client in listClients)
                if (client.Name == sName) return client;
            return null;            
        }

        // список имен всех авторизованных (имеющих имя) клиентов
        private List<string> ClientNameList {
            get { 
                List<string> asRes = new List<string>();
                // проход по всем элементам коллекции от начала
                foreach (ServerClient client in listClients)
                    if (client.Name != "") asRes.Add(client.Name);
                return asRes;
            }
        }
    }
}
