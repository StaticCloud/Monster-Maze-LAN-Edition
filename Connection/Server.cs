using System.Net;
using System.Net.Sockets;
using System.Text;
using MonsterMaze.GameLogic;
using MonsterMaze.Utils;

namespace MonsterMaze.Connection
{
    internal class Server
    {

        public Game Game { get; private set; }
        public Player Player { get; private set; }

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

                Player = new Player(PlayerType.Server);

                Game = new Game(stream, Player);
                
                _ = Task.Run(() => Listen(stream));

                while (true)
                {
                    await Game.Run();
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

                string payload = Encoding.UTF8.GetString(buffer, 0, recieved);
                Game.Grid.Update(PlayerType.Client, payload);
                Game.Grid.GetView(Player);
            }
        }
    }
}
