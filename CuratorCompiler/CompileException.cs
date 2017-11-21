using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    public class CompileException : Exception
    {
        int LineNumber, charNumber;
        public CompileException()
        {
        }

        public CompileException(string message,int LineNumber,int charNumber)
            : base(message)
        {
            this.LineNumber = LineNumber;
            this.charNumber = charNumber;
        }
        
    }
}


