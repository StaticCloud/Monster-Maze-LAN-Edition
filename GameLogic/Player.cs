using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace MonsterMaze.GameLogic
{
    internal class Player
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vkey);

        private Direction[] _directions;

        private int _direction;

        private Coords Coords { get; set; }

        public Player()
        {
            Coords = new Coords(1, 1);
            _directions = [Direction.N, Direction.E, Direction.S, Direction.W];
            _direction = 0;
        }

        private static bool KeyDown(ConsoleKey key)
        {
            return (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        public async Task HandleMovements(NetworkStream stream)
        {
            bool keydown = false;

            if (KeyDown(ConsoleKey.UpArrow))
            {
                try
                {
                    if (GetDirection().Equals(Direction.N))
                    {
                        Coords = new Coords(Coords.X, Coords.Y - 1);
                    }
                    else if (GetDirection().Equals(Direction.E))
                    {
                        Coords = new Coords(Coords.X - 1, Coords.Y);
                    }
                    else if (GetDirection().Equals(Direction.S))
                    {
                        Coords = new Coords(Coords.X, Coords.Y + 1);
                    }
                    else if (GetDirection().Equals(Direction.W))
                    {
                        Coords = new Coords(Coords.X + 1, Coords.Y);
                    }

                    keydown = true;
                } catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex);
                }
               
            }
            else if (KeyDown(ConsoleKey.LeftArrow))
            {
                _direction = (_direction + 1) % _directions.Length;
                keydown = true;
            }
            else if (KeyDown(ConsoleKey.RightArrow))
            {
                if (_direction < 0) _direction = 3;
                else _direction = (_direction - 1) % _directions.Length;
                keydown = true;
            }

            if (keydown)
            {
                await TransmitMovements(stream);
            }

            Thread.Sleep(1000);
        }

        private async Task TransmitMovements(NetworkStream stream)
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(Coords.toJSON());
            
            await stream.WriteAsync(messageByte);
        }

        public enum Direction 
        {
            N, S, E, W
        }

        private Direction GetDirection() => _directions[_direction];
    }
}
