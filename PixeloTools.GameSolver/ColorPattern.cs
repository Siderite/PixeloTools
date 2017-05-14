using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.GameSolver
{
    public class ColorPattern
    {
        public ColorPattern()
        {
            Points = new List<Point>();
        }
        public List<Point> Points { get; set; }
        public bool[,] Pattern { get; set; }
        public string Value { get; set; }
    }
}
