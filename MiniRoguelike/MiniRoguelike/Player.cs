namespace MiniRoguelike
{
    internal class Player
    {
        private readonly Map _map;
        private Map.Point _position;

        public Player(Map map, Map.Point position)
        {
            _map = map;
            _position = position;
        }

        public void Walk(int dx, int dy)
        {
            var newPosition = new Map.Point(_position.X + dx, _position.Y + dy);
            newPosition = TruncatePoint(newPosition);
            if (!_map.GetCell(newPosition.X, newPosition.Y).IsFree())
            {
                return;
            }

            var oldCell = _map.GetCell(_position.X, _position.Y);
            var newCell = _map.GetCell(newPosition.X, newPosition.Y);
            _map.SetCell(_position.X, _position.Y, newCell);
            _map.SetCell(newPosition.X, newPosition.Y, oldCell);
            _position = newPosition;
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