using System.Net.Sockets;

namespace MonsterMaze.GameLogic
{
    internal class Game
    {
        private NetworkStream _stream;
        private Player _player;
        public Grid Grid;

        public Game(NetworkStream stream, Player player)
        {
            _stream = stream;
            _player = player;
            Grid = new Grid();
        }

        public async Task Run()
        {
            await _player.HandleMovements(_stream);

            if (_player.Type == PlayerType.Client) UpdateClient(_player.Coords.toJSON());
            if (_player.Type == PlayerType.Server) UpdateServer(_player.Coords.toJSON());
        }

        public void UpdateClient(string payload) => Grid.Update(PlayerType.Client, payload);
        public void UpdateServer(string payload) => Grid.Update(PlayerType.Server, payload);
    }
}
