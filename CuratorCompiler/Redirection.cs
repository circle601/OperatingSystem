using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    internal class Redirection
    {
        //uint Localindex;
        public uint Id { get; private set; }
        public AST.Class Class { get; private set; }
        public string Fullname { get; private set; }
      


        Dictionary<string, ObjectData> NameResolution = new Dictionary<string, ObjectData>();
        int nameID = 0;
        public Redirection(uint id)
        {
            this.Id = id;
            this.Class = null;
        }
        public Redirection(uint id,string name, AST.Class Class, CodeGen Parent)
        {
            this.Id = id;
            this.Class = Class;
            this.Fullname = name;
            AddNames(Class, Parent);
        }

        private void AddNames(AST.Class Class, CodeGen Parent)
        {
            if (Class == null) return;
            if (Class.baseclass != null)
            {
                AddNames(Parent.ResolveName(Class.baseclass).Class, Parent);
            }
            foreach (var item in Class.Functions)
            {
                if (!NameResolution.ContainsKey(item.name))
                {
                    NameResolution.Add(item.name,new ObjectData( nameID++, item));
                }
            }
            foreach (var item in Class.Varables)
            {
                if (!NameResolution.ContainsKey(item.name))
                {
                    NameResolution.Add(item.name, new ObjectData(nameID++, item));
                }
            }
          
        }

        public bool IsSubclass(Redirection baseclass)
        {
            if (this == baseclass)
            {
                return true;
            }
            if(baseclass == Redirectionobject && baseclass.Id > 255)
            {
                return true;
            }
            return false;
        }

       
        public AST.FunctionDeclaration GetFunction(string name)
        {
           return Class.GetFunction(name);
        }

        public ObjectData GetPublicObject(string name)
        {
            ObjectData data;
           if(! NameResolution.TryGetValue(name,out data))
            {
                return null;
            }
            return data;
        }

        public override string ToString()
        {
            return GetName();
        }

        public string GetName()
        {
            switch (Id)
            {
                case (12): return "int";
                case (0): return "Void";
                case (5):return "bool";
                case (7):return "Type";
                case (9):return "String";
                case (10):return "byte";
                case (13):return "uint";
                case (14):return "float";
                case (15):return "double";
                case (16):return "object";
            }
            return Class.ToString();
        }

        public static Redirection RedirectionInt = new Redirection(12);
        public static Redirection Redirectionvoid = new Redirection(0);
        public static Redirection Redirectionbool = new Redirection(5);
        public static Redirection RedirectionType = new Redirection(7);
        public static Redirection RedirectionString = new Redirection(9);
        public static Redirection Redirectionbyte = new Redirection(10);
        public static Redirection Redirectionuint = new Redirection(13);
        public static Redirection Redirectionfloat = new Redirection(14);
        public static Redirection Redirectiondouble = new Redirection(15);
        public static Redirection Redirectionobject = new Redirection(16);
    }
}
