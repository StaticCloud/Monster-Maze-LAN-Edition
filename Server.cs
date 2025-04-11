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

        public async Task Start()
        {
            IPEndPoint iPEndPoint = new(IPAddress.Loopback, 3001);

            TcpListener listener = new(iPEndPoint);

            try
            {
                listener.Start();

                Console.WriteLine("Awaiting client...");

                TcpClient handler = listener.AcceptTcpClient();

                Console.WriteLine("Client connected!");

                NetworkStream stream = handler.GetStream();

                Player serverPlayer = new Player();

                _ = Task.Run(() => Listen(stream));

                while (true)
                {
                    await serverPlayer.HandleMovements(stream);
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task Listen(NetworkStream stream)
        {
            while (true) 
            {
                byte[] buffer = new byte[4096];
                int recieved = await stream.ReadAsync(buffer, 0, buffer.Length);

                string message = Encoding.UTF8.GetString(buffer, 0, recieved);
                Console.WriteLine($"Client: {message}");
            }
        }
    }
}
