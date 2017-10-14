using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void Walk(int dx, int dy)
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
            int width = _map.Width;
            int height = _map.Height;
            int newX = point.X >= width ? width - 1 : point.X < 0 ? 0 : point.X;
            int newY = point.Y >= height ? height - 1 : point.Y < 0 ? 0 : point.Y;
            return new Map.Point(newX, newY);
        }

        public void MoveLeft() => Walk(-1, 0);

        public void MoveRight() => Walk(1, 0);

        public void MoveUp() => Walk(0, -1);

        public void MoveDown() => Walk(0, 1);
    }
}
