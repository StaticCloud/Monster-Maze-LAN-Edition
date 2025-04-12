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
        }

        public void Update(PlayerType playerType, string payload) => Grid.Update(playerType, payload);
    }
}
