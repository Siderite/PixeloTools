using PixeloTools.Common;
using PixeloTools.Solving;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixeloTools.GameSolver
{
    class Program
    {
        private static char[][] _chars = new char[][] {
            new[] { 'X','.','~'},
            new[] { '\u2588','\u2591','?'},
            new[] { '\u2588','x','.'}
        };

        private static int _charsIndex=2;
        private static bool _overlap = true;
        private static bool _pressKeyToSeeSolutions = true;
        private static ColumnAndRowData _data;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var values = File.ReadAllText(args[0]);
                solveData(ColumnAndRowData.Parse(values));
            }
            var id = HotKeyManager.RegisterHotKey(Keys.P, KeyModifiers.Control | KeyModifiers.Shift);
            HotKeyManager.HotKeyPressed += HotKeyManager_HotKeyPressed;
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("Press Ctrl-Shift-P over the Pixelo window to solve it.");
            Console.WriteLine("Press q in this window to close it");
            Console.WriteLine("Press o in this window to show the partial solutions overlapping (ON by default)");
            Console.WriteLine("Press w in this window to have to press a key to see the next partial solution (ON by default)");
            Console.WriteLine("Press c in this window to change the characters used to display the solutions");
            Console.WriteLine("Press r in this window to replay the last solve");
            while (true)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(500);
                    continue;
                }
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q) break;
                if (key == ConsoleKey.O)
                {
                    _overlap = !_overlap;
                    Console.WriteLine("Overlap mode for partial solutions is now " + (_overlap ? "ON" : "OFF"));
                }
                if (key == ConsoleKey.W)
                {
                    _pressKeyToSeeSolutions = !_pressKeyToSeeSolutions;
                    Console.WriteLine("Press a key to see next partial or total solution mode is now " + (_pressKeyToSeeSolutions ? "ON" : "OFF"));
                }
                if (key == ConsoleKey.C)
                {
                    _charsIndex = (_charsIndex + 1) % _chars.Length;
                    Console.WriteLine("The display charset is now " + string.Join(", ", _chars[_charsIndex]));
                }
                if (key == ConsoleKey.R && _data != null)
                {
                    solveData(_data);
                }
            }
            HotKeyManager.UnregisterHotKey(id);
        }

        static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (e.Modifiers != (KeyModifiers.Control | KeyModifiers.Shift) || e.Key != Keys.P) return;
            try
            {
                using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                            Screen.PrimaryScreen.Bounds.Height))
                {
                    Console.WriteLine("Getting screenshot");
                    using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                    {
                        g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                         Screen.PrimaryScreen.Bounds.Y,
                                         0, 0,
                                         bmpScreenCapture.Size,
                                         CopyPixelOperation.SourceCopy);
                    }
                    SetFocus();
                    var parser = new ImageParser();
                    parser.Parse(bmpScreenCapture);
                    Console.WriteLine(parser.Output);
                    if (string.IsNullOrWhiteSpace(parser.Values))
                    {
                        Console.WriteLine("Could not understand the screenshot");
                        return;
                    }
                    _data = ColumnAndRowData.Parse(parser.Values);
                    solveData(_data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error in the program: " + ex.Message);
            }
        }

        private static void solveData(ColumnAndRowData data)
        {
            var solver = new Solver
            {
                MaxPuzzleLevel = 3
            };
            solver.Load(data);
            var now = DateTime.Now;
            solver.Solve();
            var timespan = (DateTime.Now - now);
            Console.WriteLine("Evolution:");
            displayEvolution(solver.Combinations);
            Console.WriteLine("Solved in: " + timespan.TotalMilliseconds + " milliseconds");
            if (solver.MaxLevelReached > 1) Console.WriteLine("Detected puzzle level:" + solver.MaxLevelReached);
            Console.WriteLine();
            var quit = false;
            Console.WriteLine("Partial solutions:");
            var cursor = new[] { Console.CursorLeft, Console.CursorTop };
            foreach (var board in solver.Boards)
            {
                if (_overlap) Console.SetCursorPosition(cursor[0], cursor[1]);
                displayBoard(board);
                if (_pressKeyToSeeSolutions)
                {
                    Console.WriteLine("Press Space to see next partial solution, w to go to the end, q to stop display");
                Readagain:
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Spacebar:
                            break;
                        case ConsoleKey.W:
                            _pressKeyToSeeSolutions = false;
                            break;
                        case ConsoleKey.C:
                            _charsIndex = (_charsIndex + 1) % _chars.Length;
                            displayBoard(board);
                            goto Readagain;
                        case ConsoleKey.Q:
                            quit = true;
                            break;
                        default:
                            goto Readagain;
                    }
                }
                if (quit) break;
            };
            if (quit)
            {
                Console.WriteLine("Solution display aborted");
            }
            else
            {
                ConsoleColor? saved = null;
                if (solver.Solutions.Count != 1)
                {
                    saved = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine();
                Console.WriteLine("Final solutions displayed (" + solver.Solutions.Count + ") :");
                if (saved!=null)
                {
                    Console.ForegroundColor = saved.Value;
                }
                foreach (var solution in solver.Solutions)
                {
                    displayBoard(solution);
                }
                saved = Console.ForegroundColor;
                if (solver.Solutions.Count != 1)
                {
                    displayOverlapBoards(solver.Solutions);
                    Console.ForegroundColor = ConsoleColor.Red;
                } else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.WriteLine("Final solution count = " + solver.FinalSolutionCount);
                if (saved != null)
                {
                    Console.ForegroundColor = saved.Value;
                }
                Console.WriteLine();
            }
        }

        private static void displayOverlapBoards(List<Board> solutions)
        {
            if (solutions.Count == 0) return;
            var firstBoard = solutions[0];
            var width = firstBoard.Width;
            var height = firstBoard.Height;
            var overboard = new Board(width, height);
            for (var i = 0; i<height; i++)
            {
                for (var j=0; j<width; j++)
                {
                    var pixel = firstBoard.Get(j, i);
                    overboard.Set(j, i, pixel);
                    foreach (var board in solutions)
                    {
                        if (pixel != board.Get(j, i))
                        {
                            overboard.Set(j, i, Board.State.Any);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("Overlapped boards:");
            displayBoard(overboard);
        }

        private static void displayEvolution(List<double> values)
        {
            values = values.Select(v => v == 0 ? 0 : Math.Log10(v)).ToList();
            var max = values.Max();
            var sb = new StringBuilder();
            for (var y = 4; y >= 0; y--)
            {
                for (var x = 0; x < values.Count; x++)
                {
                    var val = (int)Math.Round(values[x] / max * 10, MidpointRounding.AwayFromZero) - y * 2;
                    sb.Append(
                        val < 1
                        ? " "
                        : (val == 1 ? "o" : "O")
                        );
                }
                sb.AppendLine();
            }
            Console.Write(sb.ToString());
            Console.WriteLine("Numeric: " + string.Join(", ", values.Select(v => (int)Math.Round(v, MidpointRounding.AwayFromZero))));
            Console.WriteLine("Complexity: {0:N2}",values.Sum());
            Console.WriteLine();
        }

        private static void displayBoard(Board board)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    switch (board.Get(x, y))
                    {
                        case Board.State.Empty: Console.Write(_chars[_charsIndex][1]); break;
                        case Board.State.Occupied: Console.Write(_chars[_charsIndex][0]); break;
                        default: Console.Write(_chars[_charsIndex][2]); break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ForegroundColor = color;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void SetFocus()
        {
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr hWnd = currentProcess.MainWindowHandle;
            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, 5);
            }
        }
    }
}