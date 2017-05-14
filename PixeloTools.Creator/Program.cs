using PixeloTools.Common;
using PixeloTools.Solving;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PixeloTools.Creator
{
    class Program
    {
        private static int _rotateChar;

        static void Main(string[] args)
        {
            int puzzleSize = 20;
            if (args.Length != 2 || !File.Exists(args[0]) || !int.TryParse(args[1], out puzzleSize))
            {
                Console.WriteLine("usage: " + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + " <image> <puzzleSize>");
                Console.WriteLine("Tries to create a square Pixelo puzzle from an image.");
                Environment.Exit(-1);
            }

            var values = new double[puzzleSize, puzzleSize, 4];
            using (var originalimage = (Bitmap)Image.FromFile(args[0]))
            {
                var maxSize = 500.0;
                Bitmap resize = null;
                if (originalimage.Width > maxSize && originalimage.Height > maxSize)
                {
                    Console.WriteLine("Bigger than {0:N0}x{0:N0}. Resizing first", maxSize);
                    var q = originalimage.Width > originalimage.Height
                        ? maxSize / originalimage.Height
                        : maxSize / originalimage.Width;
                    resize = new Bitmap(originalimage.GetThumbnailImage((int)(originalimage.Width * q), (int)(originalimage.Height * q), null, IntPtr.Zero));
                }
                using (var image = autoCrop(resize ?? originalimage))
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        for (var y = 0; y < image.Height; y++)
                        {
                            var color = image.GetPixel(x, y);
                            var bx = (int)((double)x * puzzleSize / image.Width);
                            var by = (int)((double)y * puzzleSize / image.Height);
                            values[bx, by, 0] += color.R;
                            values[bx, by, 1] += color.G;
                            values[bx, by, 2] += color.B;
                            values[bx, by, 3]++;
                        }
                    }
                }
                if (resize != null) resize.Dispose();
            }
            var minQuality = 20;
            var threshold = 128;
            var n = 1;
            var accepted = new List<Solution>();
            while (threshold > 0 && threshold < 255)
            {
                var board = new Board(puzzleSize, puzzleSize);
                for (var x = 0; x < puzzleSize; x++)
                {
                    for (var y = 0; y < puzzleSize; y++)
                    {
                        var val = (values[x, y, 0] + values[x, y, 1] + values[x, y, 2]) / (3 * values[x, y, 3]);
                        if ((n / 2) % 2 == 1)
                        {
                            board.Set(x, y, val < threshold ? Board.State.Occupied : Board.State.Empty);
                        }
                        else
                        {
                            board.Set(x, y, val > threshold ? Board.State.Occupied : Board.State.Empty);
                        }
                    }
                }

                var boardCursor = new[] { Console.CursorLeft, Console.CursorTop };
                Console.WriteLine();
                displayBoard(board);
                Console.SetCursorPosition(boardCursor[0], boardCursor[1]);

                var data = board.GetData();
                var solver = new Solver
                {
                    CreatePartialSolutions = false,
                    CreateFinalSolutions = false,
                    MaxPuzzleLevel = 1
                };
                solver.Load(data);
                var quality = board.GetQuality();
                if (quality > minQuality)
                {
                    Console.WriteLine("Threshold: " + threshold + " Quality: " + quality + " Solving...");
                    solver.Solve();
                }
                var totalCombinations = solver.GetTotalCombinations();
                Console.WriteLine("Threshold: " + threshold + " Solutions: " + totalCombinations + " Quality: " + quality);
                if (totalCombinations == 1 && quality > minQuality && solver.Combinations.Count >= puzzleSize / 3)
                {
                    boardCursor = new[] { Console.CursorLeft, Console.CursorTop };
                    for (var i=0; i<board.Height; i++) Console.WriteLine(new string(' ' ,board.Width));
                    Console.SetCursorPosition(boardCursor[0], boardCursor[1]);
                    Console.WriteLine("Data: " + data);
                    Console.WriteLine("Solutions: " + solver.GetTotalCombinations());
                    Console.WriteLine("Level: " + solver.MaxLevelReached);
                    Console.WriteLine("Evolution: " + string.Join(", ", solver.Combinations.Select(c => c == 0 ? 0 : (int)Math.Round(Math.Log10(c), MidpointRounding.AwayFromZero))));
                    solver = new Solver
                    {
                        CreateFinalSolutions = true,
                        CreatePartialSolutions = true
                    };
                    solver.Load(data);
                    solver.Solve();
                    displayBoard(solver.Solutions.First());
                    Console.WriteLine("Press Y/N to accept/reject this solution, S to save solutions, Q to quit");
                    while (true)
                    {
                        var key = Console.ReadKey().Key;
                        switch (key)
                        {
                            case ConsoleKey.Q:
                                saveSolutions(accepted);
                                Environment.Exit(0);
                                break;
                            case ConsoleKey.Y:
                                accepted.Add(new Solution
                                {
                                    Data = data,
                                    Combinations = solver.Combinations
                                });
                                break;
                            case ConsoleKey.S:
                                saveSolutions(accepted);
                                break;
                        }
                        if (key == ConsoleKey.Y || key == ConsoleKey.N) break;
                    }
                }
                n++;
                threshold = 128 + n / 4 * (n % 2 == 1 ? 1 : -1);
            }
            Console.WriteLine("End of run");
            saveSolutions(accepted);
            Console.ReadKey();
        }

        private static void saveSolutions(List<Solution> accepted)
        {
            if (accepted.Count == 0) return;
            var sb = new StringBuilder();
            foreach (var solution in accepted.OrderByDescending(s=>s.Combinations.Count).ThenByDescending(s=>s.Combinations.First()))
            {
                sb.AppendLine(solution.Data.ToString());
                sb.AppendLine(string.Join(", ", solution.Combinations.Select(c => c == 0 ? 0 : (int)Math.Round(Math.Log10(c), MidpointRounding.AwayFromZero))));
                sb.AppendLine();
            }
            File.WriteAllText("solutions.txt", sb.ToString());

            Console.WriteLine("Saved all accepted solutions in solutions.txt");
        }

        private static Bitmap autoCrop(Bitmap image)
        {
            Console.Write("Cropping...");
            var pixels = new List<List<Color>>();
            for (var y = 0; y < image.Height; y++)
            {
                var row = new List<Color>();
                for (var x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);
                    row.Add(color);
                }
                pixels.Add(row);
            }
            var energyList = new List<List<double>>();
            for (var y = 0; y < image.Height; y++)
            {
                var row = new List<double>();
                for (var x = 0; x < image.Width; x++)
                {
                    var color = pixels[y][x];
                    var energy = 0.0;
                    var k = 0;
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        for (var dy = -1; dy <= 1; dy++)
                        {
                            if ((dx != 0 || dy != 0) && x + dx >= 0 && x + dx < image.Width && y + dy >= 0 && y + dy < image.Height)
                            {
                                k++;
                                energy += colorEnergy(color, image.GetPixel(x + dx, y + dy));
                            }
                        }
                    }
                    row.Add(energy / k);
                }
                energyList.Add(row);
            }

            removeSeam(pixels, energyList,0,pixels.First().Count);
            var transPixels = transpose(pixels);
            var transEnergyList = transpose(energyList);
            removeSeam(transPixels, transEnergyList,0,transPixels.First().Count);

            if (transPixels.First().Count > transPixels.Count)
            {
                removeSeam(transPixels, transEnergyList, null,transPixels.First().Count - transPixels.Count);
            }

            pixels = transpose(transPixels);

            if (pixels.First().Count > pixels.Count)
            {
                energyList = transpose(transEnergyList);
                removeSeam(pixels, energyList, null, pixels.First().Count - pixels.Count);
            }


            var result = new Bitmap(pixels.First().Count, pixels.Count);
            var energySize = Math.Max(result.Width, result.Height) / 20;
            var ry = 0;
            var blobs = new List<Tuple<Color,float,float,float>>();
            foreach (var row in pixels)
            {
                var rx = 0;
                foreach (var color in row)
                {
                    //result.SetPixel(rx, ry, color);
                    var size = (float)Math.Max(1, energySize * energyList[ry][rx]);
                    blobs.Add(new Tuple<Color,float,float,float>(color,rx,ry,size));
                    rx++;
                }
                ry++;
            }
            using (var g = Graphics.FromImage(result))
            {
                foreach (var blob in blobs.OrderBy(b => b.Item4))
                {
                    //g.DrawRectangle(new Pen(blob.Item1, 1), blob.Item2, blob.Item3, blob.Item4, blob.Item4);
                    g.FillRectangle(new SolidBrush(blob.Item1), blob.Item2, blob.Item3, blob.Item4, blob.Item4);
                }
            }
            result.Save("cropped.png");
            Console.WriteLine();
            return result;
        }

        private static List<List<T>> transpose<T>(List<List<T>> list)
        {
            var result=new List<List<T>>();
            var y = 0;
            foreach (var row in list)
            {
                var x = 0;
                foreach (var val in row)
                {
                    if (result.Count <= x) result.Add(new List<T>());
                    result[x].Add(val);
                    x++;
                }
                y++;
            }
            return result;
        }

        private static void removeSeam(List<List<Color>> pixels, List<List<double>> energyList, double? threshold, int maxRows)
        {
            var cursor = new[] { Console.CursorLeft, Console.CursorTop };
            while (maxRows > 0)
            {
                var vertSeamArr = new List<List<double>>();
                foreach (var row in energyList)
                {
                    var vals = (double[])row.ToArray().Clone();
                    vertSeamArr.Add(vals.ToList());
                }
                for (var y = 1; y < vertSeamArr.Count; y++)
                {
                    for (var x = 0; x < vertSeamArr[y].Count; x++)
                    {
                        var minEnergy = double.MaxValue;
                        var sdx = 0;
                        foreach (var dx in new[] { 0, 1, -1 })
                        {
                            if (x + dx >= 0 && x + dx < vertSeamArr[y].Count)
                            {
                                var val = vertSeamArr[y - 1][x + dx];
                                if (minEnergy > val)
                                {
                                    minEnergy = val;
                                    sdx = dx;
                                }
                            }
                        }
                        vertSeamArr[y][x] += minEnergy;
                    }
                }
                var t = threshold??vertSeamArr[vertSeamArr.Count - 1].Min();
                var seamX = Enumerable.Range(0, vertSeamArr[vertSeamArr.Count - 1].Count)
                    .Where(x => vertSeamArr[vertSeamArr.Count - 1][x] == t)
                    .Cast<int?>()
                    .FirstOrDefault();
                Console.SetCursorPosition(cursor[0], cursor[1]);
                if (seamX == null)
                {
                    Console.Write(" ");
                    Console.SetCursorPosition(cursor[0], cursor[1]);
                    break;
                }
                maxRows--;
                Console.Write(getNextRotateChar());
                var sx = seamX.Value;
                var sy = vertSeamArr.Count - 1;

                while (sy >= 0)
                {
                    pixels[sy].RemoveAt(sx);
                    vertSeamArr[sy].RemoveAt(sx);
                    energyList[sy].RemoveAt(sx);

                    if (sy > 0)
                    {
                        var minEnergy = double.MaxValue;
                        var sdx = 0;
                        foreach (var dx in new[] { 0, 1, -1 })
                        {
                            if (sx + dx >= 0 && sx + dx < vertSeamArr[sy - 1].Count)
                            {
                                var val = vertSeamArr[sy - 1][sx + dx];
                                if (minEnergy > val)
                                {
                                    minEnergy = val;
                                    sdx = dx;
                                }
                            }
                        }
                        sx += sdx;
                    }
                    sy--;
                }
            }
        }

        private static char getNextRotateChar()
        {
            var chars=@"/|\-";
            _rotateChar = (_rotateChar + 1) % chars.Length;
            return chars[_rotateChar];

        }

        private static double colorEnergy(Color color1, Color color2)
        {
            //return Math.Abs(color1.GetBrightness() - color2.GetBrightness());
            return Math.Sqrt(Math.Pow(color1.R - color2.R, 2) + Math.Pow(color1.G - color2.G, 2) + Math.Pow(color1.B - color2.B, 2) / 3) / 255;
        }

        const char cOccupied = 'X';
        const char cEmpty = '.';
        const char cUnknown = '~';

        private static void displayBoard(Board board)
        {
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    switch (board.Get(x, y))
                    {
                        case Board.State.Empty: Console.Write(cEmpty); break;
                        case Board.State.Occupied: Console.Write(cOccupied); break;
                        default: Console.Write(cUnknown); break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }


    }
}
