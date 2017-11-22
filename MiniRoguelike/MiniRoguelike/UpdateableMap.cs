using System;
using System.Collections.Generic;

namespace MiniRoguelike
{
    internal class UpdateableMap : Map
    {
        private readonly List<CellChange> _cellChanges;
        private int _drawnLeftBase;
        private int _drawnTopBase;
        private bool IsUpToDate => _drawnLeftBase == LeftBase && _drawnTopBase == TopBase;

        public UpdateableMap(string filename) : base(filename)
        {
            _cellChanges = new List<CellChange>();
            _drawnLeftBase = -1;
            _drawnTopBase = -1;
        }

        public override void Draw()
        {
            foreach (var cellChange in _cellChanges)
            {
                var position = cellChange.Position;
                var cell = cellChange.NewCell;
                base.SetCell(position, cell);
                DrawCell(position, cell);
            }
            if (!IsUpToDate)
            {
                base.Draw();
                _drawnLeftBase = LeftBase;
                _drawnTopBase = TopBase;
            }
            _cellChanges.Clear();
        }

        protected override void SetCell(Point position, Cell cell)
        {
            if (IsUpToDate)
            {
                _cellChanges.Add(new CellChange(position, cell));
            }
            else
            {
                base.SetCell(position, cell);
            }
        }

        private void DrawCell(Point position, Cell cell)
        {
            var cursorPosition = new Point(Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition(position.X - LeftBase, position.Y - TopBase);
            Console.Write(cell.ToString());
            Console.SetCursorPosition(cursorPosition.X, cursorPosition.Y);
        }

        private struct CellChange
        {
            public Point Position { get; }
            public Cell NewCell { get; }

            public CellChange(Point position, Cell newCell)
            {
                Position = position;
                NewCell = newCell;
            }
        }
    }
}