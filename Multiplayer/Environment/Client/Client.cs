using System;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Multiplayer.Environment.Client
{
    class Client
    {
        // run two of instances of the client using the command: dotnet run Client.cs
        static IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        static TcpClient client;
        static NetworkStream stream;

        public static void Main(string[] args)
        {
            client = new TcpClient(ip.ToString(), 5050);

            string message = "[CO-NNE_CT3D-TO_$ER_VER}"; // encrypted to people can't guess
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            stream = client.GetStream();
            stream.Write(messageBytes, 0, messageBytes.Length);
            stream.Flush();
            StayConnected();
        }

        public static void sendMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            IPEndPoint iPEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            message = "["+iPEndPoint.Address+", "+iPEndPoint.Port+"]: " + message;

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            NetworkStream str = client.GetStream();
            str.Write(bytes, 0, bytes.Length);
        }

        static void ReceiveMessages() {
            while (true) {
                byte[] data = new byte[2048];

                int response = stream.Read(data, 0, data.Length);
                string responseData = Encoding.ASCII.GetString(data, 0, response);
                Console.WriteLine(responseData);
            }
        }

        static void SendMessages(){
            while (true) {
                sendMessage(Console.ReadLine());
            }
        }

        static void StayConnected()
        {
            Task messages = Task.Factory.StartNew(SendMessages);
            Task receiver = Task.Factory.StartNew(ReceiveMessages);

            Task.WaitAll(messages,receiver);
        }

        ~Client()
        {
            sendMessage("disconnected from the server");

            stream.Close();
            client.Close();
        }
    }
}
