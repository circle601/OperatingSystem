using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JitCompiler.MachineCode;

namespace JitCompiler
{
    class CodeGen
    {
        byte FileCompression = 1;
        const uint magicNumber = 0x1EAB11CA;

        List<lable> ClassLables = new List<lable>();

        Architecture Arch;

       public MachineCode Output = new MachineCode();

        // __declspec(align(4)) struct Obj
        //  {
        //      int length;
        //      AtomicByte Referances;
        //      Obj* Baseclass;
        //      byte Flags;
        //      char CallMin;
        //      char CallMax;
        //      char data;
        //      //calltable
        //       //data
        //   };
        //#define CallTableBytecode 1
        //       __declspec(align(4)) struct CallTableElement
        //    {
        //        unsigned int type;
        //       int ofset;
        //    };



       public CodeGen(Architecture Arch)
        {
            this.Arch = Arch;
        }

        public void BuildProgram(List<AST.Class> Parts)
        {
            foreach (var item in Parts)
            {
                Include(item.LocalName(), item);
            }
            List<AST.Class> classes = Parts.Where(x => (x.flags & (int)Opcodes.stateflags.EXTERN) == 0).ToList();



            Output.CCInt(magicNumber);
            Output.CCInt(3); // file vertion
            Output.CC(FileCompression); //filemode 0 means uncompressed
            Output.CCInt(0); // reserved
            CCSection("HEDR");
            Output.CCString("Program");
            lable redirtab = Output.CCIntL(0);// redirectionTable
            Output.CCInt((uint)classes.Count());
            lable length = Output.CCIntL(0);// length
            Output.CCString("main"); //start point
            Output.CCInt(0); // start number
            CCSection("PROG");
            foreach (var item in classes)
            {
                Output.CCString(item.LocalName());
                BuildClass(item);
            }
            CCSection("DATA");
            lable datalength = Output.CCIntL(0);// length
            Output.AlterCCInt(redirtab, (uint)Output.Length());
            CCSection("REDI");
            Output.CCInt((uint)redirectionTable.Count);
            foreach (var item in redirectionTable)
            {
                Output.CCString(item.Key);
                Output.CCInt((uint)item.Value.Id);
                Output.CC((byte)classes.FindIndex(x => ("." + x.name) == (item.Key)));
            }

            Output.AlterCCInt(length, Output.LableLength(length));



            CCSection("ENDP");


        }





        public void BuildClass(AST.Class Class)
        {
            Dictionary<string, int> itemNumber = new Dictionary<string, int>();
            int partcount = Class.Functions.Count + Class.Varables.Count;
            if (partcount > 255)
            {
                throw new CompileException("too many elements", 0, 0);
            }
            lable length = Output.CCIntL(0);// length
            Output.CCInt(0); // Space for program pointer
            Output.CC(0); // Referances
            if (Class.baseclass == null)
            {
                Output.CCInt(0);
            }
            else
            {
                Output.CCInt(ResolveName(Class.baseclass).Id); //baseclass
            }
            Output.CC((byte)Class.flags); //flags
            if (Class.baseclass == null)
            {
                int i = 0;
                Output.CC(0); //callmin
                Output.CC((byte)partcount);
                foreach (var item in Class.Functions)
                {
                    itemNumber.Add(item.name, i++);
                    lable pos = Output.CCIntL(0); // location
                    ClassLables.Add(pos);
                    Output.CCInt(Opcodes.CallTableBytecode); // type
                }
                foreach (var item in Class.Varables)
                {
                    itemNumber.Add(item.name, i++);
                    lable pos = Output.CCIntL(0); // location
                    ClassLables.Add(pos);
                    Output.CCInt(ResolveName(item.type).Id); // type
                }
            }
            else
            {
                //TODO baseclass
            }


            foreach (var item in Class.Functions)
            {
                /*    unsigned int length;
               * 	unsigned int Compiled;
               *   unsigned int ReturnType;
               *    unsigned int length;
               *   char data;
               */
                lable itemlab = Output.CCIntL(0);
                Output.AlterCCInt(ClassLables[itemNumber[item.name]], (uint)Output.Length());
                Output.CCInt(0); //Compiled
                Output.CCInt(ResolveName(item.Returntype).Id); //ReturnType

                bool notstatic = (Class != null && ((item.flags & (int)Opcodes.stateflags.STATIC) == 0));



                uint paramCount = (uint)item.parameters.Length;
                if (notstatic) paramCount++;
                Output.CCInt(paramCount); //ParamCount
                lable lab = Output.CCIntL(0);

                if (notstatic)
                {
                    Output.CCInt(ResolveName(Class.name).Id); //paramater
                }
                foreach (var param in item.parameters)
                {
                    Output.CCInt(ResolveName(param.type).Id); //paramater
                }
                BuildFunction(item, Class);
                Output.AlterCCInt(lab, Output.LableLength(lab));
                Output.AlterCCInt(itemlab, Output.LableLength(itemlab));
            }

            foreach (var item in Class.Varables)
            {
                lable itemlab = Output.CCIntL(0);
                if (item.type.Equals("int"))
                {
                    if (item.Value == null)
                    {
                        Output.CCInt(0);
                    }
                    else
                    {
                        Output.CCInt((uint)(int)(((AST.ObjectExpression)item.Value).value));
                    }
                }
                else
                {



                }
                Output.AlterCCInt(itemlab, Output.LableLength(itemlab));
            }
            Output.AlterCCInt(length, Output.LableLength(length));
        }





        public const int CallTableBool = 5;
        public const int CallTableType = 7;
        public const int CallTableArray = 8;
        public const int CallTableString = 9;
        public const int CallTableByte = 10;
        public const int CallTableInt = 12;
        public const int CallTableUInt = 13;
        public const int CallTableFloat = 14;
        public const int CallTableDouble = 15;
        public const int CallTableObject = 16;



        public void BuildFunction(AST.FunctionDeclaration func, AST.Class Class)
        {
            FunctionBuilder fb = new FunctionBuilder(Arch,this, func, Class);
            MachineCode FunctionCode = new MachineCode();
            fb.Build(FunctionCode);
            this.Output.AddMachineCode(FunctionCode);
        }




        public Dictionary<string, Redirection> redirectionTable = new Dictionary<string, Redirection>();
        uint IncludeId = 0;
        // . before name means current namespace





        public lable CCSection(string data, int ShortForm = 1)
        {
    
            int length = 4;
            if (ShortForm > 0)
            {
                length = 1;
            }
            if (data.Length > length)
            {
                data = data.Substring(0, length);
            }
            else if (data.Length < length)
            {
                data = data.PadRight(length);
            }
            return Output.CCL(ASCIIEncoding.UTF8.GetBytes(data));
        }





        public void Include(string name, AST.Class cls)
        {
            if (!redirectionTable.ContainsKey(name))
            {
                redirectionTable.Add(name, new Redirection(256 + IncludeId++, name, cls, this));
            }
        }
        public Redirection ResolveClass(AST.Class cls)
        {
            return ResolveName(cls.name);
        }

        public Redirection ResolveName(string name)
        {
            switch (name)
            {
                case ("int"): return Redirection.RedirectionInt;
                case ("void"): return Redirection.Redirectionvoid;
                case ("bool"): return Redirection.Redirectionbool;
                case ("Type"): return Redirection.RedirectionType;
                case ("string"): return Redirection.RedirectionString;
                case ("byte"): return Redirection.Redirectionbyte;
                case ("uint"): return Redirection.Redirectionuint;
                case ("float"):
                    {
                        return Redirection.Redirectionfloat;

                    }
                case ("double"):
                    {
                        return Redirection.Redirectiondouble;

                    }
                case ("object"): return Redirection.Redirectionobject;
            }
            Redirection result = null;
            if (redirectionTable.TryGetValue(name, out result))
            {
                return result;
            }
            if (redirectionTable.TryGetValue("." + name, out result))
            {
                return result;
            }
            return null;


        }
    }
}
