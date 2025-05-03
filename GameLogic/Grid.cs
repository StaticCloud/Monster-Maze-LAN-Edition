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
        public string[] View {  get; private set; }

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

            View = new[]
            {
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
            };

            ClientCoords = new Coords(1, 1);
            ServerCoords = new Coords(1, 1);

            //DrawMap();
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

        public void Update(PlayerType playerType, string payload)
        {
            AnsiConsole.Clear();

            Coords coords = JsonSerializer.Deserialize<Coords>(payload);

            if (playerType == PlayerType.Server)
            {
                ServerCoords = coords;
            }
            
            if (playerType == PlayerType.Client)
            {
                ClientCoords = coords;
            }

            //DrawMap();
        }

        public void GetView(Player player)
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

            char[,] grid = new char[distance, 3];

            if (direction == Direction.N)
            {
                for (int i = 0; i < distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y - 1 - i][player.Coords.X + 1];
                    grid[i, 1] = Map[player.Coords.Y - 1 - i][player.Coords.X];
                    grid[i, 2] = Map[player.Coords.Y - 1 - i][player.Coords.X - 1];
                }
            }
            else if (direction == Direction.S)
            {
                for (int i = 0; i < distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y + 1 + i][player.Coords.X - 1];
                    grid[i, 1] = Map[player.Coords.Y + 1 + i][player.Coords.X];
                    grid[i, 2] = Map[player.Coords.Y + 1 + i][player.Coords.X + 1];
                }
            }
            else if (direction == Direction.E)
            {
                for (int i = 0; i < distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y + 1][player.Coords.X + 1 + i];
                    grid[i, 1] = Map[player.Coords.Y][player.Coords.X + 1 + i];
                    grid[i, 2] = Map[player.Coords.Y - 1][player.Coords.X + 1 + i];
                }
            }
            else if (direction == Direction.W)
            {
                for (int i = 0; i < distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y - 1][player.Coords.X - 1 - i];
                    grid[i, 1] = Map[player.Coords.Y][player.Coords.X - 1 - i];
                    grid[i, 2] = Map[player.Coords.Y + 1][player.Coords.X - 1 - i];
                }
            }

            // Super experimental
            Console.Clear();

            try
            {
                for (int row = distance; row > 0; row--)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        DrawView(row, col, grid);
                    }
                }
            } 
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        private void DrawView(int row, int col, char[,] grid)
        {
            if (row == 3)
            {
                if (grid[row, col] == '_')
                {
                    if (col == 2)
                    {
                        DrawWall(9, 10, 3, 2, Wall.None);
                    }
                    else if (col == 1)
                    {
                        DrawWall(11, 10, 3, 1, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(13, 10, 3, 2, Wall.None);
                    }
                }
                else
                {
                    if (col == 2)
                    {
                        DrawWall(9, 9, 5, 2, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(11, 10, 3, 1, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(13, 9, 5, 2, Wall.Right);
                    }
                }
            }

            if (row == 2)
            {
                if (grid[row, col] == '_')
                {
                    if (col == 2)
                    {
                        DrawWall(6, 8, 7, 3, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(14, 8, 7, 3, Wall.None);
                    }
                }
                else
                {
                    if (col == 2)
                    {
                        DrawWall(6, 6, 11, 3, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(7, 6, 11, 11, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(16, 6, 11, 3, Wall.Right);
                    }
                }
            }

            if (row == 1)
            {
                if (grid[row, col] == '_')
                {
                    if (col == 2)
                    {
                        DrawWall(3, 5, 9, 3, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(17, 5, 9, 3, Wall.None);
                    }
                }
                else
                {
                    if (col == 2)
                    {
                        DrawWall(3, 3, 17, 3, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(7, 6, 11, 11, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(16, 6, 17, 3, Wall.Right);
                    }
                }
            }

            if (row == 0)
            {
                if (grid[row, col] == '_')
                {
                    if (col == 2)
                    {
                        DrawWall(0, 2, 19, 3, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(22, 2, 19, 3, Wall.None);
                    }
                }
                else
                {
                    if (col == 2)
                    {
                        DrawWall(0, 0, 23, 3, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(0, 0, 23, 23, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(0, 23, 23, 3, Wall.Right);
                    }
                }
            }
        }

        private void DrawWall(int x, int y, int height, int range, Wall wall)
        {
            if (wall != Wall.None)
            {
                for (int i = 0; i <= range; i++)
                {
                    for (int j = 0; j <= height - (i * 2); j++)
                    {
                        if (wall == Wall.Left)
                        {
                            Canvas.SetPixel(x + i, y + j, Color.White);
                        }
                        else if (wall == Wall.Right) 
                        {
                            Canvas.SetPixel(x - i, y + j, Color.White);
                        }
                    }
                }
            } 
            else
            {
                for (int i = 0; i <= range; i++)
                {
                    for (int j = 0; j <= height; j++)
                    {
                        Canvas.SetPixel(x, y, Color.DarkSlateGray1);
                    }
                }
            }
        }
        
        private enum Wall
        {
            Right,
            Left,
            None
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
