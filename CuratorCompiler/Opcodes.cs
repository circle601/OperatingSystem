using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    static class Opcodes
    {

        [Flags]
        public enum stateflags
        {
            STATIC = 1,
            PRIVATE = 2,
            PROTECTED = 4,
            PUBLIC = 2,
            FINAL = 8,
            ABSTRACT = 16,
            INLINE = 32,
            CONST = 64,
            VOLATILE = 128,
            // Flags Below this are compile side only
            x86 = 256,
            _asm = 256,
            EXTERN = 512
        }





        //CallTableElement types
        public const int CallTableNull = 0;
        public const int CallTableBytecode = 1;
        public const int CallTableProgram = 2;
        public const int CallTableSystemReserved = 3;
        public const int CallTableProgramPointer = 4;

        public const int CallTableBool = 5;
        public const int CallTableType = 7;
        public const int CallTableArray = 8;
        public const int CallTableString = 9;
        public const int CallTableByte= 10;
        public const int CallTableInt = 12;
        public const int CallTableUInt = 13;
        public const int CallTableFloat = 14;
        public const int CallTableDouble = 15;
        public const int CallTableObject = 16;
        


        public const int nop = 0x0; // // length 1
        //__________debugging
        public const int breakpoint = 0x10; // // length 1
        public const int Throw = 0x11 ; // // length 2: opcode, object to throw
        //_________Function
        public const int Return = 0x20 ; // // length 1: returns null
        public const int ReturnObject = 0x21; //  // length 2: opcode, object to return
        public const int Call = 0x22; // 
        public const int CallObject = 0x23; // // length2: opcode, object
        public const int Constant = 0x24; // // length3+: opcode,varable,Data
        public const int ConstantPool = 0x25; // //loads a public constant from the pool length2: opcode, id
        public const int CallDynamic = 0x26; // // length2: opcode, object,idvarable
        public const int TeturnConstant = 0x27;// returnConstant // length3+: opcode,varable,Data todo
        public const int CallCompileTime = 0x28; //Call(compiletime) // length 3: opcode, object, elementid  // calling a static method
        public const int CallSelf = 0x29;// Call(self compiletime) // length 3: opcode, elementid,param count, params  // calling a static method in same class








       // _________scope
        public const int scopeStart = 0x30; // // length 0
        public const int ScopeElse = 0x31; // // length 0
        public const int ScopeEnd = 0x32; // // length 0

        //_________branch
        public const int ConditionalToStart = 0x40; // 
        public const int ConditionalToEnd = 0x41; //
        public const int ConditionalToExit = 0x42; //
        public const int ToStart = 0x43; //
        public const int ToEnd = 0x44; //
        public const int ToExit = 0x45; //
       // __________Varables
        public const int CreateVar = 0x50 ; // opcode,varable,type
        public const int DisposeVar = 0x51; // // probally not supported
        public const int SetVar = 0x52; //

        public const int Null = 0x53; //
        public const int NewDynamic = 0x54 ; //
        public const int NewStatic = 0x55 ; //
        public const int Gettype = 0x56; //
        public const int GetElementStatic = 0x57; // opcode, redirection being set,index, result (compile time)
        public const int SetElementStatic = 0x58; //opcode, redirection being set,index, result (compile time)
        public const int Typecast = 0x59; //
        public const int TypecastDynamic = 0x5a ; //
        public const int GetElement = 0x5b; // opcode, object being set,,index, result (compile time)
        public const int SetElement = 0x5c ; //// opcode, object ,index, result(compile time)

        
        // __________reflection
        public const int GetBase = 0x60; //
        public const int GetElementType = 0x61; //
        public const int GetElementCount = 0x62; //
        public const int GetElementFlags = 0x63; //
        public const int checkcast = 0x64; //
        public const int instanceof = 0x65; //
        public const int GetElementPointer = 0x66; //

       // ________math
        public const int Div = 0x70; //
        public const int Mul = 0x71; //
        public const int Add = 0x72; //
        public const int Sub = 0x73; //
        public const int Negate = 0x74; //
        public const int Remainder = 0x75; //
        public const int Inc = 0x76; // // opcode varable_________testcases needed   register,memory
        public const int Dec = 0x77; //
        public const int AddConstant = 0x78; //  constant
        public const int SubConstant = 0x79; //  constant


        //_________bitwise
        public const int OR = 0x80; //
        public const int Rshift = 0x81; //
        public const int LShift = 0x82; //
        public const int Xor = 0x83; //
        public const int and = 0x84; //
        public const int nand = 0x85; // probally not supported yet
        public const int not = 0x86; //
        public const int nor = 0x87; // probally not supported yet

        // _________comparision
        public const int RefEqual = 0x91; //
        public const int Equal = 0x92; //
        public const int NotEqual = 0x93; //
        public const int Compare = 0x94; //
        public const int GtThan = 0x95; //
        public const int LsThan = 0x96; //
        public const int IsNull = 0x97; //
        public const int NotNull = 0x98; //
        public const int Zero = 0x99; //
        public const int nZero = 0x9a; //

      //  _________Sync


      //  _________Array


//_________switchtable


//_________Raw/Serilisation





    }
}
