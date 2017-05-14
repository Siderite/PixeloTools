using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PixeloTools.Common
{
    public class ColumnAndRowData
    {
        public ColumnAndRowData()
        {
            Rows = new List<List<int>>();
            Columns = new List<List<int>>();
        }

        public List<List<int>> Rows { get; private set; }
        public List<List<int>> Columns { get; private set; }
        public int Width
        {
            get
            {
                return Columns.Count();
            }
        }
        public int Height
        {
            get
            {
                return Rows.Count();
            }
        }

        public override string ToString()
        {
            return string.Join(", ",Rows.Select(r=>string.Join(" ",r)))+
                "\r\n"+
                string.Join(", ",Columns.Select(c=>string.Join(" ",c)));
        }

        static Regex _regValues = new Regex(@"^\s*(?<line>\d+([ ]+\d+)*)([ ]*,[ ]*(?<line>\d+([ ]+\d+)*))*[ ]*[\r\n]+[ ]*(?<column>\d+([ ]+\d+)*)([ ]*,[ ]*(?<column>\d+([ ]+\d+)*))*\s*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        static Regex _regXml = new Regex(@"\<puzzle [^\>]+\>\s*(\<puzzleData\>(?<pixel>[01])\<\/puzzleData\>\s*)+\<\/puzzle\>", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        public static ColumnAndRowData Parse(string values)
        {
            if (string.IsNullOrWhiteSpace(values)) throw new ArgumentException("Column and row data cannot be empty");
            var match = _regXml.Match(values);
            if (match.Success)
            {
                var elem = XElement.Parse(match.Value);
                var width = int.Parse(elem.Attribute("width").Value);
                var height = int.Parse(elem.Attribute("height").Value);
                var pixels = elem.Descendants("puzzleData").Select(e => e.Value).ToList();
                var board = new Board(width, height);
                for (var i = 0; i < pixels.Count; i++)
                {
                    var x = i % width;
                    var y = i / height;
                    board.Set(x, y, pixels[i] == "1"
                                        ? Board.State.Occupied
                                        : Board.State.Empty);
                }
                return board.GetData();
            }
            else
            {
                match = _regValues.Match(values);
                if (!match.Success) throw new ArgumentException("Could not understand provided values");
                var lines = match.Groups["line"].Captures.OfType<Capture>().Select(c => c.Value).ToList();
                var cols = match.Groups["column"].Captures.OfType<Capture>().Select(c => c.Value).ToList();

                return new ColumnAndRowData
                {
                    Columns = cols
                        .Select(col => col.Split(' ').Select(s => int.Parse(s)).ToList())
                        .ToList(),
                    Rows = lines
                        .Select(line => line.Split(' ').Select(s => int.Parse(s)).ToList())
                        .ToList()
                };
            }
        }
    }
}
