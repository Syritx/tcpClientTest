using System;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace OpenGL.Environment.Client.Game
{
    class Client
    {
        // run two of instances of the client (AFTER THE SERVER)
        static IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        static TcpClient client;
        static NetworkStream stream;

        public static void Main(string[] args)
        {
            client = new TcpClient(ip.ToString(), 5050);

            string message = "i am a client that connected to the server";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            stream = client.GetStream();
            stream.Write(messageBytes, 0, messageBytes.Length);
            stream.Flush();
            StayConnected();
        }

        public static void sendMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            NetworkStream str = client.GetStream();
            str.Write(bytes, 0, bytes.Length);
        }

        static void StayConnected()
        {
            //new GameUI(1000, 720, "multiplayer");
            while (true) {
                byte[] data = new byte[2048];

                int response = stream.Read(data, 0, data.Length);
                string responseData = Encoding.ASCII.GetString(data, 0, response);
                Console.WriteLine(responseData);

                sendMessage(Console.ReadLine());
            }
        }

        ~Client()
        {
            sendMessage("disconnected from the server");

            stream.Close();
            client.Close();
        }
    }
}
