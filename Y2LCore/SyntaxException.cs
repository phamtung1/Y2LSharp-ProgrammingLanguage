using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore
{
    public class SyntaxError
    {
        public string Message;
        public int Line;
        public int Col;
        public SyntaxError(string message, int line, int col)
        {
            this.Message = message;
            this.Line = line;
            this.Col = col;
        }
        public override string ToString()
        {
            return Message + " line: " + Line + " col: " + Col;
        }
    }

}
