using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.GameSolver
{
    public static class Util
    {
        public static bool PatternEquals(bool[,] p1, bool[,] p2)
        {
            if (p1.GetLength(0) != p2.GetLength(0)) return false;
            if (p1.GetLength(1) != p2.GetLength(1)) return false;
            for (var y = 0; y < p1.GetLength(0); y++)
            {
                for (var x = 0; x < p1.GetLength(1); x++)
                {
                    if (p1[y, x] != p2[y, x]) return false;
                }
            }
            return true;
        }
    }
}
