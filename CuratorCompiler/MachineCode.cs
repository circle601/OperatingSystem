using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    public class MachineCode
    {
        
        public MachineCode()
        {
            Output = new List<byte>(1024);
        }

        public MachineCode(byte[] Code)
        {
            Output = new List<byte>(Code);
        }

        private Dictionary<string,lable> labels = new Dictionary<string,lable>();
        private List<byte> Output;

        public class lable
        {
            public string Name;
            public int pos;

        }

        public enum LableType
        {
            ClassLable,
            FunctionLable,
            VarableLable,
            FunctionCall,
            ClassReferance,
        }

        public lable Lable()
        {
            lable output = new lable();
            output.pos = Output.Count;
            return output;
        }

        public uint LableLength(lable lab)
        {
            return (uint)(Output.Count - lab.pos);
        }


        public void AlterCC(lable lab, byte data)
        {
            int index = lab.pos;

            Output[index] = ((byte)(data & 0xFF));
        }

        public void AlterCCInt(lable lab, uint data)
        {
            int index = lab.pos;

            Output[index] = ((byte)(data & 0xFF));
            Output[index + 1] = ((byte)(data >> 8 & 0xFF));
            Output[index + 2] = ((byte)(data >> 16 & 0xFF));
            Output[index + 3] = ((byte)(data >> 24 & 0xFF));
        }

        public lable CCIntL(uint data)
        {
            lable output = new lable();
            output.pos = Output.Count;
            CCInt(data);
            return output;
        }

        public lable CCStringL(string data)
        {
            lable output = new lable();
            output.pos = Output.Count;
            CC((byte)data.Length);
            Output.AddRange(ASCIIEncoding.UTF8.GetBytes(data));
            CC(0x0);
            return output;
        }


        public void AddMachineCode(MachineCode code)
        {
            Output.AddRange(code.Output);
        }

        public void CCString(string data)
        {
            CC((byte)data.Length);
            Output.AddRange(ASCIIEncoding.UTF8.GetBytes(data));
            CC(0x0);
        }

        public lable CCL(params byte[] bytes)
        {
            lable output = new lable();
            output.pos = Output.Count;
            Output.AddRange(bytes);
            return output;
        }

        public void CC(params byte[] bytes)
        {
            Output.AddRange(bytes);
        }

        public void CCInstruction(byte Byte, int data)
        {
           Output.Add(Byte);
            CCNumber(data);
        }
        public void CCUInstruction(byte Byte, uint data)
        {
            Output.Add(Byte);
            CCInt(data);
        }

        public void CCNumber(int data)
        {
            Output.Add((byte)(data & 0xFF));
            Output.Add((byte)(data >> 8 & 0xFF));
            Output.Add((byte)(data >> 16 & 0xFF));
            Output.Add((byte)(data >> 24 & 0xFF));
           
           
        
           
        }
        public void CCInt(uint data)
        {
            Output.Add((byte)(data & 0xFF));
            Output.Add((byte)(data >> 8 & 0xFF));
            Output.Add((byte)(data >> 16 & 0xFF));
            Output.Add((byte)(data >> 24 & 0xFF));
           
        }


        public byte[] GetOutput()
        {
            return Output.ToArray();
        }

        public int Length()
        {
            return Output.Count();
        }

        public string asHex()
        {
            StringBuilder hex = new StringBuilder(Output.Count * 2);
            foreach (byte b in Output)
            {
                hex.AppendFormat("{0:x2}", b);
                hex.Append(" ");
            }
            return hex.ToString();
        }
    }
}
