using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JitCompiler.Store;

namespace JitCompiler
{
    class Architecture
    {
        public bool vonNeumann { get; private set; }
        public bool HardwareFloat { get; private set; }
        public bool Managed { get; private set; }
        public float MaxVarablesPerFunction { get; private set; }


        List<RegisterStore> Registers;
        Dictionary<Opcode, List<ArchFunction>> Blocks = new Dictionary<Opcode, List<ArchFunction>>();

    


        public static Architecture FromDescription(string Description)
        {
            Architecture Result = new Architecture();
            return Result;


        }


        

        public void BuildFunction(List<FunctionBlock> Blocks, MachineCode Code)
        {
            foreach (var item in Blocks)
            {
                Code.CC((byte)item.code);
                foreach (var param in item.Parameters)
                {
                    if(param is string)
                    {
                        Code.CCString((string)param);
                    }else if (param is byte)
                    {
                        Code.CC((byte)param);
                    }
                    else if (param is int)
                    {
                        Code.CCNumber((int)param);
                    }
                    else if (param is uint)
                    {
                        Code.CCInt((uint)param);
                    }
                    else if (param is Varable)
                    {
                        Code.CC((byte)((Varable)param).id);
                    }
                    else if (param is Redirection)
                    {
                        Code.CCInt((uint)((Redirection)param).Id);
                    }
                    else if(param is Varable[])
                    {
                        Code.CC((byte)((Varable[])param).Length);
                        foreach (var paramitem in (Varable[])param)
                        {
                            Code.CC((byte)((Varable)paramitem).id);
                        }
                    }
                }
            }

        }


        public ArchFunction[] GetFunction(Opcode code)
        {
            //  return Blocks[code].ToArray();
            return new ArchFunction[] { new ArchFunction(code) };
        }

      


        public enum Opcode
        {
                    //CallTableElement types
        CallTableNull = 0,
        CallTableBytecode = 1,
        CallTableProgram = 2,
        CallTableSystemReserved = 3,
        CallTableProgramPointer = 4,

        CallTableBool = 5,
        CallTableType = 7,
        CallTableArray = 8,
        CallTableString = 9,
        CallTableByte = 10,
        CallTableInt = 12,
        CallTableUInt = 13,
        CallTableFloat = 14,
        CallTableDouble = 15,
        CallTableObject = 16,



        nop = 0x0, // // length 1
        //__________debugging
        breakpoint = 0x10, // // length 1
        Throw = 0x11, // // length 2: opcode, object to throw
        //_________Function
        Return = 0x20, // // length 1: returns null
        ReturnObject = 0x21, //  // length 2: opcode, object to return
        Call = 0x22, // 
        CallObject = 0x23, // // length2: opcode, object
        Constant = 0x24, // // length3+: opcode,varable,Data
        ConstantPool = 0x25, // //loads a public constant from the pool length2: opcode, id
        CallDynamic = 0x26, // // length2: opcode, object,idvarable
        TeturnConstant = 0x27,// returnConstant // length3+: opcode,varable,Data todo
        CallCompileTime = 0x28, //Call(compiletime) // length 3: opcode, object, elementid  // calling a static method
        CallSelf = 0x29,// Call(self compiletime) // length 3: opcode, elementid,param count, params  // calling a static method in same class








        // _________scope
        scopeStart = 0x30, // // length 0
        ScopeElse = 0x31, // // length 0
        ScopeEnd = 0x32, // // length 0

        //_________branch
        ConditionalToStart = 0x40, // 
        ConditionalToEnd = 0x41, //
        ConditionalToExit = 0x42, //
        ToStart = 0x43, //
        ToEnd = 0x44, //
        ToExit = 0x45, //
                                        // __________Varables
        CreateVar = 0x50, // opcode,varable,type
        DisposeVar = 0x51, // // probally not supported
        SetVar = 0x52, //

        Null = 0x53, //
        NewDynamic = 0x54, //
        NewStatic = 0x55, //
        Gettype = 0x56, //
        GetElementStatic = 0x57, // opcode, redirection being set,index, result (compile time)
        SetElementStatic = 0x58, //opcode, redirection being set,index, result (compile time)
        Typecast = 0x59, //
        TypecastDynamic = 0x5a, //
        GetElement = 0x5b, // opcode, object being set,,index, result (compile time)
        SetElement = 0x5c, //// opcode, object ,index, result(compile time)


        // __________reflection
        GetBase = 0x60, //
        GetElementType = 0x61, //
        GetElementCount = 0x62, //
        GetElementFlags = 0x63, //
        checkcast = 0x64, //
        instanceof = 0x65, //
        GetElementPointer = 0x66, //

        // ________math
        Div = 0x70, //
        Mul = 0x71, //
        Add = 0x72, //
        Sub = 0x73, //
        Negate = 0x74, //
        Remainder = 0x75, //
        Inc = 0x76, // // opcode varable_________testcases needed   register,memory
        Dec = 0x77, //
        AddConstant = 0x78, //  constant
        SubConstant = 0x79, //  constant


        //_________bitwise
        OR = 0x80, //
        Rshift = 0x81, //
        LShift = 0x82, //
        Xor = 0x83, //
        and = 0x84, //
        nand = 0x85, // probally not supported yet
        not = 0x86, //
        nor = 0x87, // probally not supported yet

        // _________comparision
        RefEqual = 0x91, //
        Equal = 0x92, //
        NotEqual = 0x93, //
        Compare = 0x94, //
        GtThan = 0x95, //
        LsThan = 0x96, //
        IsNull = 0x97, //
        NotNull = 0x98, //
        Zero = 0x99, //
        nZero = 0x9a //
    }


    }
}
