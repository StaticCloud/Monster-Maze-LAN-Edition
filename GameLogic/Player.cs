using MonsterMaze.Utils;

namespace MonsterMaze.GameLogic
{
    internal class Player
    {
        public Direction[] Directions { get; }

        public int DirectionIndex { get; set; }

        public Coords Coords { get; set; }
        public PlayerType Type { get; init; }

        public Player(PlayerType type)
        {
            Type = type;
            Coords = new Coords(1, 1);
            Directions = [Direction.N, Direction.E, Direction.S, Direction.W];
            DirectionIndex = 0;
        }

        public Direction GetDirection() => Directions[DirectionIndex];
    }
}
