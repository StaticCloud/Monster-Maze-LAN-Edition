using System.Net.Sockets;
using System.Net;
using System.Text;
using MonsterMaze.GameLogic;

namespace MonsterMaze.Connection
{
    internal class Client
    {
        private Game _game;
        private Player _player;

        public Client()
        {

        }

        public async Task Start()
        {
            IPEndPoint iPEndPoint = new(IPAddress.Parse("10.0.0.201"), 3001);

            TcpClient client = new();

            try
            {
                client.Connect(iPEndPoint);

                Console.WriteLine("Connected to server!");

                NetworkStream stream = client.GetStream();

                _player = new Player(PlayerType.Client);

                _game = new Game(stream, _player);

                _ = Task.Run(() => Listen(stream));

                while (true) 
                {
                    await _game.Run();
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
                _game.UpdateServer(payload);
            }
        }
    }
}
