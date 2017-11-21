using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    class Varable
    {
        public ObjectReferance ParentObject;
        public bool Invalidated;
        public Varable(string name, int id, Redirection type, ObjectReferance ParentObject = null)
        {
            this.name = name;
            this.id = id;
            this.type = type;
            this.ParentObject = ParentObject;
            Invalidated = true;
        }
        public string name;
        public int id;
        public Redirection type;

        public bool isSubclass(Redirection type)
        {
            if (this.type.IsSubclass(type))
            {
                return true;
            }
            return false;
        }
    }
}
