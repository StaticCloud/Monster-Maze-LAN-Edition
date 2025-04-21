using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using MonsterMaze.Utils;

namespace MonsterMaze.GameLogic
{
    internal class Game
    {
        public NetworkStream Stream { get; init; }
        public Player Player { get; init; }
        public Grid Grid { get; init; }
        public Camera Camera { get; init; }

        public Game(NetworkStream stream, Player player)
        {
            Stream = stream;
            Player = player;
            Grid = new Grid();
            Camera = new Camera();
        }

        public async Task Run()
        {
            await HandleMovements();
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vkey);

        private static bool KeyDown(ConsoleKey key)
        {
            return (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        public async Task HandleMovements()
        {
            bool keydown = false;

            if (KeyDown(ConsoleKey.UpArrow))
            {
                try
                {
                    if (Grid.SpaceIsFree(Player))
                    {
                        if (Player.GetDirection().Equals(Direction.N))
                        {
                            Player.Coords = new Coords(Player.Coords.X, Player.Coords.Y - 1);
                        }
                        else if (Player.GetDirection().Equals(Direction.E))
                        {
                            Player.Coords = new Coords(Player.Coords.X - 1, Player.Coords.Y);
                        }
                        else if (Player.GetDirection().Equals(Direction.S))
                        {
                            Player.Coords = new Coords(Player.Coords.X, Player.Coords.Y + 1);
                        }
                        else if (Player.GetDirection().Equals(Direction.W))
                        {
                            Player.Coords = new Coords(Player.Coords.X + 1, Player.Coords.Y);
                        }

                        keydown = true;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex);
                }

            }
            else if (KeyDown(ConsoleKey.LeftArrow))
            {
                Player.DirectionIndex = (Player.DirectionIndex + 1) % Player.Directions.Length;
                keydown = true;
            }
            else if (KeyDown(ConsoleKey.RightArrow))
            {
                if (Player.DirectionIndex == 0) Player.DirectionIndex = 3;
                else Player.DirectionIndex = (Player.DirectionIndex - 1) % Player.Directions.Length;
                keydown = true;
            }

            if (keydown)
            {
                Grid.Update(Player.Type, Player.Coords.toJSON());
                Grid.GetView(Player);
                
                await TransmitMovements(Stream);
            }

            Thread.Sleep(1000);
        }

        private async Task TransmitMovements(NetworkStream stream)
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(Player.Coords.toJSON());

            await stream.WriteAsync(messageByte);
        }
    }
}
