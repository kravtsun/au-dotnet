namespace MiniRoguelike
{
    internal struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point Invalid()
        {
            return new Point(-1, -1);
        }

        public bool IsInvalid()
        {
            return X == -1 && Y == -1;
        }
    }
}
