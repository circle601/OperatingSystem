using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JitCompiler.MachineCode;

namespace JitCompiler
{
    class ClassBuilder
    {

        
        MachineCode Output;
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
    }
}
