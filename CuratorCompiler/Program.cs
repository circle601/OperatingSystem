using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JitCompiler;
using System.IO;
namespace CodeCompiler
{
    using System.Diagnostics;


 


    class Program
    {
        static void Main(string[] args)
        {
#if false
            JitCompiler.Unit unittest = new JitCompiler.Unit();
            unittest.Runtests();
#endif
            Console.WriteLine("Compiling");
            // JitCompiler.Compiler.Compile("static class Hello { static inline int a(){int b = 5; int c = 3; int d = b + c; return d;}}");
            // JitCompiler.Compiler.Compile("static class Hello { static inline int a(void b){World.test c = b + 5;return c;}}");
            //  JitCompiler.Compiler.Compile("static class Hello { static inline int a(){int b = 5; if(b==5){ return 1;} return 2;} inline int b(){int b = 5; if(b==5){ return 1;} return 57;} static inline int r(){int b = 5; if(b==5){ return 1;} return 2;}}   class DostuffClass { public int DoSomething(){int b = 5; if(b==5){ return 1;} return 2;} }");

            // JitCompiler.Compiler.Compile("static class Hello { static inline int a(){int b = 5; if(b==5){ return 1;} return 2;}}");

            // Need to fix JitCompiler.Compiler.Compile("static class main { static int a(){DostuffClass b = new DostuffClass();  return b.DoSomething( b.DoSomething2());}  }   class DostuffClass {  public int DoSomething(int c){return c;}  public int DoSomething2(){return 2;}}");


            //     JitCompiler.Compiler.Compile("static class main { static int a(){DostuffClass b = new DostuffClass();int test1 = 1; int test2 = 2;  int c =  b.DoSomething2();  return  b.DoSomething(c);}  }   class DostuffClass {  public int DoSomething(int c){return c + c;}  public int DoSomething2(){return 45;}}");

            //   JitCompiler.Compiler.Compile("static class main { static int a(){DostuffClass b = new DostuffClass();  return b.DoSomething2() + b.DoSomething2(); }  }   class DostuffClass {  public int DoSomething(int c){return c + c;}  public int DoSomething2(){return 45;}}");


            //JitCompiler.Compiler.Compile("static class main { static int main(){return b() + b();}  static int b(){return 5;} }  ");

            // JitCompiler.Compiler.Compile("static class main { static int main(){return Math.b() +  Math.b();}   } static class Math { static int b(){return 5;} }  ");



            string filepath = @"D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\ManagedOS\";
            DirectoryInfo d = new DirectoryInfo(filepath);
            var files = d.GetFiles("*.cs").Select(x => x.FullName).ToList();

            
            JitCompiler.Compiler.CompileFileList(files);

                Console.WriteLine("Running");
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Debug\Jit.exe")
            {
                UseShellExecute = false
            };

            p.Start();
            p.WaitForExit();

            Console.WriteLine("DONE!");
            Console.ReadLine();
            
        }




    }
}
