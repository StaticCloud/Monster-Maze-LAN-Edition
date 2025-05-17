using System.Text.Json;
using Spectre.Console;
using MonsterMaze.Utils;
using System.Text;

namespace MonsterMaze.GameLogic
{
    internal class Grid
    {
        public Canvas Canvas { get; init; }
        public Coords ClientCoords { get; private set; }
        public Coords ServerCoords { get; private set; }
        public string[] Map { get; private set; }
        public string[] View {  get; private set; }

        public string[][] MonsterSprites { get; init; }

        public int Width { get; init; }
        public int Height { get; init; }

        public int Distance { get; private set; }

        public Grid() 
        {
            Width = 23;
            Height = 23;
            Canvas = new Canvas(Width, Height);
            Map = new[] {
                "#######################",
                "#_____________________#",
                "#_#_#_#_#_###_#_#_#_#_#",
                "#_#_#_#_#_____#_#___#_#",
                "#___#___###_###___#___#",
                "#_#_#_#_#_____#_#_#_#_#",
                "#_#_#_#_#_###_#_#_#_#_#",
                "#_____________________#",
                "#_##_###_##_##_###_##_#",
                "#_#___#_________#___#_#",
                "#___#___#_###_#___#___#",
                "#_#___#_________#___#_#",
                "#_##_###_##_##_###_##_#",
                "#_____________________#",
                "#_#_#_#_#_###_#_#_#_#_#",
                "#_#_#_#_#_____#_#___#_#",
                "#___#___###_###___#___#",
                "#_#_#_#_#_____#_#_#_#_#",
                "#_#_#_#_#_###_#_#_#_#_#",
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

            MonsterSprites = new string[3][];

            MonsterSprites[0] = new string[] {
                    "______________________",
                    "______________________",
                    "______________________",
                    "________######________",
                    "_______########_______",
                    "_______#OO##OO#_______",
                    "_______#OO##OO#_______",
                    "_______########_______",
                    "______####O#O##_______",
                    "_____##O######O#______",
                    "____###OO#OO#OO##_____",
                    "___####OOOOOOO####____",
                    "___#####OOOOOO#####___",
                    "___#_####OOOO######___",
                    "_____###########_##___",
                    "______##########__#___",
                    "______##########______",
                    "_____####___####______",
                    "_____###_____####_____",
                    "_____________####_____",
                    "______________________",
                    "______________________"
            };

            MonsterSprites[1] = new string[] {
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "__________####_________",
                "_________######________",
                "_________OO##OO________",
                "________#OO##OO#_______",
                "_______##########______",
                "______###O#O##O##______",
                "______###OOOOOO##______",
                "______#_##OOOO#________",
                "________#######________",
                "________########_______",
                "_______####__###_______",
                "_______###_____________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________"
            };

            MonsterSprites[2] = new string[] {
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "__________###__________",
                "__________O#O__________",
                "_________#####_________",
                "__________####_________",
                "____________#__________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________",
                "_______________________"
            };

            ClientCoords = new Coords(1, 1);
            ServerCoords = new Coords(1, 1);
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

            RefreshMap();

            UpdateMap(ServerCoords, 'S');
            UpdateMap(ClientCoords, 'C');
        }

        private void RefreshMap()
        {
            Map = new[] {
                "#######################",
                "#_____________________#",
                "#_#_#_#_#_###_#_#_#_#_#",
                "#_#_#_#_#_____#_#___#_#",
                "#___#___###_###___#___#",
                "#_#_#_#_#_____#_#_#_#_#",
                "#C#_#_#_#_###_#_#_#_#_#",
                "#_____________________#",
                "#_##_###_##_##_###_##_#",
                "#_#___#_________#___#_#",
                "#___#___#_###_#___#___#",
                "#S#___#_________#___#_#",
                "#_##_###_##_##_###_##_#",
                "#_____________________#",
                "#_#_#_#_#_###_#_#_#_#_#",
                "#_#_#_#_#_____#_#___#_#",
                "#___#___###_###___#___#",
                "#_#_#_#_#_____#_#_#_#_#",
                "#_#_#_#_#_###_#_#_#_#_#",
                "#_____________________#",
                "#######################",
            };
        }

        private void UpdateMap(Coords coords, char character)
        {
            string row = Map[coords.Y];

            StringBuilder rowBuilder = new StringBuilder(row);

            rowBuilder[coords.X] = character;

            Map[coords.Y] = rowBuilder.ToString();
        }
 
        public void GetView(Player player)
        {
            Distance = 4;

            Direction direction = player.Direction;
            char[,] grid = new char[Distance,3];

            if (direction == Direction.N)
            {
                for (int i = 1; i < Distance; i++)
                {
                    if (Map[player.Coords.Y - i][player.Coords.X] == '#')
                    {
                        Distance = i;
                        break;
                    }
                }

                grid = new char[Distance, 3];

                for (int i = 0; i < Distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y - i][player.Coords.X + 1];
                    grid[i, 1] = Map[player.Coords.Y - i][player.Coords.X];
                    grid[i, 2] = Map[player.Coords.Y - i][player.Coords.X - 1];
                }
            }
            else if (direction == Direction.S)
            {
                for (int i = 1; i < Distance; i++)
                {
                    if (Map[player.Coords.Y + i][player.Coords.X] == '#')
                    {
                        Distance = i;
                        break;
                    }
                }

                grid = new char[Distance, 3];

                for (int i = 0; i < Distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y + i][player.Coords.X - 1];
                    grid[i, 1] = Map[player.Coords.Y + i][player.Coords.X];
                    grid[i, 2] = Map[player.Coords.Y + i][player.Coords.X + 1];
                }
            }
            else if (direction == Direction.E)
            {
                for (int i = 1; i < Distance; i++)
                {
                    if (Map[player.Coords.Y][player.Coords.X + i] == '#')
                    {
                        Distance = i;
                        break;
                    }
                }

                grid = new char[Distance, 3];

                for (int i = 0; i < Distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y + 1][player.Coords.X + i];
                    grid[i, 1] = Map[player.Coords.Y][player.Coords.X + i];
                    grid[i, 2] = Map[player.Coords.Y - 1][player.Coords.X + i];
                }
            }
            else if (direction == Direction.W)
            {
                for (int i = 1; i < Distance; i++)
                {
                    if (Map[player.Coords.Y][player.Coords.X - i] == '#')
                    {
                        Distance = i;
                        break;
                    }
                }

                grid = new char[Distance, 3];

                for (int i = 0; i < Distance; i++)
                {
                    grid[i, 0] = Map[player.Coords.Y - 1][player.Coords.X - i];
                    grid[i, 1] = Map[player.Coords.Y][player.Coords.X - i];
                    grid[i, 2] = Map[player.Coords.Y + 1][player.Coords.X - i];
                }
            }

            AnsiConsole.Clear();
            ClearView();

            try
            {
                for (int row = Distance - 1; row >= 0; row--)
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

            RenderGame();   
        }

        private void RenderGame()
        {
            Table GameView = new Table();
            GameView.Alignment(Justify.Center);
            GameView.BorderColor(Color.Red);
            GameView.AddColumn("");
            GameView.AddColumn("Client PL");
            GameView.AddColumn("Server PL");
            GameView.AddRow(Canvas).Centered();
            AnsiConsole.Write(GameView);
        }

        private void ClearView()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Canvas.SetPixel(i, j, Color.Black);
                }
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
                    else if (col == 1 && Distance == 4)
                    {
                        DrawWall(11, 10, 3, 1, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(12, 10, 3, 2, Wall.None);
                    }
                }
                else if (grid[row, col] == '#')
                {
                    if (col == 2)
                    {
                        DrawWall(9, 9, 5, 2, Wall.Left);
                    }
                    else if (col == 0)
                    {
                        DrawWall(13, 9, 5, 2, Wall.Right);
                    }
                }

                if (col == 2)
                {
                    if (grid[row, col - 1] == 'S' || grid[row, col - 1] == 'C')
                    {
                        DrawMonster(MonsterSprites[2]);
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
                    else if (col == 1 && Distance == 3)
                    {
                        DrawWall(9, 8, 7, 5, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(14, 8, 7, 3, Wall.None);
                    }
                }
                else if (grid[row, col] == '#')
                {
                    if (col == 2)
                    {
                        DrawWall(6, 6, 11, 3, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(9, 8, 7, 5, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(16, 6, 11, 3, Wall.Right);
                    }
                }

                if (col == 2)
                {
                    if (grid[row, col - 1] == 'S' || grid[row, col - 1] == 'C')
                    {
                        DrawMonster(MonsterSprites[1]);
                    }
                }
                
            }
            if (row == 1)
            {
                if (grid[row, col] == '_')
                {
                    if (col == 2)
                    {
                        DrawWall(3, 5, 13, 3, Wall.None);
                    }
                    else if (col == 1 && Distance == 2)
                    {
                        DrawWall(6, 5, 13, 11, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(17, 5, 13, 3, Wall.None);
                    }
                }
                else if (grid[row, col] == '#')
                {
                    if (col == 2)
                    {
                        DrawWall(3, 3, 17, 3, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(6, 5, 13, 11, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(19, 3, 17, 3, Wall.Right);
                    }
                }

                if (col == 2)
                {
                    if (grid[row, col - 1] == 'S' || grid[row, col - 1] == 'C')
                    {
                        DrawMonster(MonsterSprites[0]);
                    }
                }
            }
            if (row == 0)
            {
                if (grid[row, col] == '_' || grid[row, col] == 'C' || grid[row, col] == 'S')
                {
                    if (col == 2)
                    {
                        DrawWall(0, 2, 19, 3, Wall.None);
                    }
                    else if (col == 1 && Distance == 1)
                    {
                        Console.WriteLine(col);
                        DrawWall(3, 2, 19, 17, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(20, 2, 19, 3, Wall.None);
                    }
                }
                else if (grid[row, col] == '#')
                {
                    if (col == 2)
                    {
                        DrawWall(0, 0, 23, 3, Wall.Left);
                    }
                    else if (col == 1)
                    {
                        DrawWall(3, 2, 19, 17, Wall.None);
                    }
                    else if (col == 0)
                    {
                        DrawWall(22, 0, 23, 3, Wall.Right);
                    }
                }
            }
        }

        private void DrawWall(int x, int y, int height, int range, Wall wall)
        {
            if (wall != Wall.None)
            {
                for (int i = 0; i < range; i++)
                {
                    for (int j = 0; j < height - (i * 2); j++)
                    {
                        if (wall == Wall.Left)
                        {
                            Canvas.SetPixel(x + i, y + j + i, Color.Grey70);
                        }
                        else if (wall == Wall.Right)
                        {
                            Canvas.SetPixel(x - i, y + j + i, Color.Grey70);
                        }
                    }
                }
            } 
            else
            {
                for (int i = 0; i < range; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Canvas.SetPixel(x + i, y + j, Color.White);
                    }
                }
            }
        }
        
        private void DrawMonster(string[] sprite)
        {
            for (int i = 0; i < 22; i++)
            {
                for (int j = 0; j < 22; j++)
                {
                    if (sprite[j][i] == '#')
                    {
                        Canvas.SetPixel(i, j, Color.Red);
                    }

                    if (sprite[j][i] == 'O')
                    {
                        Canvas.SetPixel(i, j, Color.Black);
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
