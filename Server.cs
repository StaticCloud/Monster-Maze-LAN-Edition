using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonsterMaze
{
    internal class Server
    {
        public Server() 
        {
            
        }

        public static async Task Start()
        {
            IPEndPoint iPEndPoint = new(IPAddress.Loopback, 3001);

            TcpListener listener = new(iPEndPoint);

            try
            {
                listener.Start();

                Console.WriteLine("Awaiting client...");

                using TcpClient handler = listener.AcceptTcpClient();

                Console.WriteLine("Client connected!");

                NetworkStream stream = handler.GetStream();

                _ = Task.Run(() => Listen(stream));

                while (true)
                {

                    string message = Console.ReadLine();
                    var messageByte = Encoding.UTF8.GetBytes(message);

                    await stream.WriteAsync(messageByte);
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private static async Task Listen(NetworkStream stream)
        {
            while (true) 
            {
                var buffer = new byte[4096];
                int recieved = await stream.ReadAsync(buffer, 0, buffer.Length);

                var message = Encoding.UTF8.GetString(buffer, 0, recieved);
                Console.WriteLine($"Client: {message}");
            }
        }
    }
}
