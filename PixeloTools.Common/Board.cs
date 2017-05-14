using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.Common
{
    public class Board
    {
        public Board(int width,int height)
        {
            _data = new int[height, width];
        }

        private int[,] _data;

        public int Width
        {
            get { return _data.GetLength(1); }
        }
        public int Height
        {
            get { return _data.GetLength(0); }
        }

        public State Get(int x, int y)
        {
            return (State)_data[y, x];
        }

        public void Set(int x, int y, State value)
        {
            _data[y, x] = (int)value;
        }

        public enum State
        {
            Unknown = 0,
            Occupied = 1,
            Empty = -1,
            Any = 666
        }

        public Board Clone()
        {
            var board = new Board(Width, Height);
            board._data = (int[,])_data.Clone();
            return board;
        }

        public double GetQuality()
        {
            var occupied = 0;
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (Get(x, y) == State.Occupied) occupied++;
                }
            }
            return 100.0 * Math.Min(occupied, Width * Height - occupied) / (Width * Height);
        }

        public ColumnAndRowData GetData()
        {
            var data = new ColumnAndRowData();
            for (var x = 0; x < Width; x++)
            {
                var list = new List<int>();
                var state = State.Unknown;
                int s = 0;
                for (var y = 0; y < Height; y++)
                {
                    var newState = Get(x, y);
                    if (state != newState)
                    {
                        if (state == State.Occupied)
                        {
                            list.Add(s);
                        }
                        s = 1;
                        state = newState;
                    }
                    else
                    {
                        s++;
                    }
                }
                if (state == State.Occupied)
                {
                    list.Add(s);
                }
                if (!list.Any())
                {
                    list.Add(0);
                }
                data.Columns.Add(list);
            }
            for (var y = 0; y < Height; y++)
            {
                var list = new List<int>();
                var state = State.Unknown;
                int s = 0;
                for (var x = 0; x < Width; x++)
                {
                    var newState = Get(x, y);
                    if (state != newState)
                    {
                        if (state == State.Occupied)
                        {
                            list.Add(s);
                        }
                        s = 1;
                        state = newState;
                    }
                    else
                    {
                        s++;
                    }
                }
                if (state == State.Occupied)
                {
                    list.Add(s);
                }
                if (!list.Any())
                {
                    list.Add(0);
                }
                data.Rows.Add(list);
            }
            return data;
        }
    }
}
