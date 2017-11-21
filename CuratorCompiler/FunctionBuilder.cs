using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JitCompiler.Architecture;

namespace JitCompiler
{
    class FunctionBuilder
    {
        static Varable NullVarable = new Varable("0%Void", 255, Redirection.Redirectionvoid);

        List<FunctionBlock> Blocks = new List<FunctionBlock>();
        AST.Class functype;
        AST.FunctionDeclaration func;
        CodeGen parent;
        Architecture Arch;




        int varid;
        int tempid = 0;
        int scopeId = -1;
        Stack<int> scopes = new Stack<int>();
        List<int> Scopecount = new List<int>();
        Dictionary<int, Varable> vars = new Dictionary<int, Varable>();
        Dictionary<string, int> names = new Dictionary<string, int>();





        List<Varable> TempVarables = new List<Varable>();


        public FunctionBuilder(Architecture Arch, CodeGen parent, AST.FunctionDeclaration func, AST.Class typ)
        {
            this.Arch = Arch;
            functype = typ;
            this.func = func;
            this.parent = parent;
        }


        public void BuildCode(Opcode code,params object[] Parameters)
        {
            //  ArchFunction[] Functions = Arch.GetFunction(code);
            FunctionBlock result = new FunctionBlock(new ArchFunction(code), Parameters);
            Blocks.Add(result);
        }


        public void Build(MachineCode Code)
        {
            scopeId++;
            Scopecount.Add(0);
            Scopecount[scopeId] = 0;
            if (functype != null && ((func.flags & (int)Opcodes.stateflags.STATIC) == 0))
            {
                DefineParameter("this", parent.ResolveName(functype.LocalName()));
            }
            foreach (var item in func.parameters)
            {
                DefineParameter(item.name, parent.ResolveName(item.type));
            }
            CreateScope();
            GenerateScope(func.code);
            UnloadScope();
            Arch.BuildFunction(Blocks, Code);
             
        }



        #region Scope

        public void ElseScope()
        {
            throw new NotImplementedException();
        }

        public void CreateScope()
        {
            scopeId++;
            Scopecount.Add(0);
            Scopecount[scopeId] = 0;
            BuildCode(Opcode.scopeStart);
        }




        private Varable ResolveVarable(string name)
        {
            try
            {
                return vars[names[name]];
            }
            catch (KeyNotFoundException knfe)
            {
                try
                {
               

                    var varb = this.functype.Varables.FirstOrDefault(x => x.name == name);
                    if (varb == null) return null;
                    Redirection VarRed = parent.ResolveName(varb.type);

                  

                    Varable Result = GetTempVarable(VarRed);
                    Result.Invalidated = false;


                    if ((varb.flags & (int)Opcodes.stateflags.STATIC) != 0)
                    {
                        Redirection thisRedirection = parent.ResolveClass(this.functype);
                        Result.ParentObject = new ObjectReferance(thisRedirection, thisRedirection.GetPublicObject(name));

                    }
                    else if ((this.func.flags & (int)Opcodes.stateflags.STATIC) == 0)
                    {
                        Varable thisvar = ResolveVarable("this");
                        if (thisvar != null)
                        {
                            Result.ParentObject = new ObjectReferance(thisvar, thisvar.type.GetPublicObject(name));
                        }
                        else
                        {
                            throw new CompileException("cannot access instante varables from a static function", 0, 0);
                        }

                    }
                    else
                    {
                        throw new CompileException("cannot access instante varables from a static function", 0, 0);
                        Result = null;
                    }

                    if (Result != null)
                    {
                        NonLocalVarable(Result);
                    }
                    return Result;

                }
                catch (Exception nfe)
                {
                    return null;
                }
            }
        }




        void NonLocalVarable(Varable Var, bool unloadOnly = false)
        {
            if (Var.ParentObject == null) return;
            ObjectReferance Parentobject = Var.ParentObject;
            object target = Parentobject.Target;
            if (Var.Invalidated)
            {
                if (target is Redirection)
                {
                    //todo fix
                    BuildCode(Opcode.SetElementStatic, (Redirection)target, Parentobject, Var);
                }
                else if (target is Varable)
                {
                    BuildCode(Opcode.SetElement, (Varable)target, Parentobject, Var);
                }
                else
                {
                    throw new CompileException("interal error", 0, 0);
                }
            }
            else if (!unloadOnly)
            {
                if (target is Redirection)
                {
                    //todo fix  
                    //// opcode, redirection being set,index, result (compile time)

                    BuildCode(Opcode.GetElementStatic, (Redirection)target, Parentobject, Var);
                }
                else if (target is Varable)
                {
                    BuildCode(Opcode.GetElementStatic, (Varable)target, Parentobject, Var);
                }
                else
                {
                    throw new CompileException("interal error", 0, 0);
                }
            }
            Var.Invalidated = false;
        }


        public void UnloadScope()
        {

            int amount = Scopecount[Scopecount.Count - 1];
            for (int i = 0; i < amount; i++)
            {
                Varable var = vars[--varid];
                NonLocalVarable(var, true);
                vars.Remove(varid);
                names.Remove(var.name);
            }
            Scopecount.RemoveAt(Scopecount.Count - 1);
            BuildCode(Opcode.ScopeEnd);
            scopeId--;
        }

        public void MemoryBarrier()
        {
            foreach (Varable var in vars.Values)
            {
                NonLocalVarable(var);
            }
        }

#endregion

#region Varables




        private Varable DefineParameter(string name, Redirection type)
        {
            return DefineVar(name, type);
        }


        private Varable DefineVar(string name, Redirection type)
        {
            int id = varid++;
            if (id > 255) throw new CompileException("too many varables", 0, 0);
            names.Add(name, id);
          
            scopes.Push(id);
            Scopecount[scopeId]++;
            Varable result = new Varable(name, id, type);
            BuildCode(Opcode.CreateVar, result, type);
            vars.Add(id, result);
            return result;
        }

        private Varable GetTempVarable(Redirection type)
        {
            //todo use cahse
            Varable varr = DefineVar("%£" + type.GetName() + "_temp_" + tempid++, type);

            
            return varr;
        }



        private void Assignvarable(Varable var, AST.Expression exp)
        {
            if (var == null)
            {
                throw new Exception();
            }
                ProcessExpression(exp, var);
        }

        private void Assignvarable(Varable Dest, Varable Scr)
        {
            BuildCode(Opcode.SetVar, Scr, Dest);
        }

        private Varable DefineVarable(AST.VarableDeclaration vardec)
        {
            Redirection red = parent.ResolveName(vardec.type);
            Varable varr = DefineVar(vardec.name, red);
            BuildCode(Opcode.CreateVar, varr, varr.type);
            if (vardec.Value != null)
            {
                Assignvarable(varr, vardec.Value);
            }
            return varr;
        }

#endregion

        private AST.FunctionDeclaration ResolveElement(string type, string name, IEnumerable<string> types)
        {
            // support polymorhism
            foreach (var item in functype.Functions)
            {
                if (item.name == name) return item;
            }
            return null;
        }


        public AST.Class ResolveType(AST.OperatorExperssion opexp)
        {
            List<string> namelist = new List<string>();
            AST.OperatorExperssion exp = opexp;
            while (exp != null)
            {
                exp = exp.a as AST.OperatorExperssion;

            }
            return null;
        }


        private AST.FunctionDeclaration ResolveFunction(string name, IEnumerable<string> types)
        {
            // support polymorhism
            foreach (var item in functype.Functions)
            {
                if (item.name == name) return item;
            }
            return null;
        }




#region Expression





        private Varable ProcessExpression(AST.Expression expression, Varable output)
        {
            if (output == null)
            {
                throw new Exception();
                return null;
            }
                output.Invalidated = true;
            AST.Expression exp = SimplifyExpression(expression);
            if (exp is AST.OperatorExperssion)
            {
                if (((AST.OperatorExperssion)exp).oper == ".")
                {

                    AST.Expression a;
                    AST.Expression b;
                    AST.Expression CurrentExp = exp;
                    StringBuilder Namw = new StringBuilder();
                    bool first = true;
                    Varable Currentvar = null;
                    Redirection CurrentType = null;
                    Varable Tempvar = output;
                    while (CurrentExp != null)
                    {

                        if (CurrentExp is AST.OperatorExperssion)
                        {
                            a = ((AST.OperatorExperssion)CurrentExp).a;
                            if (a is AST.VarableExpression)
                            {
                                if (Currentvar == null && CurrentType == null)
                                {
                                    if (first)
                                    {
                                        Varable var = ResolveVarable(a.ToString());
                                        if (var == null)
                                        {
                                            Currentvar = null;
                                            CurrentType = parent.ResolveName(a.ToString());
                                        }
                                        else
                                        {
                                            first = false;
                                            Currentvar = var;
                                            CurrentType = var.type;
                                        }
                                    }
                                    if (Currentvar == null)
                                    {
                                        string varname = a.ToString();
                                        Namw.Append(varname);
                                        Redirection red = parent.ResolveName(Namw.ToString());
                                        if (red != null)
                                        {
                                            CurrentType = red;
                                        }
                                        Namw.Append(".");
                                    }
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                    string varname = a.ToString();
                                    AST.VarableDeclaration vardec = CurrentType.Class.Varables.Single(x => x.name == varname);
                                    if (vardec == null)
                                    {
                                        CurrentType = parent.ResolveName(vardec.type);
                                    }
                                    else
                                    {
                                        throw new CompileException("could not find: " + varname, 0, 0);
                                    }
                                }
                            }
                            CurrentExp = ((AST.OperatorExperssion)CurrentExp).b;
                        }
                        else if (CurrentExp is AST.FunctionCallExpression)
                        {
                            AST.FunctionCallExpression fce = (AST.FunctionCallExpression)CurrentExp;
                            string varname = fce.name;
                            if (Currentvar != null)
                            {

                                AST.FunctionDeclaration vardec = CurrentType.GetFunction(varname);
                                if (vardec != null)
                                {
                                    Varable[] var = new Varable[fce.prams.Count()];
                                    for (int i = 0; i < var.Count(); i++)
                                    {
                                        var[i] = VarableiseExpression(fce.prams[i]);
                                    }
                                    Redirection retuntype = parent.ResolveName(vardec.Returntype);
                                    byte functionid = (byte)vardec.Id;
                                    if (output.isSubclass(retuntype) || output.isSubclass(Redirection.Redirectionvoid))
                                    {

                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }

                                    BuildCode(Opcode.Call, CurrentType, output, (byte)functionid, var);
                                    
                                    Currentvar = output;
                                    CurrentType = retuntype;
                                    CurrentExp = null;
                                }
                                else
                                {
                                    throw new CompileException("could not find: " + varname, 0, 0);
                                }
                                MemoryBarrier();
                            }
                            else if (Currentvar == null && CurrentType != null)
                            {
                                AST.FunctionDeclaration fundec = CurrentType.GetFunction(varname);
                                if (fundec != null)
                                {
                                    Varable[] var = new Varable[fce.prams.Count()];
                                    for (int i = 0; i < var.Count(); i++)
                                    {
                                        var[i] = VarableiseExpression(fce.prams[i]);
                                    }
                                    Redirection retuntype = parent.ResolveName(fundec.Returntype);
                                    byte functionid = (byte)fundec.Id;

                                    if (output.isSubclass(retuntype))
                                    {

                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }
                                    if (fundec.parameters.Length != var.Length) throw new CompileException("Incorrect Number of paramers", 0, 0);
                                    for (int i = 0; i < fundec.parameters.Length; i++)
                                    {

                                        //todo check type compatilbity
                                    }

                                    BuildCode(Opcode.CallCompileTime, CurrentType, output, (byte)functionid, var);
                                   
                                    Currentvar = output;
                                    CurrentType = retuntype;
                                    CurrentExp = null;
                                    MemoryBarrier();
                                }
                                else
                                {
                                    throw new CompileException("could not find: " + varname, 0, 0);
                                }
                            }
                            else
                            {
                                throw new CompileException("varable not found", 0, 0);
                            }
                        }
                        else
                        {
                            throw new CompileException(". expected", 0, 0);
                        }

                    }

                    if (CurrentType == null)
                    {
                        throw new CompileException("unknow type", 0, 0);
                    }
                    return Currentvar;

                }
                else
                {
                    AST.OperatorExperssion oexp = exp as AST.OperatorExperssion;
                    AST.Expression a = oexp.a;
                    AST.Expression b = oexp.b;

                 


                    switch (((AST.OperatorExperssion)exp).oper)
                    {
                        case ("*"):
                            break;
                        case ("/"):
                            break;
                        case ("<<"):
                            break;
                        case (">>"):
                            break;
                        case ("|"):
                            break;
                        case ("&"):
                            break;
                        case ("%"):
                            break;
                        case (">"):
                            BuildCode(Opcode.GtThan, VarableiseExpression(a), VarableiseExpression(b), output);
                            break;
                        case ("<"):
                            BuildCode(Opcode.LsThan, VarableiseExpression(a), VarableiseExpression(b), output);
                            break;
                        case ("<="):
                            throw new NotImplementedException();
                        case (">="):
                            
                            throw new NotImplementedException();
                        case ("=="):
                           // if (b is )// todo much todo here
                           // {
                                BuildCode(Opcode.NotNull, VarableiseExpression(a), VarableiseExpression(b), output);
                            //}
                           // else
                           // {
                                BuildCode(Opcode.Equal, VarableiseExpression(a), VarableiseExpression(b), output);
                           // }

                            break;
                        case ("!="):
                            BuildCode(Opcode.NotEqual, VarableiseExpression(a), VarableiseExpression(b), output);
                            break;
                        case ("^"):
                            break;
                        default:
                            if (oexp.oper == "+" || oexp.oper == "-")
                            {
                                string var = null;
                                string var2 = null;
                                object obj = null;

                                if (a is AST.ObjectExpression && b is AST.VarableExpression)
                                {
                                    var = b.ToString();
                                    obj = ((AST.ObjectExpression)a).value;
                                }
                                else if (b is AST.ObjectExpression && a is AST.VarableExpression)
                                {
                                    var = a.ToString();
                                    obj = ((AST.ObjectExpression)b).value;
                                }
                                else if (b is AST.VarableExpression && a is AST.VarableExpression)
                                {
                                    var = a.ToString();
                                    var2 = b.ToString();
                                }

                                if (var != null && var2 != null)
                                {
                                    if (oexp.oper == "+")
                                    {
                                       
                                        BuildCode(Opcode.Add, ResolveVarable(var), ResolveVarable(var2), output);
                                    }
                                    else if (oexp.oper == "-")
                                    {
                                         BuildCode(Opcode.Sub, ResolveVarable(var), ResolveVarable(var2), output);
                                    }
                                }
                                else
                                if (obj != null && (obj is int))
                                {
                                    if (oexp.oper == "+")
                                    {
                                        BuildCode(Opcode.AddConstant, output, (int)obj);
                                    }
                                    else if (oexp.oper == "-")
                                    {
                                        BuildCode(Opcode.SubConstant, output, (int)obj);
                                    }

                                }
                                else
                                {
                                    Varable vara = VarableiseExpression(oexp.a);
                                    Varable varb = VarableiseExpression(oexp.b);
                                    BuildCode(Opcode.Add, output, vara, varb, output);
                                    //TODO check
                                }
                            }
                            else
                            {
                                throw new CompileException("unknow operator", 0, 0);
                            }
                            break;
                    }
                }
            }
            else if (exp is AST.ConstructorExpression)
            { //todo add argument
                AST.ConstructorExpression ce = exp as AST.ConstructorExpression;

                BuildCode(Opcode.NewStatic, parent.ResolveName(ce.type), output);
            }
            else if (exp is AST.ObjectExpression)
            {
                object obj = ((AST.ObjectExpression)exp).value;
                Type typ = obj.GetType();
                
                BuildCode(Opcode.Constant, output, obj);
                

            }
            else if (exp is AST.VarableExpression)
            {
                Varable varid = ResolveVarable(exp.ToString());

                Assignvarable(varid, output);
                

            }
            else if (exp is AST.FunctionCallExpression)
            {
                AST.FunctionCallExpression fce = exp as AST.FunctionCallExpression;
                AST.FunctionDeclaration fd = ResolveFunction(fce.name, null);
                if ((functype.flags & (int)Opcodes.stateflags.STATIC) < 0)
                {
                   // Code.CC(Opcodes.CallSelf);
                    // CC((byte)fce.prams.Length); //todo
                   // Code.CC((byte)fce.prams.Length);
                    foreach (var item in fce.prams)
                    {

                    }
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotImplementedException();
                }
                throw new NotImplementedException();
                MemoryBarrier();
            }
            else
            {
                throw new NotImplementedException();

            }
            return output;
        }





        private Varable VarableiseExpression(AST.Expression exp)
        {
            exp = SimplifyExpression(exp);

            if (exp is AST.VarableExpression)
            {
                string name = exp.ToString();
                return ResolveVarable(name);

            }
            else
            {
                if (exp is AST.OperatorExperssion)
                {
                    AST.OperatorExperssion opex = (AST.OperatorExperssion)exp;
                    if (opex.oper == "++")
                    {
                        throw new NotImplementedException();
                    }
                    else if (opex.oper == "--")
                    {
                        throw new NotImplementedException();
                    }
                }

                Redirection type = TypeExpression(exp);
                Varable outp = GetTempVarable(type);
                ProcessExpression(exp, outp);
                return outp;

            }
            return null;
        }



        public Redirection TypeExpression(AST.Expression exp)
        {
            if (exp is AST.ArrayConstantExpression)
            {
                throw new NotImplementedException();
            }
            else if (exp is AST.ConditionalExperssion)
            {

                AST.Expression b = SimplifyExpression(((AST.ConditionalExperssion)exp).b);
                AST.Expression c = SimplifyExpression(((AST.ConditionalExperssion)exp).c);
                Redirection tb = TypeExpression(b);
                Redirection tc = TypeExpression(c);
                if (tb.Equals(tc))
                {
                    return tb;
                }
                else
                {
                    throw new CompileException("type mismatch", 0, 0);
                }
            }
            else if (exp is AST.ObjectExpression)
            {
                object value = ((AST.ObjectExpression)exp).value;
                if (value is int)
                {
                    return Redirection.RedirectionInt;
                }
                if (value is string)
                {
                    return Redirection.RedirectionString;
                }
                if (value is float)
                {
                    return Redirection.Redirectionfloat;
                }
                if (value is double)
                {
                    return Redirection.Redirectiondouble;
                }

                return parent.ResolveName(((AST.ObjectExpression)exp).value.GetType().ToString());
            }
            else if (exp is AST.VarableExpression)
            {
                return ResolveVarable(((AST.VarableExpression)exp).name).type;
            }
            else if (exp is AST.ConstructorExpression)
            { //todo add argument
                AST.ConstructorExpression ce = exp as AST.ConstructorExpression;
                return parent.ResolveName(ce.type);

            }
            else if (exp is AST.OperatorExperssion)
            {
                if (((AST.OperatorExperssion)exp).oper == ".")
                {
                    AST.Expression a;
                    AST.Expression b;
                    AST.Expression CurrentExp = exp;
                    Redirection CurrentType = null;
                    StringBuilder Namw = new StringBuilder();
                    bool first = true;
                    while (CurrentExp != null)
                    {

                        if (CurrentExp is AST.OperatorExperssion)
                        {
                            a = ((AST.OperatorExperssion)CurrentExp).a;
                            if (a is AST.VarableExpression)
                            {
                                if (CurrentType == null)
                                {
                                    if (first)
                                    {

                                        Varable var = ResolveVarable(a.ToString());
                                        if (var == null)
                                        {
                                            CurrentType = parent.ResolveName(a.ToString());
                                        }
                                        else
                                        {
                                            CurrentType = var.type;
                                            first = false;
                                        }

                                    }
                                    if (CurrentType != null)
                                    {
                                        string varname = a.ToString();
                                        Namw.Append(varname);
                                        Redirection red = parent.ResolveName(Namw.ToString());
                                        if (red != null)
                                        {
                                            CurrentType = red;
                                        }
                                        Namw.Append(".");
                                    }



                                }
                                else
                                {
                                    string varname = a.ToString();
                                    AST.VarableDeclaration vardec = CurrentType.Class.Varables.Single(x => x.name == varname);
                                    if (vardec == null)
                                    {
                                        CurrentType = parent.ResolveName(vardec.type);
                                    }
                                    else
                                    {
                                        throw new CompileException("could not find: " + varname, 0, 0);
                                    }
                                }
                            }
                            CurrentExp = ((AST.OperatorExperssion)CurrentExp).b;
                        }
                        else if (CurrentExp is AST.FunctionCallExpression)
                        {
                            if (CurrentType != null)
                            {
                                AST.FunctionCallExpression fce = (AST.FunctionCallExpression)CurrentExp;
                                string varname = fce.name;
                                AST.FunctionDeclaration vardec = CurrentType.Class.Functions.Single(x => x.name == varname);
                                if (vardec != null)
                                {
                                    CurrentType = parent.ResolveName(vardec.Returntype);
                                    CurrentExp = null;
                                }
                                else
                                {
                                    throw new CompileException("could not find: " + varname, 0, 0);
                                }
                            }
                        }
                        else
                        {
                            throw new CompileException(". expected", 0, 0);
                        }

                    }



                    if (CurrentType == null)
                    {
                        throw new CompileException("unknow type", 0, 0);
                    }
                    return CurrentType;

                    return null;
                }
                else
                {
                    AST.Expression a = SimplifyExpression(((AST.OperatorExperssion)exp).a);
                    AST.Expression b = SimplifyExpression(((AST.OperatorExperssion)exp).b);
                    Redirection ta = TypeExpression(a);
                    Redirection tb = TypeExpression(b);
                    if (tb.Equals(ta))
                    {
                        return ta;
                    }
                    else
                    {
                        throw new CompileException("type mismatch", 0, 0);
                    }
                }
            }
            else if (exp is AST.FunctionCallExpression)
            {
                AST.FunctionCallExpression fce = exp as AST.FunctionCallExpression;
                AST.FunctionDeclaration fd = ResolveFunction(fce.name, null);
                return parent.ResolveName(fd.Returntype);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        //todo add boolean simplification
        public AST.Expression SimplifyExpression(AST.Expression exp)
        {
            if (exp is AST.ArrayConstantExpression)
            {
                return exp;
            }
            else if (exp is AST.ArrayElementExpression)
            {
                AST.Expression body = SimplifyExpression(((AST.ArrayElementExpression)exp).Body);
                AST.Expression index = SimplifyExpression(((AST.ArrayElementExpression)exp).Index);
                ((AST.ArrayElementExpression)exp).Body = body;
                ((AST.ArrayElementExpression)exp).Index = index;
                if (body is AST.ArrayConstantExpression)
                {
                    if (index is AST.ObjectExpression)
                    {
                        int? value = ((AST.ObjectExpression)index).value as int?;

                        if (value != null)
                        {
                            int realindex = (int)value;
                            body = ((AST.ArrayConstantExpression)body).expressions[realindex];
                            return body;
                        }
                    }
                }
                return exp;
            }
            else if (exp is AST.ConditionalExperssion)
            {
                AST.Expression a = SimplifyExpression(((AST.ConditionalExperssion)exp).a);
                AST.Expression b = SimplifyExpression(((AST.ConditionalExperssion)exp).b);
                AST.Expression c = SimplifyExpression(((AST.ConditionalExperssion)exp).c);
                ((AST.ConditionalExperssion)exp).a = a;
                ((AST.ConditionalExperssion)exp).b = b;
                ((AST.ConditionalExperssion)exp).c = c;
                if (a is AST.ObjectExpression)
                {
                    bool? res = ((AST.ObjectExpression)a).value as bool?;
                    if (res.HasValue && res.Value)
                    {
                        return b;
                    }
                    else if (res.HasValue && !res.Value)
                    {
                        return c;
                    }
                }
                return exp;

            }
            else if (exp is AST.FunctionCallExpression)
            {
                return exp;
            }
            else if (exp is AST.ObjectExpression)
            {
                return exp;
            }
            else if (exp is AST.VarableExpression)
            {
                return exp;
            }
            else if (exp is AST.ConstructorExpression)
            { //todo add argument
                return exp;
            }
            else if (exp is AST.OperatorExperssion)
            {
                AST.Expression a = SimplifyExpression(((AST.OperatorExperssion)exp).a);
                AST.Expression b = SimplifyExpression(((AST.OperatorExperssion)exp).b);
                ((AST.OperatorExperssion)exp).a = a;
                ((AST.OperatorExperssion)exp).b = b;
                if (a is AST.ObjectExpression && b is AST.ObjectExpression)
                {
                    object avalue = ((AST.ObjectExpression)a).value;
                    object bvalue = ((AST.ObjectExpression)b).value;

                    if ((avalue is int) && (bvalue is int))
                    {
                        switch (((AST.OperatorExperssion)exp).oper)
                        {
                            case ("*"):
                                return new AST.ObjectExpression((int)avalue * (int)bvalue);
                            case ("+"):
                                return new AST.ObjectExpression((int)avalue + (int)bvalue);
                            case ("-"):
                                return new AST.ObjectExpression((int)avalue - (int)bvalue);
                            case ("/"):
                                return new AST.ObjectExpression((int)avalue / (int)bvalue);
                            case ("<<"):
                                return new AST.ObjectExpression((int)avalue << (int)bvalue);
                            case (">>"):
                                return new AST.ObjectExpression((int)avalue >> (int)bvalue);
                            case ("|"):
                                return new AST.ObjectExpression((int)avalue | (int)bvalue);
                            case ("&"):
                                return new AST.ObjectExpression((int)avalue | (int)bvalue);
                            case ("%"):
                                return new AST.ObjectExpression((int)avalue % (int)bvalue);
                            case (">"):
                                return new AST.ObjectExpression((int)avalue > (int)bvalue);
                            case ("<"):
                                return new AST.ObjectExpression((int)avalue < (int)bvalue);
                            case ("<="):
                                return new AST.ObjectExpression((int)avalue <= (int)bvalue);
                            case (">="):
                                return new AST.ObjectExpression((int)avalue >= (int)bvalue);
                            case ("=="):
                                return new AST.ObjectExpression((int)avalue == (int)bvalue);
                            case ("!="):
                                return new AST.ObjectExpression((int)avalue != (int)bvalue);
                            case ("^"):
                                return new AST.ObjectExpression((int)avalue ^ (int)bvalue);
                            default: return exp;
                        }
                    }
                }
                return exp;
            }
            else
            {
                throw new CompileException("unknow operator", 0, 0);
            }
            return exp;
        }
#endregion Expression





        public void GenerateScope(AST.Scope scope)
        {

            foreach (var item in scope.statements)
            {
                if (item is AST.Assignment)
                {
                    AST.Assignment vardec = (AST.Assignment)item;
                    Assignvarable(VarableiseExpression(vardec.lhs), vardec.rhs);
                }
                else if (item is AST.ExpStatement)
                {
                    AST.ExpStatement vardec = (AST.ExpStatement)item;
                    Assignvarable(NullVarable, vardec.exp);
                }
                else if (item is AST.ForStatement)
                {
                    CreateScope();
                    AST.ForStatement vardec = (AST.ForStatement)item;
                    CreateScope();
                    GenerateScope(vardec.scope);
                    UnloadScope();
                    throw new NotImplementedException();

                }
                else if (item is AST.ScopeStatement)
                {

                    AST.ScopeStatement vardec = (AST.ScopeStatement)item;
                    CreateScope();
                    if (vardec.type == null)
                    {

                    }
                    else
                    {
                        switch (vardec.type)
                        {
                            case ("if"):
                                {
                                    Varable var = VarableiseExpression(vardec.exp);

                                    throw new NotImplementedException("if");
                                    BuildCode(Opcode.ConditionalToEnd, var);
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                    }
                    GenerateScope(vardec.scope);
                    if (vardec.elseScope != null)
                    {
                        ElseScope();
                        throw new NotImplementedException();
                        // GenerateScope(vardec.elseScope);
                    }
                    if (vardec.type == null)
                    {

                    }
                    else
                    {

                    }
                    UnloadScope();


                }
                else if (item is AST.VarableDeclaration)
                {
                    AST.VarableDeclaration vardec = (AST.VarableDeclaration)item;
                    DefineVarable(vardec);
                }
                else if (item is AST.BuiltInStatement)
                {
                    AST.BuiltInStatement vardec = (AST.BuiltInStatement)item;
                    string text = vardec.name;
                    switch (text)
                    {
                        case ("return"):
                            {
                                MemoryBarrier();
                                Varable var = VarableiseExpression(vardec.B);
                                if (var == null)
                                {
                                    throw new CompileException("unable to resolve expression", 0, 0);
                                }

                                if (var.isSubclass(parent.ResolveName(func.Returntype)))
                                {
                                    BuildCode(Opcode.ReturnObject, var);
                                }
                                else
                                {
                                    throw new CompileException("invlaid return type", 0, 0);
                                }
                                break;

                            }
                        case ("break"):
                            {
                                BuildCode(Opcode.ToEnd);
                                break;
                            }
                        case ("continue"):
                            {
                                BuildCode(Opcode.ToStart);
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (item is AST.AsmBlock)
                {
                    AST.BuiltInStatement vardec = (AST.BuiltInStatement)item;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }







    }
}
