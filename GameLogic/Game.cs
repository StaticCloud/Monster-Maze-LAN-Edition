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
            // Not proud of this at all, but it does fix the synchronization issue. Please educate yourself further on multithreading and asynchronosity, please.
            Action gridFunc = _player.Type switch
            {
                PlayerType.Server => () => UpdateServer(_player.Coords.toJSON()),
                PlayerType.Client => () => UpdateClient(_player.Coords.toJSON())
            };

            await _player.HandleMovements(_stream, gridFunc);
        }

        public void UpdateClient(string payload) => Grid.Update(PlayerType.Client, payload);
        public void UpdateServer(string payload) => Grid.Update(PlayerType.Server, payload);
    }
}
