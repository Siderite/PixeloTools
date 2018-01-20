using PixeloTools.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.Solving
{
    public class Solver
    {
        private ColumnAndRowData _data;
        private List<PossibilitiesRecord> _colPossibilities;
        private List<PossibilitiesRecord> _rowPossibilities;
        private static Dictionary<int, List<List<int>>> _dictIndexes = new Dictionary<int, List<List<int>>>();

        public Solver()
        {
            Boards = new List<Board>();
            Solutions = new List<Board>();
            Combinations = new List<double>();
            CreateFinalSolutions = true;
            CreatePartialSolutions = true;
            MaxPuzzleLevel = int.MaxValue;
            MaxDisplayedSolutions = 20;
        }

        public bool CreatePartialSolutions { get; set; }
        public bool CreateFinalSolutions { get; set; }
        public int MaxPuzzleLevel { get; set; }
        public int MaxDisplayedSolutions { get; set; }

        public List<Board> Boards { get; private set; }
        public List<Board> Solutions { get; private set; }
        public List<double> Combinations { get; private set; }
        public int MaxLevelReached { get; private set; }
        public int FinalSolutionCount { get; private set; }

        public double GetTotalCombinations()
        {
            ensureDataLoaded();
            var totalCombinations = Math.Min(
                _colPossibilities.Select(colpos => colpos.Possibilities.Count).Aggregate(1.0, (p, v) => p * v),
                _rowPossibilities.Select(rowpos => rowpos.Possibilities.Count).Aggregate(1.0, (p, v) => p * v)
            );
            return totalCombinations;
        }

        public void Load(ColumnAndRowData data)
        {
            _data = data;
            // all possible combinations for each column
            _colPossibilities = _data.Columns.Select((col, idx) => new PossibilitiesRecord
            {
                Index = idx,
                Possibilities = getPossibleValues(col, _data.Height).ToList()
            }).OrderBy(pos => pos.Possibilities.Count).ToList();
            // all possible combinations for each row
            _rowPossibilities = _data.Rows.Select((row, idx) => new PossibilitiesRecord
            {
                Index = idx,
                Possibilities = getPossibleValues(row, _data.Width).ToList()
            }).OrderBy(pos => pos.Possibilities.Count).ToList();
        }


        public void Solve()
        {
            ensureDataLoaded();
            Boards.Clear();
            Solutions.Clear();
            var level = 1;
            MaxLevelReached = 1;
            Combinations.Clear();
            var totalCombinations = GetTotalCombinations();
            Combinations.Add(totalCombinations);
            var prevCombinations = totalCombinations;
            var boardSize = Math.Max(_data.Width, _data.Height);
            while (true)
            {
                // get all possible combinations of nr+1 indexes, so that we can filter the possibilities away
                // nr=0 means the possible combinations are just one of each perpendicular rows or columns
                // nr=1 means possibilities are weighed against every combination in any two perpendicular rows or columns,etc
                var piRows = getAllPossibleIndexes(_data.Height, level);
                filterPossibilities(_colPossibilities, _rowPossibilities, piRows); // filter column possibilities by rows
                totalCombinations = GetTotalCombinations();
                addCombinationNr(totalCombinations);
                generateBoard(); // generates a board with partial results

                if (prevCombinations != totalCombinations)
                {
                    level = 1;
                }
                Debug.WriteLine($"Combinations so far: {totalCombinations}, level: {level}, max level reached: {MaxLevelReached}");
                Console.WriteLine($"Combinations so far: {totalCombinations}, level: {level}, max level reached: {MaxLevelReached}");

                var piCols = getAllPossibleIndexes(_data.Width, level);
                filterPossibilities(_rowPossibilities, _colPossibilities, piCols); // filter row possibilities by columns
                totalCombinations = GetTotalCombinations();
                if (totalCombinations <= 1) break;
                addCombinationNr(totalCombinations);
                generateBoard(); // generates a board with partial results

                if (prevCombinations == totalCombinations)
                {
                    if (level <= boardSize && level < MaxPuzzleLevel)
                    {
                        level++;
                        if (level > MaxLevelReached) MaxLevelReached = level;
                    }
                    else
                    {
                        Debug.WriteLine("Maximum puzzle level reached");
                        Console.WriteLine("Maximum puzzle level reached. Starting brute force (it will take a while, please wait...)");
                        break;
                    }
                }
                else
                {
                    level = 1;
                }
                Debug.WriteLine($"Combinations so far: {totalCombinations}, level: {level}, max level reached: {MaxLevelReached}");
                Console.WriteLine($"Combinations so far: {totalCombinations}, level: {level}, max level reached: {MaxLevelReached}");

                prevCombinations = totalCombinations;
            }
            // fill all possible solutions from the remaining column and row possibilities
            generateSolutions();
        }

        private void addCombinationNr(double totalCombinations)
        {
            if (Combinations.LastOrDefault() != totalCombinations)
            {
                Combinations.Add(totalCombinations);
            }
        }

        private void ensureDataLoaded()
        {
            if (_data == null) throw new InvalidOperationException("Load data before attempting this operation");
        }

        protected IEnumerable<Board.State[]> getPossibleValues(List<int> line, int totalSize)
        {
            if (line.Count == 1 && line[0] == 0)
            {
                yield return Enumerable.Range(0, totalSize).Select(i => Board.State.Empty).ToArray();
                yield break;
            }

            var values = new List<List<int>>();
            values.Add(new List<int>());

            foreach (var nr in line)
            {
                var newValues = new List<List<int>>();
                foreach (var positions in values)
                {
                    var min = positions.Any()
                        ? positions.Last() + line[positions.Count - 1] + 1
                        : 0;
                    var max = totalSize - nr;
                    for (var pos = min; pos <= max; pos++)
                    {
                        var newPositions = new List<int>(positions);
                        newPositions.Add(pos);
                        newValues.Add(newPositions);
                    }
                }
                values = newValues;
            }
            foreach (var positions in values)
            {
                var list = new List<Board.State>();
                for (var i = 0; i < totalSize; i++)
                {
                    var occupied = Enumerable.Range(0, positions.Count)
                                    .Any(r => positions[r] <= i && positions[r] + line[r] > i);
                    list.Add(occupied ? Board.State.Occupied : Board.State.Empty);
                }
                yield return list.ToArray();
            }
        }

        private static List<List<int>> getAllPossibleIndexes(int boardSize, int nr)
        {
            if (nr > boardSize) nr = boardSize;
            List<List<int>> result;
            var key = boardSize * 1000 + nr;
            if (_dictIndexes.TryGetValue(key, out result))
            {
                return result;
            }
            result = new List<List<int>>();
            for (var i = 0; i < boardSize; i++)
            {
                result.Add(new List<int> { i });
            }
            for (var n = 1; n < nr; n++)
            {
                var l = result.Count;
                for (var i = 0; i < boardSize; i++)
                {
                    for (var j = 0; j < l; j++)
                    {
                        var list = result[j];
                        if (!list.Contains(i))
                        {
                            result.Add(new List<int>(list) { i });
                        }
                    }
                }
                result.RemoveRange(0, l);
            }
            _dictIndexes[key] = result;
            return result;
        }

        private static void filterPossibilities(List<PossibilitiesRecord> possibilities, List<PossibilitiesRecord> filters, List<List<int>> possibleIndexes)
        {
            foreach (var possibility in possibilities)
            {
                var i = 0;
                while (i < possibility.Possibilities.Count)
                {
                    var valid = !possibleIndexes
                        .AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                        .Any(irs => irs.Any(ir => !filters[ir].Possibilities.Any(pos => pos[possibility.Index] == possibility.Possibilities[i][filters[ir].Index])));
                    if (valid)
                    {
                        i++;
                    }
                    else
                    {
                        possibility.Possibilities.RemoveAt(i);
                    }
                }
            }
            possibilities.Sort(new Comparison<PossibilitiesRecord>((p1, p2) => -p1.Possibilities.Count.CompareTo(p2.Possibilities.Count)));
        }

        private void generateBoard()
        {
            if (!CreatePartialSolutions) return;
            var board = new Board(_data.Width, _data.Height);
            foreach (var colpos in _colPossibilities)
            {
                var col = getSurePoints(board.Height, colpos);
                for (var y = 0; y < col.Length; y++)
                {
                    if (col[y] != Board.State.Unknown)
                    {
                        board.Set(colpos.Index, y, col[y]);
                    }
                }
            }
            foreach (var rowpos in _rowPossibilities)
            {
                var row = getSurePoints(board.Height, rowpos);
                for (var x = 0; x < row.Length; x++)
                {
                    if (row[x] != 0)
                    {
                        board.Set(x, rowpos.Index, row[x]);
                    }
                }
            }
            var prevBoard = Boards.LastOrDefault();
            if (prevBoard == null)
            {
                Boards.Add(board);
            }
            else
            {
                var fire = false;
                for (var x = 0; x < board.Width; x++)
                {
                    for (var y = 0; y < board.Height; y++)
                    {
                        if (board.Get(x, y) != prevBoard.Get(x, y))
                        {
                            fire = true;
                            break;
                        }
                    }
                }
                if (fire) Boards.Add(board);
            }
        }

        private static Board.State[] getSurePoints(int boardSize, PossibilitiesRecord rec)
        {
            var values = Enumerable.Repeat(Board.State.Unknown, boardSize).ToArray();
            for (var i = 0; i < boardSize; i++)
            {
                foreach (var pos in rec.Possibilities)
                {
                    if (values[i] == pos[i])
                    {
                        continue;
                    }
                    if (values[i] == 0)
                    {
                        values[i] = pos[i];
                    }
                    else
                    {
                        values[i] = Board.State.Any;
                        break;
                    }
                }
            }
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] == Board.State.Any)
                {
                    values[i] = Board.State.Unknown;
                }
            }
            return values;
        }

        private void generateSolutions()
        {
            if (!CreateFinalSolutions) return;

            var enums = Enumerable.Range(0, _rowPossibilities.Count)
                .Select(i => new PersistentListEnumerator<Board.State[]>(_rowPossibilities
                    .First(pos => pos.Index == i)
                    .Possibilities))
                .ToArray();

            var loop = enums.All(en => en.MoveNext());
            FinalSolutionCount = 0;
            var board = new Board(_data.Width, _data.Height);
            var p = -1;
            var rowsSkipped = 0;
            var possibilities = enums.Aggregate(new BigInteger(1), (acc, val) => acc * val.Count);
            var prevPossibilities = new BigInteger(0);
            var start = DateTime.Now;
            while (loop)
            {
                if (prevPossibilities == 0 || (int)Math.Log((double)prevPossibilities, 10) > (int)Math.Log((double)possibilities, 10) || DateTime.Now - start > TimeSpan.FromSeconds(10))
                {
                    Debug.WriteLine($"Possibilities left: {possibilities} solutions so far: {FinalSolutionCount}");
                    Console.WriteLine($"Possibilities left: {(double)possibilities:E5} solutions so far: {FinalSolutionCount}");
                    prevPossibilities = possibilities;
                    start = DateTime.Now;
                }
                for (var i = p + 1; i < enums.Length; i++)
                {
                    var enumerator = enums[i];
                    var row = enumerator.Current;
                    for (var j = 0; j < row.Length; j++)
                    {
                        board.Set(j, i, row[j]);
                    }
                    // get index where row is invalid to filter other similar rows
                    var invalidIndex = getInvalidColumnIndex(board, i);
                    if (invalidIndex == -1)
                    {
                        p = i;
                    }
                    else
                    {
                        Board.State[] nextRow = null;
                        // remove rows that have the same value at the invalid index
                        var rowsEliminated = new BigInteger(0);
                        do
                        {
                            rowsEliminated++;
                            nextRow = enumerator.PeekNext();
                        } while (nextRow != null && row[invalidIndex] == nextRow[invalidIndex] && enumerator.MoveNext());
                        rowsSkipped += (int)(rowsEliminated - 1);
                        for (var k = i + 1; k < enums.Length; k++)
                        {
                            rowsEliminated *= enums[k].Count;
                        }
                        possibilities -= rowsEliminated;
                        break;
                    }
                }
                if (p < 0) throw new Exception("Should not happen");
                var isFinalSolution = p == enums.Length - 1;
                if (isFinalSolution)
                {
                    FinalSolutionCount++;
                    if (FinalSolutionCount <= MaxDisplayedSolutions)
                    {
                        Solutions.Add(board.Clone());
                    }
                    else if (FinalSolutionCount == MaxDisplayedSolutions + 1)
                    {
                        Console.WriteLine($"Only showing {MaxDisplayedSolutions} solutions");
                        Debug.WriteLine($"Only showing {MaxDisplayedSolutions} solutions");
                    }
                }
                if (p < enums.Length - 1) p++;
                do
                {
                    var moved = enums[p].MoveNext();
                    if (moved) break;
                    p--;
                    loop = p >= 0;
                } while (loop);

                for (var i = p + 1; i < enums.Length; i++)
                {
                    enums[i].Reset();
                    var moved = enums[i].MoveNext();
                    if (!moved) throw new Exception("This should never happen");
                }
                if (isFinalSolution)
                {
                    p = -1;
                }
                else
                {
                    p--;
                }
            }
            Debug.WriteLine($"Possibilities left: {possibilities} solutions so far: {FinalSolutionCount}");
            Console.WriteLine($"Possibilities left: {possibilities} solutions so far: {FinalSolutionCount}");
            Debug.WriteLine($"Skipped rows: {rowsSkipped}");
        }

        private int getInvalidColumnIndex(Board board, int rowIndex)
        {
            for (var j = 0; j < board.Width; j++)
            {
                var list = new List<int>();
                int c = 0;
                var state = Board.State.Unknown;
                for (var i = 0; i <= rowIndex; i++)
                {
                    var value = board.Get(j, i);
                    if (state == value)
                    {
                        c++;
                    }
                    else
                    {
                        if (state == Board.State.Occupied)
                        {
                            list.Add(c);
                        }
                        c = 1;
                        state = value;
                    }
                }
                if (list.Count > _data.Columns[j].Count)
                {
                    return j;
                }
                if (state == Board.State.Occupied)
                {
                    if (list.Count == _data.Columns[j].Count || c > _data.Columns[j][list.Count])
                    {
                        return j;
                    }
                }
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i] != _data.Columns[j][i])
                    {
                        return j;
                    }
                }
            }
            return -1;
        }
    }
}