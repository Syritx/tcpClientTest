using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Environment.Server
{
    class Server
    {
        // run the server first
        static List<Socket> clients;
        static ClientCommands clientCommands = new ClientCommands();

        static IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        static int port = 5050;

        public static void Main(string[] args) {
            Console.Title = "[SERVER PANEL]";
            Console.WriteLine("[SERVER PANEL]");

            clients = new List<Socket>();
            server = new Socket(AddressFamily.InterNetwork,
                                SocketType.Stream,
                                ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(ip, port);
            server.Bind(endPoint);
            server.Listen(5);
            Console.WriteLine("Created server");

            ReceiveClients();
        }

        static void clientThread(Socket client) {
            string message = "welcome\n";

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] bytes = new byte[2048];

            client.Send(messageBytes);
            Console.WriteLine("sent message to client");
            IPEndPoint clientEndPoint = client.RemoteEndPoint as IPEndPoint;

            while (client.Connected) {
                bytes = new byte[2048];
                int i = client.Receive(bytes);
                bool canSendMessage = true;

                foreach (string cmd in clientCommands.startMessages) {
                    if (cmd == Encoding.UTF8.GetString(bytes)) canSendMessage = false;
                }

                Console.WriteLine("[{0}, {1}] SENT: {2}",clientEndPoint.Address,
                                                    clientEndPoint.Port,
                                                    Encoding.UTF8.GetString(bytes));

                foreach(Socket clientSocket in clients) {
                    if (clientSocket != client && canSendMessage) {
                        try { clientSocket.Send(bytes); }
                        catch (Exception e) { clients.Remove(clientSocket); }
                    }
                }
            }
            clients.Remove(client);
        }

        static void ReceiveClients() {
            while (true) {

                Socket client = server.Accept();
                clients.Add(client);
                Task.Run(() => clientThread(client));
            }
        }
    }
}
