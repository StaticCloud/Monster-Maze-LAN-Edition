using System.Text.Json;
using Spectre.Console;
using MonsterMaze.Utils;

namespace MonsterMaze.GameLogic
{
    internal class Grid
    {
        public Canvas Canvas { get; init; }
        public Coords ClientCoords { get; private set; }
        public Coords ServerCoords { get; private set; }
        public string[] Map { get; init; }

        public int Width { get; init; }
        public int Height { get; init; }

        public Grid() 
        {
            Width = 23;
            Height = 23;
            Canvas = new Canvas(Width, Height);
            Map = new[] {
                "#######################",
                "#_____________________#",
                "#_##_##_###_###_##_##_#",
                "#_#___#_#_____#_#___#_#",
                "#___#_____#_#_____#___#",
                "#_#___#_#_____#_#___#_#",
                "#_##_##_###_###_##_##_#",
                "#_____________________#",
                "#_##_###_##_##_###_##_#",
                "#_#___#_________#___#_#",
                "#___#___#_###_#___#___#",
                "#_#___#_________#___#_#",
                "#_##_###_##_##_###_##_#",
                "#_____________________#",
                "#_##_##_###_###_##_##_#",
                "#_#___#_#_____#_#___#_#",
                "#___#_____#_#_____#___#",
                "#_#___#_#_____#_#___#_#",
                "#_##_##_###_###_##_##_#",
                "#_____________________#",
                "#######################",
            };

            ClientCoords = new Coords(1, 1);
            ServerCoords = new Coords(1, 1);

            DrawMap();
        }

        public void DrawMap()
        {
            AnsiConsole.Clear();

            for (int i = 0; i < Map.Length; i++)
            {
                for (int j = 0; j < Map[i].Length; j++) 
                {
                    char tile = Map[i][j];

                    Color color = tile switch
                    {
                        '#' => Color.White,
                        '_' => Color.Black
                    };

                    Canvas.SetPixel(j, i, color);
                }
            }

            Canvas.SetPixel(ServerCoords.X, ServerCoords.Y, Color.Aqua);
            Canvas.SetPixel(ClientCoords.X, ClientCoords.Y, Color.Red);

            AnsiConsole.Write(Canvas);
        }

        public void Update(PlayerType player, string payload)
        {
            AnsiConsole.Clear();

            Coords coords = JsonSerializer.Deserialize<Coords>(payload);

            if (player == PlayerType.Server)
            {
                ServerCoords = coords;
            }
            
            if (player == PlayerType.Client)
            {
                ClientCoords = coords;
            }

            DrawMap();
        }

        public char[][] GetView(Player player)
        {
            int distance = 4;

            Direction direction = player.Direction;

            if (direction == Direction.N)
            {
                distance = player.Coords.Y - distance < 0 ? player.Coords.Y : distance;
            }
            else if (direction == Direction.S) 
            {
                distance = player.Coords.Y + distance > Map.Length - 1 ? Map.Length - 1 - player.Coords.Y : distance;
            }
            else if (direction == Direction.E)
            {
                distance = player.Coords.X + distance > Map[player.Coords.Y].Length - 1 ? Map[player.Coords.Y].Length - 1 - player.Coords.X : distance;
            }
            else if (direction == Direction.W)
            {
                distance = player.Coords.X - distance < 0 ? player.Coords.X : distance;
            }

            Console.WriteLine(distance);

            return [['k']];
        }

        public bool SpaceIsFree(Player player)
        {
            return player.Direction switch
            {
                Direction.S => Map[player.Coords.Y + 1][player.Coords.X] != '#',
                Direction.N => Map[player.Coords.Y - 1][player.Coords.X] != '#',
                Direction.E => Map[player.Coords.Y][player.Coords.X + 1] != '#',
                Direction.W => Map[player.Coords.Y][player.Coords.X - 1] != '#'
            };
        }
    }
}
