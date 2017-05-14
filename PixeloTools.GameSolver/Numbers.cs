using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.GameSolver
{
    public class Numbers
    {
        public static string Get(bool[,] pattern)
        {
            var pairs = dict.Where(p => Util.PatternEquals(pattern, p.Key)).ToList();
            return pairs.Any() ? pairs.First().Value : null;
        }

        static Dictionary<bool[,], string> dict = new Dictionary<bool[,], string>
        {
            //small numbers
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true}
}
,"0"},
{new bool[,]{
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true},
{true, true, true, true}
}
,"1"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true}
},"2"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true}
}
,"3"},
{new bool[,]{
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true}
}
,"4"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true}
}
,"5"},
{new bool[,]{
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true}
}
,"6"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true}
},"7"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true}
}
,"8"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true}
}
,"9"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true, true, true, true, true, false, false, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, false, false, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, false, false, true, true, true, true, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true}
}
,"20"},
// big numbers
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, false, false, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true}
}
,"2"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false}
}
,"3"},
{new bool[,]{
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, false, false, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, true, true, true, true, false, false},
{false, false, false, false, false, false, true, true, true, true, false, false},
{false, false, false, false, false, false, true, true, true, true, false, false},
{false, false, false, false, false, false, true, true, true, true, false, false}
}
,"4"},
{new bool[,]{
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false}
}
,"5"},
{new bool[,]{
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, false, false, false, false, false, false, false, false},
{true, true, true, true, false, false, false, false, false, false, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false}
}
,"6"},
{new bool[,]{
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false}
}
,"8"},
{new bool[,]{
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, false, false, false, false, false, false, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false}
}
,"9"},
{new bool[,]{
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, false, false, false, false, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{true, true, true, true, true, true, true, true, true, true, true, true},
{false, false, true, true, true, true, true, true, true, true, false, false},
{false, false, true, true, true, true, true, true, true, true, false, false}
}
,"0"}
        };

    }
}
