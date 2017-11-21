using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    class AST
    {
        
        public class Statement
        {

        }

        public class Expression
        {
           
        }


        public class ObjectExpression : Expression
        {
            public ObjectExpression() { }
            public ObjectExpression(object data) { value = data; }
            public object value;
            public override string ToString()
            {
                return value.ToString(); 
            }
        }
        
        public class ArrayConstantExpression : Expression
        {
            public Expression[] expressions;
        }
        public class ArrayElementExpression : Expression
        {
            public Expression Body;
            public Expression Index;
        }

        public class VarableExpression : Expression
        {
            public VarableExpression() { }
            public VarableExpression(string name) { this.name = name; }
            public string name;
            public override string ToString()
            {
                return name.ToString();
            }
        }



        public class ConstructorExpression : Expression
        {
            public Expression[] prams;
            public string name;
            public string type;
            public ConstructorExpression() { }
        }

        public class FunctionCallExpression : Expression
        {
            public Expression[] prams;
            public string name;
            public FunctionCallExpression() { }
            public FunctionCallExpression(string name) { this.name = name; }
        }
        public class OperatorExperssion : Expression
        {
            public OperatorExperssion() { }
            public Expression a;
            public Expression b;
            public string oper;
        }
        public class ConditionalExperssion : Expression
        {
            public ConditionalExperssion() { }
            public Expression a;
            public Expression b;
            public Expression c;
        }

        public class Scope : Statement
        {
            public  Statement[] statements;
        }


        public class ScopeStatement : Statement
        {
            public Scope scope;
            public Statement elseScope;
            public string type;
            public Expression exp;
        }

        public class AsmBlock : Statement
        {
            public string[] parts;
        }


        public class ForStatement : ScopeStatement
        {
            public Expression A;
            public Expression B;
        }

        public class BuiltInStatement : Statement
        {
            public string name;
            public Expression B;
        }


        public class ExpStatement : Statement
        {
            public Expression exp;
        }

        
        public class Assignment : Statement
        {
            public Expression lhs;
            public Expression rhs; 
        }

        public class VarableDeclaration : Statement
        {
            public int flags;
            public string type;
            public string name;
            public Expression Value;
        }

        public class Parameter
        {
            public string type;
            public string name;
        }

        public class FunctionDeclaration
        {
            public string Returntype;
            public string name;
            public Parameter[] parameters;
            public Scope code;
            public int flags;
            public int Id;
        }

        
        public class Class
        {
            public Class()
            {
                Varables = new List<VarableDeclaration>();
                Functions = new List<FunctionDeclaration>();
            }
            public int flags;
            public string name;
            public string baseclass;
            public List<VarableDeclaration> Varables;
            public List<FunctionDeclaration> Functions;
            public string LocalName()
            {
                return "." + name;
            }

            public AST.FunctionDeclaration GetFunction(string name)
            {
                AST.FunctionDeclaration fundec = Functions.SingleOrDefault(x => x.name == name);
                if (fundec == null)
                {
                    if(baseclass != null)
                    {
                      
                    }
                    else
                    {
                        return null;
                    }
                  
                }
                return fundec;
            }
        }
        

    }
}
