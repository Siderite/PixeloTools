using PixeloTools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixeloTools.Solving
{
    public class PossibilitiesRecord
    {
        public int Index { get; set; }

        public List<Board.State[]> Possibilities { get; set; }
    }
}
