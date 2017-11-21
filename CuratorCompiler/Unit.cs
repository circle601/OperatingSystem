using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
  public  class Unit
    {
        int minor=0;
        int major=0;
        private void StartTest(string code)
        {
            minor =1;
            major++;
            Console.WriteLine(code);
        }

        private bool Parse(string code)
        {
            Scanner scan = new JitCompiler.Scanner();
            var symbols = scan.scan(code);
            Parser par = new Parser();
            var result = par.Parse(symbols);
            Console.WriteLine("    " + major + ":" + minor++ + " Passed!");

            try
            {
               }
            catch(CompileException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("    " + major + ":" + minor++ + " Failed!");
                return false;
            }
            
            return true;
        }

        private void Expression(string code,object expected)
        {
            Scanner scan = new JitCompiler.Scanner();
            var symbols = scan.scan(code +  ";" );
            Parser par = new Parser();
            var result2 = par.ParseExpressionSolo(symbols);
            Architecture arch = new Architecture();
            CodeGen codegen = new CodeGen(arch);
       
        }

        public void Runtests()
        {

            StartTest("Expression Simplification test");
            Expression("1*10", 10);
            Expression("3*4", 12);
            Expression("3*4+2", 14);
            Expression("1|2", 3);
            Expression("\"hello\"", "hello");

            StartTest("Empty Body test");
            Parse("static class Hello {}");
            Parse("class Hello { static inline void a(void b){}}");
            Parse("class Hello { static inline void a(void b){;}}");
            Parse("class Hello { static inline void a(void b){;{}}}");
            Parse("class Hello { static inline void a(void b){;}  static inline void c(void b){;}}") ;
            Parse("class Hello { static inline void a(void b,void c,void d,void e,void g){;}  static inline void c(void b){;}}  class goodbye { static inline void a(void b,void c,void d,void e,void g){;}  static inline void c(void b){;}}");

            StartTest("Empty Body Scope test");
            Parse("class Hello { static inline void a(void b){if(){}}}");
            Parse("class Hello { static inline void a(void b){if(){}else{}}}");
            Parse("class Hello { static inline void a(void b){while(){}}}");

            StartTest("Expression parse test");
            Parse("class Hello { static inline void a(void b){if(x+4){}}}");
            Parse("class Hello { static inline void a(void b){if(-3){}else{}}}");
            Parse("class Hello { static inline void a(void b){while(-x*2+12){}}}");
            Parse("class Hello { static inline void a(void b){while((x*3)){}}}");
            Parse("class Hello { static inline void a(void b){while((-x*2+12)[7+2]){}}}");
            Parse("class Hello { static inline void a(void b){C = x >> y;}}");
            Parse("class Hello { static inline void a(void b){C = x << y;}}");

            StartTest("Statement parse test");
            Parse("class Hello { static inline void a(void b){x=5;}}");
            Parse("class Hello { static inline void a(void b){x();}}");
            Parse("class Hello { static inline void a(void b){a(1,2,3,4,5,6,6[6]);}}");
            Parse("class Hello { static inline void a(void b){a(1)=2;}}");
            Parse("class Hello { static inline void a(void b){a(1)=b(2);}}");
            Parse("class Hello { static inline void a(void b){a(1)[2];}}");
            Parse("class Hello { static inline void a(void b){(a(1))[2];}}");
            Parse("class Hello { static inline void a(void b){a();a();}}");
            Parse("class Hello { static inline void a(void b){h=a(1)[2]+1;h=a(1)[2]+1;}}");
            Parse("class Hello { static inline void a(void b){System.Console.WriteLine(\"hello\");}}");
            Parse("class Hello { static inline void a(void b){A.B.C = \"hello\";System.Console.WriteLine(\"hello\");}}");
            Parse("class Hello { static inline void a(void b){C = B as a;}}");
            Parse("class Hello { static inline void a(void b){C = x ^ y;}}");
            Parse("class Hello { static inline void a(void b){C = x & y;}}");
            Parse("class Hello { static inline void a(void b){C = x | y;}}");
            Parse("class Hello { static inline void a(void b){C = x & y;}}");

            StartTest("Comparison parse test");
            Parse("class Hello { static inline void a(void b){if(a < b){}}}");
            Parse("class Hello { static inline void a(void b){if(a > b){}}}");
            Parse("class Hello { static inline void a(void b){if(a==b){}}}");
            Parse("class Hello { static inline void a(void b){if(a >= b){}}}");
            Parse("class Hello { static inline void a(void b){if(a <= b){}}}");
            Parse("class Hello { static inline void a(void b){if(a != b){}}}");
            Parse("class Hello { static inline void a(void b){if(a is b){}}}");
            Parse("class Hello { static inline void a(void b){if(a && b){}}}");
            Parse("class Hello { static inline void a(void b){if(a || b){}}}");
            Parse("class Hello { static inline void a(void b){if(a ?? b){}}}");
            Parse("class Hello { static inline void a(void b){if(a ?? b){}}}");
            Parse("class Hello { static inline void a(void b){if(a ? b : c){}}}");
            Parse("class Hello { static inline void a(void b){if((a is b) ? A.B.C : (a && b)){}}}");

            StartTest("Assignment operators parse test");
            Parse("class Hello { static inline void a(void b){a += b;}}");
            Parse("class Hello { static inline void a(void b){a -= b;}}");
            Parse("class Hello { static inline void a(void b){a *= b;}}");
            Parse("class Hello { static inline void a(void b){a /= b;}}");
            Parse("class Hello { static inline void a(void b){a %= b;}}");
            Parse("class Hello { static inline void a(void b){a ^= b;}}");
            Parse("class Hello { static inline void a(void b){a |= b;}}");
            Parse("class Hello { static inline void a(void b){a &= b;}}");
            Parse("class Hello { static inline void a(void b){a <<= b;}}");
            Parse("class Hello { static inline void a(void b){a >>= b;}}");

            StartTest("Varable Declartation");
            Parse("class Hello { static inline int a(void b){int c = b + 5;return c;}}");

            StartTest("Return");
            Parse("class Hello { static inline int a(void b){return c + c;}}");
            Parse("class Hello { static inline int a(void b){return c + d;}}");

            StartTest("builtins");
            Parse("class Hello { static inline int a(void b){return c + c;}}");
            Parse("class Hello { static inline int a(void b){return c + d;}}");
            Console.WriteLine("testing Code gen");
          

            Console.WriteLine("Done!");
            Console.ReadLine();


        }
    }
}
