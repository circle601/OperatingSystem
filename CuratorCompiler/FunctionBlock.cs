using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    class FunctionBlock
    {
      

        public Architecture.Opcode code
        {
            get { return function.code; }
        }
        
        ArchFunction function;
        public object[] Parameters;
        public FunctionBlock(ArchFunction function,object[] Parameters)
        {
            this.function = function;
            this.Parameters = Parameters;
        }

        public override string ToString()
        {
            return code.ToString(); 
        }
    }
}
