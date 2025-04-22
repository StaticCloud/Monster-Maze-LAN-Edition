using MonsterMaze.Utils;

namespace MonsterMaze.GameLogic
{
    internal class Player
    {
        public Direction[] Directions { private get; init; }

        public int DirectionIndex { get; private set; }

        public Coords Coords { get; set; }
        public PlayerType Type { get; init; }

        public Direction Direction { get => Directions[DirectionIndex]; }

        public Player(PlayerType type)
        {
            Type = type;
            Coords = new Coords(1, 1);
            Directions = [Direction.N, Direction.E, Direction.S, Direction.W];
            DirectionIndex = 2;
        }
    
        public void TurnLeft()
        {
            DirectionIndex = (DirectionIndex + 3) % 4;
        }

        public void TurnRight()
        {
            DirectionIndex = (DirectionIndex + 1) % 4;
        }
    }
}
