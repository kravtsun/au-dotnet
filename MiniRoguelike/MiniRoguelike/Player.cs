namespace MiniRoguelike
{
    internal class Player
    {
        private readonly Map _map;
        private Point _position;

        public Player(Map map, Point position)
        {
            _map = map;
            _position = position;
        }

        public void Walk(int dx, int dy)
        {
            var newPosition = new Point(_position.X + dx, _position.Y + dy);
            newPosition = TruncatePoint(newPosition);
            if (!_map.IsFreeCell(newPosition))
            {
                return;
            }

            _map.SwapCells(_position, newPosition);
            _position = newPosition;
            _map.SetFocus(_position);
        }

        private Point TruncatePoint(Point point)
        {
            var width = _map.Width;
            var height = _map.Height;
            var newX = point.X >= width ? width - 1 : point.X < 0 ? 0 : point.X;
            var newY = point.Y >= height ? height - 1 : point.Y < 0 ? 0 : point.Y;
            return new Point(newX, newY);
        }
    }
}