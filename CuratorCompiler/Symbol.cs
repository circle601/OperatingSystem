using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    public class Symbol
    {
        public Symbol(Object value, int lineNimber, int charNumber)
        {
            data = value;
            this.lineNimber = lineNimber;
            this.charNumber = charNumber;
        }
        public object data { get; set; }
        public int lineNimber { get; set; }
        public int charNumber { get; set; }

        public override string ToString()
        {
            return data.ToString();
        }
        
    }

}
