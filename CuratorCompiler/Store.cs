using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    public class Store
    {
        public bool Assignable { get; set; }

        public class RegisterStore : Store
        {
            public string name { get; set; }
            public bool GeneralPurpose { get; set; }
        }

        public class VarableStore : Store
        {
            public string name { get; set; }
            public bool Typed { get; set; }
            public string Type { get; set; }
            public bool GeneralPurpose { get; set; }

        }
    }
}
