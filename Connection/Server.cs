using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MonsterMaze.GameLogic;

namespace MonsterMaze.Connection
{
    internal class Server
    {

        private Game _game;
        private Player _player;

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

                _player = new Player(PlayerType.Server);

                _game = new Game(stream, _player);
                
                _ = Task.Run(() => Listen(stream));

                while (true)
                {
                    await _game.Run();
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
                _game.UpdateClient(payload);
            }
        }
    }
}
