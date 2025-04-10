using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterMaze
{
    internal class Client
    {
        public Client()
        {

        }

        public static async Task Start()
        {
            IPEndPoint iPEndPoint = new(IPAddress.Loopback, 3001);

            TcpClient client = new();

            try
            {
                client.Connect(iPEndPoint);

                Console.WriteLine("Connected to server!");

                NetworkStream stream = client.GetStream();

                _ = Task.Run(() => Listen(stream));

                while (true)
                {
                    string message = Console.ReadLine();
                    var messageByte = Encoding.UTF8.GetBytes(message);

                    await stream.WriteAsync(messageByte);
                }
            } finally
            {
                client.Dispose();
            }
        }

        private async static Task Listen(NetworkStream stream)
        {
            while (true)
            {
                var buffer = new byte[4096];
                int recieved = await stream.ReadAsync(buffer, 0, buffer.Length);

                var message = Encoding.UTF8.GetString(buffer, 0, recieved);
                Console.WriteLine($"Server: {message}");
            }
        }
    }
}
