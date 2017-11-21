using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
     class Parser
    {
 

        List<Symbol> tokens;
        int index;
        public AST.Expression ParseExpressionSolo(List<Symbol> tokens)
        {
            this.tokens = tokens;
            index = 0;
            return ParseExpression();

        }
        public List<AST.Class> Parse(List<Symbol> tokens)
        {
            this.tokens = tokens;
            index = 0;
            List<AST.Class> output = new List<AST.Class>();
            AST.Class cls;
            do
            {
                cls = ParseClass();
                output.Add(cls);
            } while (cls != null && index< tokens.Count);
            return output;
        }

        private string ParseType()
        {
            return gettype<string>("type expected");

        }
        private string ParseName()
        {
            return gettype<string>("name expected");
        }
        private AST.Expression ParseSingleExpression()
        {
            Symbol data = tokens[index++];
            AST.Expression output = null;
            if (data.data is float)
            {
                output = new AST.ObjectExpression((float)(data.data));
            }
            if (data.data is int)
            {
                output = new AST.ObjectExpression((int)(data.data));
            }
            else if (data.data is char)
            {
                char ch = (char)(data.data);
                if (ch == ';')
                {
                    throwException("expression expected");
                }
                else
                if (ch == ')')
                {
                    index--;
                    return null;
                }
                else if (ch == '(')
                {
                    output = ParseExpression();
                    ConsumeValue<char>(')');
                }
                else if (ch == '{')
                {
                    AST.ArrayConstantExpression fexp = new AST.ArrayConstantExpression();
                    List<AST.Expression> items = new List<AST.Expression>();
                    do
                    {
                        items.Add(ParseExpression());
                    } while (OptionalConsume<char>(','));
                    ConsumeValue<char>('}');
                    fexp.expressions = items.ToArray();
                    output = fexp;
                }
                else if (ch == '-')
                {

                    AST.Expression exp = ParseExpression();
                    if (exp is AST.ObjectExpression)
                    {
                        AST.ObjectExpression fexp = (AST.ObjectExpression)exp;
                        if (fexp.value is float)
                        {
                            fexp.value = -((float)fexp.value);
                            output = fexp;
                        }
                        else
                        {
                            throw new CompileException("float expcted" + data.data.ToString() + "'", data.lineNimber, data.charNumber);
                        }
                    }
                    else
                    {
                        AST.OperatorExperssion foexp = new AST.OperatorExperssion();
                        foexp.a = new AST.ObjectExpression(0);
                        foexp.b = exp;
                        foexp.oper = "-";
                        output = foexp;
                    }
                }
            }
            else if (data.data is string)
            {
                string ch = (string)(data.data);
                if (ch.Equals("true"))
                {
                    output = new AST.ObjectExpression(true);
                }
                else if (ch.Equals("new"))
                {
                    AST.ConstructorExpression res = new AST.ConstructorExpression();
                    res.name = "new";
                    res.type = gettype<string>("type expected");
                    ConsumeValue<char>('(');

                    if (peekValue<char>() != ')')
                    {
                        List<AST.Expression> exps = new List<AST.Expression>();
                        do
                        {
                            exps.Add(ParseExpression());
                        } while (OptionalConsume<char>(','));
                        res.prams = exps.ToArray();
                    }
                    else
                    {
                        res.prams = null;
                    }
                    ConsumeValue<char>(')');

                    output = res;
                }
                else if (ch.Equals("false"))
                {
                    output = new AST.ObjectExpression(false);
                }
                else
                {
                    output = new AST.VarableExpression(ch);
                    data = tokens[index];
                    if (data.data is char)
                    {
                        if (((char)data.data) == '(')
                        {
                            ConsumeValue<char>('(');
                            AST.FunctionCallExpression temp = new AST.FunctionCallExpression(((AST.VarableExpression)output).name);
                            List<AST.Expression> exps = new List<AST.Expression>();
                            do
                            {

                                AST.Expression ex = ParseExpression();
                                if (ex == null)
                                {
                                    break;
                                }
                                else
                                {
                                    exps.Add(ex);
                                }
                            } while (OptionalConsume<char>(','));
                            ConsumeValue<char>(')');
                            temp.prams = exps.ToArray();
                            output = temp;
                        }
                    }

                }

            }
            else if (data.data is StringBuilder)
            {
                output = new AST.ObjectExpression(data.ToString());

            }
            return output;
        }

        #region Old
#if false
          private AST.Expression ParseExpression()
        {

            Queue<AST.Expression> outputQueue = new Queue<AST.Expression>();
            Stack<string> OperatorStack = new Stack<string>();
            AST.Expression output = ParseSingleExpression();
            outputQueue.Enqueue(output);
            Symbol data = tokens[index];
            while (true)
            {
                if (data.data is char) {
     
                    char op = (char)data.data;
                    if (op == ';' || op == ')') break;
                    if ("+-*/%^.".Contains(op))
                    {
                       opName =(op +"");
                        if (LongpeekValue<char>() == '=') return output;
                        index++;
                        AST.OperatorExperssion oper = new AST.OperatorExperssion();
                        oper.a = output;
                        oper.b = ParseExpression();
                        oper.oper = op.ToString();
                        output = oper;

                    }
                    else if (op == '&' | op == '|')
                    {

                        char lookahead = LongpeekValue<char>();
                        if (lookahead == '=')
                        {
                            throw new CompileException("symbol expected");
                            return output;
                        }
                        else
                        if (lookahead == op)
                        {
                            index += 2;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = lookahead.ToString() + lookahead.ToString();
                            output = oper;
                        }
                        else
                        {
                            index += 1;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = lookahead.ToString();
                            output = oper;
                        }
                    }
                    else if (op == '?')
                    {
                        char lookahead = LongpeekValue<char>();
                        if (lookahead == op)
                        {
                            index += 2;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = lookahead.ToString() + lookahead.ToString();
                            output = oper;
                        }
                        else
                        {
                            index += 1;
                            AST.Expression qexp = ParseExpression();
                            if (peekValue<char>() == ':')
                            {
                                index += 1;
                                AST.ConditionalExperssion cout = new AST.ConditionalExperssion();
                                cout.a = output;
                                cout.b = qexp;
                                cout.c = ParseExpression();
                                output = cout;
                            }
                            else
                            {
                                AST.OperatorExperssion oper = new AST.OperatorExperssion();
                                oper.a = output;
                                oper.b = qexp;
                                oper.oper = lookahead.ToString();
                                output = oper;
                            }
                        }
                    }
                    else if (op == '!')
                    {
                        char lookahead = LongpeekValue<char>();
                        if (lookahead == '=')
                        {
                            index += 2;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = "!=";
                            output = oper;
                        }
                    }
                    else if (op == '[')
                    {
                        index++;
                        AST.ArrayElementExpression aeex = new AST.ArrayElementExpression();
                        aeex.Body = output;
                        aeex.Index = ParseExpression();
                        ConsumeValue<char>(']');
                        output = aeex;
                    }
                    else if (op == '>' || op == '<')
                    {
                        char lookahead = LongpeekValue<char>();

                        if (op == lookahead)
                        {
                            index += 2;
                            if (peekValue<char>() == '=') return output;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = lookahead.ToString() + lookahead.ToString();
                            output = oper;
                        }
                        else if (lookahead == '=')
                        {
                            index += 2;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = lookahead.ToString() + "=";
                            output = oper;
                        }
                        else
                        {
                            index += 1;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = op.ToString();
                            output = oper;
                        }
                    }
                    else if (op == '=')
                    {

                        char lookahead = LongpeekValue<char>();
                        if (lookahead == '=')
                        {
                            index += 2;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = "==";
                            output = oper;
                        }
                        else
                        {
                            break;
                        }
                    }

                    else
                    {
                        break;
                    }
                }else if(data.data is string) {
                    string ops = (string)data.data;
                    if (ops.Equals("is") || ops.Equals("as"))
                    {
                        index += 1;
                        AST.OperatorExperssion oper = new AST.OperatorExperssion();
                        oper.a = output;
                        oper.b = ParseExpression();
                        oper.oper = ops;
                        output = oper;
                    }
                    else
                    {
                        break;
                    }
                }
                data = tokens[index];
            }


            return output;
        }
#endif

#endregion

        private AST.Expression ParseExpression()
        {

            Stack<AST.Expression> outputStack = new Stack<AST.Expression>();
            Stack<string> OperatorStack = new Stack<string>();
            AST.Expression output = ParseSingleExpression();
            outputStack.Push(output);
            Symbol data = tokens[index];
            string opName = "";
            int FreeCount = 1;
            while (true)
            {
                opName = "";
                if (data.data is char) {
     
                    char op = (char)data.data;
                    if (op == ';' || op == ')') break;
                    if ("+-*/%^.".Contains(op))
                    {
                        opName = (op + "");
                        if (LongpeekValue<char>() == '=') return output;
                        index++;
                    }
                    else if (op == '&' | op == '|')
                    {
                        char lookahead = LongpeekValue<char>();
                        if (lookahead == '=')
                        {
                            throwException("symbol expected");
                            return output;
                        }
                        else if (lookahead == op)
                        {
                            index += 2;
                           opName =(op + "" + op);
                        }
                        else
                        {
                            index += 1;
                           opName =(op + "");
                        }
                    }
                    else if (op == '?')
                    {
                        char lookahead = LongpeekValue<char>();
                        if (lookahead == op)
                        {
                            index += 2;
                           opName =(op + "" + op);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else if (op == '!')
                    {
                        char lookahead = LongpeekValue<char>();
                        if (lookahead == '=')
                        {
                            index += 2;
                           opName =(op + "=");
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else if (op == '[')
                    {
                        throw new NotImplementedException();
                        index++;
                        AST.ArrayElementExpression aeex = new AST.ArrayElementExpression();
                        aeex.Body = output;
                        aeex.Index = ParseExpression();
                        ConsumeValue<char>(']');
                        outputStack.Push(aeex);
                    }
                    else if (op == '>' || op == '<')
                    {
                        char lookahead = LongpeekValue<char>();

                        if (op == lookahead)
                        {
                            index += 2;
                           opName =(op + "" + op);
                        }
                        else if (lookahead == '=')
                        {
                            index += 2;
                           opName =(op + "=");
                        }
                        else
                        {
                            index += 1;
                           opName =(op + "");
                        }
                    }
                    else if (op == '=')
                    {

                        char lookahead = LongpeekValue<char>();
                        if (lookahead == '=')
                        {
                            index += 2;
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.a = output;
                            oper.b = ParseExpression();
                            oper.oper = "==";
                            output = oper;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (op == '(')
                    {
                        if (FreeCount > 0) break;
                        FreeCount++;
                        outputStack.Push(ParseSingleExpression());
                    }
                    else
                    {
                        if (FreeCount > 0) break;
                        FreeCount++;
                        outputStack.Push(ParseSingleExpression());
                    }
                }else if(data.data is string) {
                    string ops = (string)data.data;
                    if (ops.Equals("is") || ops.Equals("as"))
                    {
                        index += 1;
                      
                    }
                    else
                    {
                        if (FreeCount > 0) break;
                        FreeCount++;
                        outputStack.Push(ParseSingleExpression());
                    }
                }else
                {
                    if (FreeCount > 0) break;
                    FreeCount++;
                    outputStack.Push(ParseSingleExpression());
                }
                if(opName != "")
                {
                    FreeCount = 0;
                    if (OperatorStack.Count > 0) {
                        if (  Operator.Compare(opName, OperatorStack.Peek()))
                        {
                            AST.OperatorExperssion oper = new AST.OperatorExperssion();
                            oper.b = outputStack.Pop();
                            oper.a = outputStack.Pop();
                            oper.oper = OperatorStack.Pop();
                            outputStack.Push(oper);
                        }
                    }
                    OperatorStack.Push(opName);
                }
                data = tokens[index];
            }

            foreach (var item in OperatorStack)
            {
                AST.OperatorExperssion oper = new AST.OperatorExperssion();
                oper.b = outputStack.Pop();
                oper.a = outputStack.Pop();
                oper.oper = item;
                outputStack.Push(oper);
            }
            if(outputStack.Count > 1)
            {
                throwException("unable to process expression");
            }
            if (outputStack.Count == 0)
            {
                return null;
            }
                return outputStack.Pop();
        }

        public int ParseFlags()
        {
            int flags = 0;
     Opcodes.stateflags fla;
            while (true) {
                string peek = peekValue<string>();
                if (peek != null && Enum.TryParse(peek.ToUpper(), out fla))
                {
                    flags |= (int)fla;
                    index++;
                }
                else
                {
                    break;
                }
            }
            return flags;
        }

        


        private AST.Class ParseClass()
        {
            int funcIndex = 0;
            AST.Class output = new AST.Class();
            output.flags = ParseFlags();
            ConsumeValue<string>("class");
            output.name = ParseName();
            if (peekValue<char>() == ':')
            {
                index++;
                output.baseclass = ParseName();
            }
            ConsumeValue<char>('{');
            do
            {
                if (tokens[index].data is string)
                {
                    int flags = ParseFlags();
                    string typ = ParseType();
                    string name = ParseName();
                    if (peekValue<char>() == '(')
                    {
                        AST.FunctionDeclaration fundec = new AST.FunctionDeclaration();
                        fundec.name = name;
                        fundec.Returntype = typ;
                        fundec.flags = flags;
                        fundec.Id = funcIndex++;
                        ConsumeValue<char>('(');
                        List<AST.Parameter> paras = new List<AST.Parameter>();
                        if (peekValue<char>() != ')') { 
                     
                        do
                        {
                            AST.Parameter parameter = new AST.Parameter();
                            parameter.type = ParseType();
                            parameter.name = ParseName();
                            paras.Add(parameter);

                        } while (OptionalConsume<char>(','));
                         }
                        ConsumeValue<char>(')');
                        fundec.parameters = paras.ToArray();
                        if (peekValue<char>() == '{') {
                        
                                fundec.code = ParseScope();
                        }else if (peekValue<char>() == ';'){

                            ConsumeValue<char>(';');
                        }else
                        {
                            throwException("{ expected");
                        }
                        output.Functions.Add(fundec);
                    }
                    else
                    {
                        AST.VarableDeclaration vardec = new AST.VarableDeclaration();
                        vardec.flags = flags;
                        vardec.name = name;
                        vardec.type = typ;
                        if (peekValue<char>() == ';')
                        {
                            vardec.Value = null;
                            ConsumeValue<char>(';');
                        }
                        else if (peekValue<char>() == '=')
                        {
                            ConsumeValue<char>('=');
                            vardec.Value = ParseExpression();
                            ConsumeValue<char>(';');
                        }
                        else
                        {
                            throwException("end of line or assigntment expected");
                        }
                        output.Varables.Add(vardec);
                    }
                }
                else if (tokens[index].data is char && (char)tokens[index].data == '}')
                {

                }
                else
                {
                    throwException("unexpected symbol");
                }
            } while (peekValue<char>()!='}');

            ConsumeValue<char>('}');
            return output;
        }

        private AST.Scope ParseScope()
        {
            AST.Scope result = new AST.Scope();
            List<AST.Statement> statements = new List<AST.Statement>();
            ConsumeValue<char>('{');
            if (peekValue<char>() == '}')
            {
                ConsumeValue<char>('}');
                return null;
            }
            do{
                AST.Statement st = ParseStatement();
                if (st != null)
                {
                    statements.Add(st);
                }
            } while (peekValue<char>()!='}');
            ConsumeValue<char>('}');
            result.statements = statements.ToArray();
            return result;
        }


        private AST.Statement ParseStatement()
        {
            char peekchar = peekValue<char>();
            if ( peekchar == ';')
            {
                index++;
                return null;
            }
            AST.Statement result = null;
            string next = peekValue<string>();

            AST.ScopeStatement scopest = null;
            AST.ScopeStatement Current;
            switch (next)
            {
                case (""):
                    break;
                case (null):
                    break;
                case ("continue"):
                    {
                        AST.BuiltInStatement bis = new AST.BuiltInStatement();
                        index++;
                        bis.name = next;
                        result = bis;
                        break;
                    }
                case ("break"):
                    {
                        AST.BuiltInStatement bis = new AST.BuiltInStatement();
                        index++;
                        bis.name = next;
                        result = bis;
                        break;
                    }
                case ("if"):
                    scopest = new AST.ScopeStatement();
                    index++;
                    scopest.type = next;
                    ConsumeValue<char>('(');
                    scopest.exp = ParseExpression();
                    ConsumeValue<char>(')');
                    scopest.scope = ParseScope();
                    break;
                case ("return"):
                    {
                        AST.BuiltInStatement bis = new AST.BuiltInStatement();
                        index++;
                        bis.name = next;
                        bis.B = ParseExpression();
                        result = bis;
                        break;
                    }

                 case ("throw"):
                    {
                        AST.BuiltInStatement bis = new AST.BuiltInStatement();
                        index++;
                        bis.name = next;
                        bis.B = ParseExpression();
                        result = bis;
                        break;
                    }
                case ("do"):
                    scopest = new AST.ScopeStatement();
                    index++;
                    scopest.scope = ParseScope();
                    string dotype = peekValue<string>();
                    if(dotype.Equals("do") || dotype.Equals("untill"))
                    {
                        index++;
                        scopest.type = dotype;
                    }
                    else
                    {
                        throwException("unsupported do type");
                    }
                    ConsumeValue<char>('(');
                    scopest.exp = ParseExpression();
                    ConsumeValue<char>(')');
                    break;
                case ("while"): 
                    scopest = new AST.ScopeStatement();
                    index++;
                    scopest.type = next;
                    ConsumeValue<char>('(');
                    scopest.exp = ParseExpression();
                    ConsumeValue<char>(')');
                    scopest.scope = ParseScope();
                    break;
                case ("for"):
                    throwException("TODO");
                    break;
                case ("foreach"):
                    throwException("TODO");
                    break;
                case ("try"):
                    throwException("TODO");
                    break;
                case ("asm" ):
                    throwException("TODO");
                    break;
                default:
                    break;
            }
            if (scopest != null)
            {
                result = scopest;
                Current = scopest;
                next = peekValue<string>();
                while (next != null && next.Equals("else") && Current != null)
                {
                    index++;
                    if (peekValue<char>() == '{')
                    {
                        Current.elseScope = ParseScope();
                        Current = null;
                    }
                    else
                    {
                        Current.elseScope = ParseStatement();
                        Current = null;
                    }
                    next = peekValue<string>();
                }
                
            
            } else if (peekValue<char>() == '{')
            {
                AST.ScopeStatement resulta = new AST.ScopeStatement();
                resulta.scope = ParseScope();
                result = resulta;
            }
            else if(result==null){
                

                AST.Expression exp = ParseExpression();
                char peek = peekValue<char>();
                if(peek == 0)
                {
                    string str = peekValue<string>();
                    
                        char longpeek = LongpeekValue<char>();
                        if (longpeek == '(' || longpeek == '.' || longpeek == '<') // function call
                        {
                            throwException("todo");
                        }
                        else
                        {
                           
                            string name = peekValue<string>();
                            //if (name == null) break;
                            AST.VarableDeclaration vardec = new AST.VarableDeclaration();
                            vardec.name = name;
                        if (exp is AST.OperatorExperssion)
                        {
                            AST.OperatorExperssion exop = (AST.OperatorExperssion)exp;
                            if(exop.oper.Equals("."))
                            {
                                string path = exop.b.ToString();
                                AST.Expression part1 = exop.a;
                                while (part1 != null )
                                {
                                    path = part1.ToString() + "." + path;
                                    if(part1 is AST.OperatorExperssion && ((AST.OperatorExperssion)part1).oper.Equals("."))
                                    {
                                        part1 = ((AST.OperatorExperssion)part1).a;
                                    }
                                    else
                                    {
                                        part1 = null;
                                    }
                                }
                                vardec.type = path;


                            }
                            else
                            {
                                vardec.type = next;
                            }
                        }
                        else{
                            vardec.type = next;
                        }
                          
                            index++;
                            if (peekValue<char>() == '=')
                            {
                                index++;
                                vardec.Value = ParseExpression();
                            }
                            result = vardec;
                        }
                    
                }else
                if (peek == '=')
                {
                    index++;
                    AST.Assignment resulta = new AST.Assignment();
                    resulta.lhs = exp;
                    ConsumeValue<char>('=');
                    resulta.rhs = ParseExpression(); ;
                    result = resulta;
                }
                else if ("+-*/&|^%".Contains(peek))
                {
                    index++;
                    AST.Assignment resulta = new AST.Assignment();
                    resulta.lhs = exp;
                    ConsumeValue<char>('=');
                    AST.OperatorExperssion opex = new AST.OperatorExperssion();
                    opex.a = exp;
                    opex.b = ParseExpression();
                    opex.oper = peek.ToString() ;
                    resulta.rhs = opex;
                    result = resulta;
                }
                else if (peek == '<' || peek == '>')
                {
                    ConsumeValue<char>(peek);
                    ConsumeValue<char>(peek);
                    if (peekValue<char>() == '=')
                    {
                        ConsumeValue<char>(peek);
                        AST.Assignment resulta = new AST.Assignment();
                        resulta.lhs = exp;
                        AST.OperatorExperssion opex = new AST.OperatorExperssion();
                        opex.a = exp;
                        opex.b = ParseExpression();
                        opex.oper = peek.ToString() + peek.ToString();
                        resulta.rhs = opex;
                        result = resulta;
                    }
                }
                else
                {
                    AST.ExpStatement resultb = new AST.ExpStatement();
                    resultb.exp = exp;
                    result = resultb;
                }
                ConsumeValue<char>(';');
            }
          
            return result;
        }
      


    






#region access
        private void throwException(String message)
        {
          //  throw new CompileException(message, 0, 0);
        }

        private T peekValue<T>()
        {
            Symbol data = tokens[index];
            if (data.data is T)
            {
                return (T)(data.data);

            }
            else
            {
                return default(T);
            }
        }
        private T LongpeekValue<T>()
        {
            Symbol data = tokens[index+1];
            if (data.data is T)
            {
                return (T)(data.data);

            }
            else
            {
                return default(T);
            }
        }
        private T gettype<T>(string message)
        {
            Symbol data = tokens[index++];
            if (data.data is T)
            {
                return (T)(data.data);

            }
            else
            {
                index--;
                throw new CompileException(message + " at '" + data.data.ToString() + "'  wrong type", data.lineNimber, data.charNumber);
            }
        }

        private void ConsumeValue<T>(T expected)
        {
            Symbol data = tokens[index];
            if (data.data is T)
            {
                if (((T)(data.data)).Equals(expected))
                {
                    index++;
                }
                else
                {

                    throw new CompileException(expected.ToString() + " expected at '" + data.data.ToString() + "'  wrong type", data.lineNimber, data.charNumber);
                }

            }
            else
            {

            }
        }
        private bool OptionalConsume<T>(T expected)
        {
            Symbol data = tokens[index];
            if (data.data is T)
            {
                if (((T)(data.data)).Equals(expected))
                {
                    index++;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }



#endregion
    }
}
