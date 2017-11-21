using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler.Assembler
{
    class Assembler
    {
        string[] Lines;
        public Assembler(string[] Lines)
        {
            this.Lines = Lines;
        }

        public MachineCode Assemble()
        {
            MachineCode result = new MachineCode();
            foreach (var item in Lines)
            {
                string line = item;
                if (line.Contains(':'))
                {
                    string[] parts = line.Trim().Split(':');
                    string lable = parts[0];
                    result.Lable();
                    line = parts[1];
                }
                line = line.Trim().ToLower();
                string[] OpcodeParts = line.Trim().Split(' ');

                string[] Endpart = OpcodeParts[1].Trim().Split(',');
                string opcodetext = OpcodeParts[0].Trim();
                if (opcodetext.Equals("in") || opcodetext.Equals("out")) opcodetext = "_" + opcodetext;
                Opcode opcodeadata = (Opcode)Enum.Parse(typeof(Opcode), (opcodetext));


                switch (opcodeadata)
                {
                    case (Opcode.mov): 
                        throw new NotImplementedException();
                    case (Opcode.cmp): //Two operands logic/arithmetic
                    case (Opcode.add): //Two operands comparison
                        {
                            bool immediate = false;
                            int register = Dreg(Endpart[1]);
                            if(register == 255)
                            {
                                register = ParseNumric(Endpart[1]);
                                immediate = true;
                            }
                            byte aluop = 0;
                            if (opcodetext.Equals("add")) aluop = 0;
                            else if (opcodetext.Equals("sub")) aluop = 1;
                            else if (opcodetext.Equals("and")) aluop = 2;
                            else if (opcodetext.Equals("or")) aluop = 3;
                            else if (opcodetext.Equals("cmp")) aluop = 1;
                            else if (opcodetext.Equals("xor")) aluop = 0;
                            result.CC((byte)GenOpcode((byte)opcodeadata, immediate, Dreg(aluop, Endpart[1])));
                            result.CC((byte)register);
                            break;
                        }
                    case (Opcode.not): //One operand logic/arithmetic
                        {
                            byte aluop = 0;
                            if (opcodetext.Equals("not")) aluop = 0;
                            else if (opcodetext.Equals("shr ")) aluop = 1;
                            else if (opcodetext.Equals("ror")) aluop = 2;
                            else if (opcodetext.Equals("asr")) aluop = 3;
                            else if (opcodetext.Equals("rol")) aluop = 0;
                            result.CC((byte)GenOpcode((byte)opcodeadata, opcodetext.Equals("rol"), Dreg(aluop, Endpart[1])));
                            result.CC((byte)0);
                            break;
                        }
                    // case (Opcode.cmp): //Jumps
                    case (Opcode._out): // External interface (opcode 110)
                        {
                            if (opcodetext.Equals("_in")){
                                result.CC((byte)GenOpcode((byte)opcodeadata, false, Dreg(1, Endpart[1])));
                                result.CC((byte)0);
                            }
                            else if (opcodetext.Equals("_out")){
                                bool immediate = false;
                                int register = Dreg(Endpart[0]);
                                if (register == 255)
                                {
                                    register = ParseNumric(Endpart[1]);
                                    immediate = true;
                                }
                                result.CC((byte)GenOpcode((byte)opcodeadata, immediate, Dreg(0, Endpart[1])));
                                result.CC((byte)register);
                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                }
            }
            return result;
        }



        enum Opcode{
            mov = 0,
            add = 1,
            sub = 1,
            and = 1,
            or =  1,

            xor =2,
            cmp = 2,
        
            not = 3,
            shr = 3,
            ror = 3,
            asr =3,
            rol = 3,

            _out = 6,
            _in = 6,
        }


        byte Dreg(byte op,string Register)
        {
            int register = Dreg(Register);
            if (register == 255) throw new Exception();
            return (byte)((op << 2) | register);
        }

        byte GenOpcode(byte Opcode,bool immediate, byte extra)
        {
            return (byte)(((Opcode & 7) << 5) | ((immediate ? 1 : 0) << 4) | (extra & 15));
        }
        

        byte ParseNumric(string Number)
        {
            return 0;
        }

        byte Dreg(string  Register)
        {
            Register = Register.Trim().Replace("[", "").Replace("]", "");
            switch (Register)
            {
                case ("ra"): return 0;
                case ("rb"): return 1;
                case ("rc"): return 2;
                case ("rd"): return 3;
                default: return 255;
            }
        }

    }
}
