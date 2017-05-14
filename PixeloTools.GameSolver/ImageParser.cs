using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.GameSolver
{
    class ImageParser
    {
        public string Output { get; set; }

        public string Values { get; set; }

        internal void Parse(Bitmap image)
        {
            Values = getPixeloValuesFromImage(image);
        }

        private string getPixeloValuesFromImage(Bitmap image, int passes=0)
        {
            var output = new StringBuilder();
            output.AppendLine("Getting board image");
            var black = new bool[image.Height, image.Width];
            var gray = new bool[image.Height, image.Width];
            extractPixels(image, black, gray);
            var patterns = extractPatterns(image, black, gray);
            // set values to recognized patterns
            foreach (var pattern in patterns)
            {
                pattern.Value = Numbers.Get(pattern.Pattern);
            }
            //remove unrecognized patterns
            patterns.RemoveAll(p => p.Value == null);

            joinDigitsIntoNumbers(patterns);

            // get horizontal lines and their length
            var lines = new Dictionary<int, Tuple<int, int>>();
            foreach (var pattern in patterns)
            {
                foreach (var point in pattern.Points)
                {
                    Tuple<int, int> xvalues;
                    if (!lines.TryGetValue(point.Y, out xvalues))
                    {
                        xvalues = new Tuple<int, int>(int.MaxValue, int.MinValue);
                    }
                    lines[point.Y] = new Tuple<int, int>(Math.Min(xvalues.Item1, point.X), Math.Max(xvalues.Item2, point.X + pattern.Pattern.GetLength(1)));
                }
            }

            if (!lines.Any()) return null;

            // longest line will be a one with vertical values
            var longestLine = lines.OrderByDescending(l => l.Value.Item2 - l.Value.Item1).First();
            // the lines that hold horizontal values will be to the left of the vertical values
            var horizontalLines = lines.Where(l => l.Value.Item2 < longestLine.Value.Item1).OrderBy(l => l.Key).ToList();
            // and their number will determine the size of the side of the Pixelo board
            var size = horizontalLines.Count;
            output.AppendFormat("It is a {0}x{0} sized board\r\n", size);
            // compute the x start of the vertical values
            var lastx = horizontalLines.Max(l => l.Value.Item2);

            if (passes == 0)
            {
                //find lower left corner of board
                var x = lastx;
                var y = horizontalLines.First(hl => hl.Value.Item2 == x).Key;
                while (image.GetPixel(x, y) != Color.FromArgb(104, 104, 104)) x++;
                while (image.GetPixel(x, y) == Color.FromArgb(104, 104, 104)) y++;
                y--;
                // find the possible region covered by the keyboard indicator arrows
                x -= 170;
                y -= 114;
                for (var dx = 0; dx <= 56; dx++)
                {
                    for (var dy = 0; dy <= 56; dy++)
                    {
                        var color=image.GetPixel(x + dx, y + dy);
                        if (color.R == color.G && color.G == color.B && color.R<=145)
                        {
                            image.SetPixel(x + dx, y + dy, Color.Black);
                            passes = 1;
                        }
                    }
                }
                // if we changed anything, try to pass over it again
                if (passes > 0)
                {
                    return getPixeloValuesFromImage(image, passes);
                }
            }

            // compute the x position of columns with vertical values
            var q = (longestLine.Value.Item2 - lastx) / (double)size;
            var columnxs = Enumerable.Range(0, size).Select(i => (int)(lastx + i * q)).ToList();

            // split the patterns into individual values once again
            var numbers = patterns.SelectMany(p => p.Points.Select(pt => new
            {
                Value = p.Value,
                Point = pt,
                ColumnX = pt.X > lastx //also compute the vertical column for the ones that are beyond lastx
                    ? columnxs.Where(cx => cx <= pt.X).OrderBy(cx => pt.X - cx).First()
                    : -1
            })).ToList();

            // return the horizontal and vertical values
            var sb = new StringBuilder();
            var linetexts = new List<string>();
            foreach (var horizontalLine in horizontalLines)
            {
                var values = string.Join(" ", numbers.Where(n => n.Point.Y == horizontalLine.Key).OrderBy(n => n.Point.X).Select(n => n.Value));
                linetexts.Add(values);
            }
            var horizontalValues = string.Join(", ", linetexts);
            output.AppendFormat("Horizontal values: {0}\r\n", horizontalValues);
            sb.AppendLine(horizontalValues);
            var columntexts = new List<string>();
            foreach (var columnx in columnxs)
            {
                var values = string.Join(" ", numbers.Where(n => n.ColumnX == columnx).OrderBy(n => n.Point.Y).Select(n => n.Value));
                columntexts.Add(values);
            }
            var verticalValues = string.Join(", ", columntexts);
            sb.Append(verticalValues);
            output.AppendFormat("Vertical values: {0}\r\n", verticalValues);
            Output = output.ToString();
            return sb.ToString();
        }

        private static List<ColorPattern> extractPatterns(Bitmap image, bool[,] black, bool[,] gray)
        {
            var patterns = new List<ColorPattern>();
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    if (black[y, x])
                    {
                        var points = new List<Point>();
                        black[y, x] = false;
                        points.Add(new Point(x, y));
                        fillPoints(black, points);
                        addToPatterns(patterns, points);
                    }
                    if (gray[y, x])
                    {
                        var points = new List<Point>();
                        gray[y, x] = false;
                        points.Add(new Point(x, y));
                        fillPoints(gray, points);
                        addToPatterns(patterns, points.Select(p => new Point(p.X - 2, p.Y - 2)).ToList());
                    }
                }
            }
            return patterns;
        }

        private static void joinDigitsIntoNumbers(List<ColorPattern> patterns)
        {
            for (var i = 0; i < patterns.Count(); i++)
            {
                for (var j = 0; j < patterns.Count(); j++)
                {
                    var p1 = patterns[i];
                    var p2 = patterns[j];
                    for (var pi = 0; pi < p1.Points.Count(); pi++)
                    {
                        for (var pj = 0; pj < p2.Points.Count(); pj++)
                        {
                            var pt1 = p1.Points[pi];
                            var pt2 = p2.Points[pj];
                            if (pt1.IsEmpty || pt2.IsEmpty) continue;
                            if (i == j && pi == pj) continue;
                            if (pt1.Y == pt2.Y && pt2.X > pt1.X && pt2.X - pt1.X - p1.Pattern.GetLength(1) <= 4
                                && p1.Value == "1")
                            {
                                var exPat = patterns.FirstOrDefault(p => p.Value == p1.Value + p2.Value);
                                if (exPat == null)
                                {
                                    exPat = new ColorPattern
                                    {
                                        Value = p1.Value + p2.Value,
                                        Pattern = new bool[p1.Pattern.GetLength(0), pt2.X + p2.Pattern.GetLength(1) - pt1.X]
                                    };
                                    patterns.Add(exPat);
                                }
                                exPat.Points.Add(pt1);
                                p1.Points[pi] = Point.Empty;
                                p2.Points[pj] = Point.Empty;
                            }
                        }
                    }
                }
            }
            foreach (var pattern in patterns)
            {
                pattern.Points.RemoveAll(p => p.IsEmpty);
            }
        }

        private static unsafe void extractPixels(Bitmap image, bool[,] black, bool[,] gray)
        {
            var lockData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            var pixelSize = 4;
            for (var y = 0; y < image.Height; y++)
            {
                byte* row = (byte*)lockData.Scan0 + (y * lockData.Stride);
                for (var x = 0; x < image.Width; x++)
                {
                    var blue = row[x * pixelSize];
                    var green = row[x * pixelSize + 1];
                    var red = row[x * pixelSize + 2];

                    var blackColor = Color.FromArgb(0, 0, 0); //05,11, 31-4A, B2-B7, 6F-75-91
                    var grayColor = Color.FromArgb(0xCF, 0xCF, 0xCF);
                    var color = Color.FromArgb(red, green, blue);
                    if (colorSimilarity(color, blackColor) < 0.1) black[y, x] = true;
                    if (colorSimilarity(color, grayColor) < 0.1) gray[y, x] = true;
                }
            }
            image.UnlockBits(lockData);
        }

        private static double colorSimilarity(Color color1, Color color2)
        {
            var distance = Math.Sqrt((Math.Pow(color1.R - color2.R, 2) +
                                    Math.Pow(color1.G - color2.G, 2) +
                                    Math.Pow(color1.B - color2.B, 2)) / 3);
            return distance / 255.0;
        }

        private static void addToPatterns(List<ColorPattern> patterns, List<Point> points)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            foreach (var point in points)
            {
                if (minX > point.X) minX = point.X;
                if (maxX < point.X) maxX = point.X;
                if (minY > point.Y) minY = point.Y;
                if (maxY < point.Y) maxY = point.Y;
            }
            var pattern = new bool[maxY - minY + 1, maxX - minX + 1];
            foreach (var point in points)
            {
                pattern[point.Y - minY, point.X - minX] = true;
            }
            var existingPattern = patterns.FirstOrDefault(p => Util.PatternEquals(pattern, p.Pattern));
            if (existingPattern == null)
            {
                existingPattern = new ColorPattern
                {
                    Pattern = pattern
                };
                patterns.Add(existingPattern);
            }
            existingPattern.Points.Add(new Point(minX, minY));
        }

        private static void fillPoints(bool[,] pixels, List<Point> points)
        {
            bool again = true;
            while (again)
            {
                again = false;
                var i = 0;
                while (i < points.Count)
                {
                    var point = points[i];
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        for (var dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0)
                            {
                                if (dy == 0) continue;
                            }
                            else
                            {
                                if (dy != 0) continue;
                            }
                            if (point.X + dx >= 0 && point.X + dx < pixels.GetLength(1)
                                && point.Y + dy >= 0 && point.Y + dy < pixels.GetLength(0))
                            {
                                if (pixels[point.Y + dy, point.X + dx])
                                {
                                    points.Add(new Point(point.X + dx, point.Y + dy));
                                    pixels[point.Y + dy, point.X + dx] = false;
                                    again = true;
                                }
                            }
                        }
                    }
                    i++;
                }
            }
        }


    }
}
