using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    class ArchParameter
    {
        public bool Input { get; private set; }



        public string[] AccpetableLocations { get; private set; }
         


        public static ArchParameter FromString(string text)
        {
            ArchParameter result = new ArchParameter();
            text = text.Trim();
            result.AccpetableLocations = text.Split(',');
            return result;
        }

      
    }
}
