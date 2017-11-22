namespace MiniRoguelike
{
    internal class Player
    {
        private readonly Map _map;
        public Map.Point Position { get; private set; }

        public Player(Map map, Map.Point position)
        {
            _map = map;
            Position = position;
        }

        public void Walk(int dx, int dy)
        {
            var newPosition = new Map.Point(Position.X + dx, Position.Y + dy);
            newPosition = TruncatePoint(newPosition);
            if (!_map.IsFreeCell(newPosition))
            {
                return;
            }

            _map.SwapCells(Position, newPosition);
            Position = newPosition;
        }

        private Map.Point TruncatePoint(Map.Point point)
        {
            var width = _map.Width;
            var height = _map.Height;
            var newX = point.X >= width ? width - 1 : point.X < 0 ? 0 : point.X;
            var newY = point.Y >= height ? height - 1 : point.Y < 0 ? 0 : point.Y;
            return new Map.Point(newX, newY);
        }
    }
}