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
        private Canvas _canvas;
        private Coords _clientCoords;
        private Coords _serverCoords;
        private string[] _map;

        public Grid() 
        {
            _canvas = new Canvas(23, 23);
            _map = new[] {
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

            for (int i = 0; i < _map.Length; i++)
            {
                for (int j = 0; j < _map[i].Length; j++) 
                {
                    char tile = _map[i][j];

                    Color color = tile switch
                    {
                        '#' => Color.White,
                        '_' => Color.Black
                    };

                    _canvas.SetPixel(j, i, color);
                }
            }

            _canvas.SetPixel(_serverCoords.X, _serverCoords.Y, Color.Aqua);
            _canvas.SetPixel(_clientCoords.X, _clientCoords.Y, Color.Red);

            AnsiConsole.Write(_canvas);
        }

        public void Update(PlayerType player, string payload)
        {
            Coords coords = JsonSerializer.Deserialize<Coords>(payload);

            if (player == PlayerType.Server)
            {
                _serverCoords = coords;
            }
            
            if (player == PlayerType.Client)
            {
                _clientCoords = coords;
            }

            DrawMap();
        }
    }

    enum PlayerType
    {
        Server, Client
    }
}
