using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JitCompiler.MachineCode;

namespace JitCompiler.Assembler
{
    public class Assembler
    {
        Dictionary<string, lable> lables = new Dictionary<string, lable>();
        Dictionary<string, List< lable>> lablesTochange = new Dictionary<string, List<lable>>();
        string[] Lines;
        public Assembler(string[] Lines)
        {
            this.Lines = Lines;
        }
        MachineCode result;
        public MachineCode Assemble()
        {
            result = new MachineCode();

            foreach (var item in Lines)
            {
                string line = item;
                if (line.Contains(':'))
                {

                    string[] parts = line.Trim().Split(':');
                    string lable = parts[0].Trim();
                    lable Currentlable = result.Lable();
                    lables.Add(lable, Currentlable);
                    line = parts[1];

                    if (lablesTochange.ContainsKey(lable))
                    {
                        foreach (var lab in lablesTochange[lable])
                        {
                            result.AlterCC(lab, (byte)Currentlable.pos);
                        }
                        lablesTochange.Remove(lable);
                    }
                }
                line = line.Trim().ToLower();
                string[] OpcodeParts = line.Trim().Split(' ');

                string[] Endpart = OpcodeParts[1].Trim().Split(',');
                string opcodetext = OpcodeParts[0].Trim();
                if (opcodetext.Equals("in") || opcodetext.Equals("out")) opcodetext = "_" + opcodetext;
                Opcode opcodeadata = (Opcode)Enum.Parse(typeof(Opcode), (opcodetext));


                switch (opcodeadata)
                {
                    case (Opcode.CC):
                        {
                            
                            break;
                        }
                    case (Opcode.mov):
                        {
                            bool immediate = false;

                            bool DDMemA = Endpart[0].Contains('[');
                            bool DDMemB = Endpart[1].Contains('[');

                            int registerA = Dreg(Endpart[0]);
                            if (registerA == 255)
                            {
                                throw new Exception();
                            }
                            int registerB = Dreg(Endpart[1]);
                            if (registerB == 255)
                            {
                                registerB = ParseNumric(Endpart[1]);
                                immediate = true;
                            }
                            result.CC((byte)GenOpcode((byte)opcodeadata, immediate, Dreg((byte)((DDMemA ? 2 : 0) | (DDMemA ? 1 : 0)), (byte)registerA)));
                            result.CC((byte)registerB);
                            break;
                        }
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
                            result.CC((byte)GenOpcode((byte)opcodeadata, immediate, Dreg(aluop, Endpart[0])));
                            result.CC((byte)register);
                            break;
                        }
                    case (Opcode.cmp): //Two operands logic/arithmetic
                        {
                            byte aluop = 0;
                            bool immediate = false;
                            if (opcodetext.Equals("xor")) aluop = 0;
                            else if (opcodetext.Equals("cmp")) aluop = 1;

                            int register = Dreg(Endpart[1]);
                            if (register == 255)
                            {
                                register = ParseNumric(Endpart[1]);
                                immediate = true;
                            }

                            result.CC((byte)GenOpcode((byte)opcodeadata, immediate, Dreg(aluop, Endpart[0])));
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
                            result.CC((byte)GenOpcode((byte)opcodeadata, opcodetext.Equals("rol"), Dreg(aluop, Endpart[0])));
                            result.CC((byte)0);
                            break;
                        }
                    case (Opcode.jmp):
                        {
                            int Jumptype = 0;
                            bool immediate = false;
                            switch (opcodetext)
                            {
                                case ("jmp"):
                                    Jumptype = 0;break;
                                case ("jz"):
                                case ("je"):
                                    Jumptype = 1; break;
                                case ("jne"):
                                case ("jnz"):
                                    Jumptype = 1 | 8; break;
                                case ("ja"):
                                    Jumptype = 2; break;
                                case ("jae"):
                                    Jumptype = 3; break;
                                case ("jbe"):
                                    Jumptype = 2 | 8; break;
                                case ("jb"):
                                    Jumptype = 3 | 8; break;
                                case ("jc"):
                                    Jumptype = 4; break;
                                case ("jnc"):
                                    Jumptype = 4 | 8; break;
                            }
                            int register = Dreg(Endpart[0]);
                            if (register == 255)
                            {
                                register = ParseNumric(Endpart[0]);
                                immediate = true;
                            }
                            result.CC((byte)GenOpcode((byte)opcodeadata, immediate, (byte)Jumptype));
                            result.CC((byte)register);
                            break;
                        }
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


            jmp = 5,

            je = 5,
            jz = 5,
            
            jne = 5,
            jnz = 5,
            

            ja = 5,
            jae = 5,
            jbe = 5,
            jb = 5,
            jc = 5,
            jnc = 5,
            

            _out = 6,
            _in = 6,

            
            CC // for adding data

        }


        byte Dreg(byte op, byte Register)
        {
            int register = Register;
            if (register == 255) throw new Exception();
            return (byte)((op << 2) | register);
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
            Number = Number.Trim();
            byte Result;
            if( byte.TryParse(Number,out Result))
            {
                return Result;
            }
            else
            {
                 if (lables.ContainsKey(Number))
                {
                    lable lable = lables[Number];
                    return (byte)lable.pos;
                }
                else
                {
                    List<lable> resu;
                    if (lablesTochange.ContainsKey(Number))
                    {
                        resu = lablesTochange[Number];
                    }
                    else
                    {
                        resu = new List<lable>();
                        lablesTochange.Add(Number,resu);
                    }

                    resu.Add( result.Lable());
                    return 0;
                }
            }
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
