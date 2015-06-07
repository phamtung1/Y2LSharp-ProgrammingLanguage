using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    class EvalException : Exception
    {
        public int Line;
        public int Col;
        public EvalException(string message, int line, int col)
            : base(message)
        {
            this.Line = line;
            this.Col = col;
        }
    }
}
