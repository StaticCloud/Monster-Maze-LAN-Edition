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

        public async Task Start()
        {
            IPEndPoint iPEndPoint = new(IPAddress.Loopback, 3001);

            TcpClient client = new();

            try
            {
                client.Connect(iPEndPoint);

                Console.WriteLine("Connected to server!");

                NetworkStream stream = client.GetStream();

                Player clientPlayer = new Player();

                _ = Task.Run(() => Listen(stream));

                while (true)
                {
                    await clientPlayer.HandleMovements(stream);
                }
            } 
            finally
            {
                client.Close();
            }
        }

        private async Task Listen(NetworkStream stream)
        {
            while (true)
            {
                byte[] buffer = new byte[4096];
                int recieved = await stream.ReadAsync(buffer, 0, buffer.Length);

                string message = Encoding.UTF8.GetString(buffer, 0, recieved);
                Console.WriteLine($"Server: {message}");
            }
        }
    }
}
