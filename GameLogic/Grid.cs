using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;

namespace MonsterMaze.GameLogic
{
    internal class Grid
    {
        public Canvas Canvas { get; init; }
        public Coords ClientCoords { get; private set; }
        public Coords ServerCoords { get; private set; }
        public string[] Map { get; init; }

        public Grid() 
        {
            Canvas = new Canvas(23, 23);
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
    }

    enum PlayerType
    {
        Server, Client
    }
}
