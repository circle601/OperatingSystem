using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace JitCompiler
{
    public static class Compiler
    {
        static Architecture arch = new Architecture();
        public static void CompileFileList(IEnumerable<string> code)
        {
            Queue<string> Files = new Queue<string>(code);
            List<AST.Class> classes = new List<AST.Class>();
            while (Files.Count > 0)
            {
               string path = Files.Dequeue();
               classes.AddRange(ParseCode(ReadcodeCode(path)));
            }
            CodeGen cg = new CodeGen(arch);
          
          
            cg.BuildProgram(classes);
            File.WriteAllBytes("D:\\ouput.erf", cg.Output.GetOutput());
        }


        static string ReadcodeCode(string code)
        {
          return File.ReadAllText(code);
        }

        static List<AST.Class> ParseCode(string code)
        {
            Scanner scan = new JitCompiler.Scanner();
            var symbols = scan.scan(code);
            Parser par = new Parser();
            return par.Parse(symbols);
        }

        public static void Compile(string code)
        {
            var result = ParseCode(code);

            CodeGen cg = new CodeGen(arch);
            //   cg.BuildClass(result[0]);

            // cg.BuildFunction(result[0].Functions[0]);
            
        
            cg.BuildProgram(result);
            

            File.WriteAllBytes("D:\\ouput.erf", cg.Output.GetOutput());
          //  string data = PrintArray(cg.Output);
            try
            {
            }
            catch (CompileException e)
            {
             
            }
            
        }

        public static string PrintArray(IEnumerable<byte> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            bool first = true;
            foreach (var item in data)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(" , ");
                }
                sb.Append(String.Format("0x{0:X}", item));
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}
