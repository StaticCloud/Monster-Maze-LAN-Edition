using System.Net.Sockets;
using System.Net;
using System.Text;
using MonsterMaze.GameLogic;
using MonsterMaze.Utils;

namespace MonsterMaze.Connection
{
    internal class Client
    {
        public Game Game { get; private set; }
        public Player Player { get; private set; }

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

                Player = new Player(PlayerType.Client);

                Game = new Game(stream, Player);

                _ = Task.Run(() => Listen(stream));

                while (true) 
                {
                    await Game.Run();
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

                string payload = Encoding.UTF8.GetString(buffer, 0, recieved);
                Game.Grid.Update(PlayerType.Server, payload);
            }
        }
    }
}
