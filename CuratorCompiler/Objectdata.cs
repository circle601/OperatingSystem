using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    public class ObjectData
    {
        public ObjectData(int id, object decleration)
        {
            this.id = id;
            this.decleration = decleration;
        }
        public int id;
        public object decleration;
    }
}
