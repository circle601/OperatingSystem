using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JitCompiler.Architecture;

namespace JitCompiler
{
    class ArchFunction
    {

        ArchParameter[] Parameters;
        public Opcode code { get; private set; }
        public bool VarableLength { get; private set; }
        public ArchFunction(Opcode code)
        {
            this.code = code;
        }

        private ArchFunction()
        {
        }

        public static ArchFunction FromString(string text)
        {
            List<ArchParameter> para = new List<ArchParameter>();
            ArchFunction result = new ArchFunction();
            text = text.Trim();
            StringBuilder sb = new StringBuilder();
            int i = -1;
            char ch;
            while ((ch=text[++i]) != ':')
            {
                sb.Append(ch);
            }
            Opcode opcode;
            if (!Opcode.TryParse(sb.ToString(),out opcode))
            {
                throw new Exception();
            }
            result.code = opcode;
            
            while (++i < text.Length)
            {
                ch = text[i];
                if (ch == '{')
                {
                    while ((ch = text[++i]) != '}')
                    {
                        sb.Append(ch);
                    }
                    para.Add( ArchParameter.FromString(sb.ToString()));
                }
                else if(ch == '*')
                {
                    result.VarableLength = true;
                }
                sb.Append(ch);

            }

            result.Parameters = para.ToArray();
            return result;
        }

    }
}
