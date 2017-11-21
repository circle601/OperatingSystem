using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    class ObjectReferance
    {
        public ObjectReferance(object Target, byte id,object typedata)
        {
            this.Target = Target;
            this.id = id;
            this.typedata = typedata;
        }
        public ObjectReferance(object Target, ObjectData data)
        {
            this.Target = Target;
            this.id = (byte)data.id;
            this.typedata = data.decleration;
        }

        public Object typedata;
       public object Target;
       public byte id;
    }
}
