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

        public Grid() 
        {
            _canvas = new Canvas(24, 24);

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 24; j++) _canvas.SetPixel(i, j, Color.White);
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(_canvas);
        }

        public void Update(PlayerType player, string payload)
        {
            Coords coords = JsonSerializer.Deserialize<Coords>(payload);

            if (player == PlayerType.Server)
            {
                _serverCoords = coords;
                _canvas.SetPixel(_serverCoords.X, _serverCoords.Y, Color.Red);
            }
            else
            {
                _clientCoords = coords;
                _canvas.SetPixel(_clientCoords.X, _clientCoords.Y, Color.Aqua);
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(_canvas);
        }
    }

    enum PlayerType
    {
        Server, Client
    }
}
