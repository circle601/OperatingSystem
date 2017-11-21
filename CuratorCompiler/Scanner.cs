using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
    public class Scanner
    {
        public Scanner()
        {


        }

        const string symbs = ",+-\\/{}[]();^*&~#|':=!£$%^<>";
        private List<Symbol> result;
        private int lineno;
        private int charno;
        private int index;
        private int length;
        string work;

        public List<Symbol> scan(string text)
        {
            result = new List<Symbol>();
            work = text + '\n';
            charno = 0;
            lineno = 1;
            length = work.Length;
            char ch;

            while (index < length)
            {
                ch = pop();
                if (ch == '\n')
                {
                    charno = 0;
                    lineno += 1;
                    continue;
                }
                if (char.IsWhiteSpace(ch))
                {



                }
                else if (ch == '"')
                {
                    StringBuilder sb = new StringBuilder();
                    ch = peek();
                    do
                    {
                        if (endof()) throwException("\" excpected");

                        if (ch == '\n') throwException("\" excpected");
                        Takepeek();
                        sb.Append(ch);
                        ch = peek();
                    } while (ch != '"');
                    additem(sb);
                    Takepeek();
                }
                else if (Char.IsDigit(ch))
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append(ch);
                    ch = peek();
                    bool dot = false; 
                    while (Char.IsDigit(ch) || ch == '.')
                    {
                        if (ch == '.') dot = true;
                        if (endof()) throwException("number excpected");
                        Takepeek();
                        sb.Append(ch);
                        ch = peek();
                    }
                    if (dot)
                    {
                        try
                        {
                            additem(float.Parse(sb.ToString()));
                        }
                        catch (FormatException fe)
                        {
                            throwException(fe.Message);
                        }
                    }
                    else {
                        try
                        {
                            additem(int.Parse(sb.ToString()));
                        }
                        catch (FormatException fe)
                        {
                            throwException(fe.Message);
                        }
                    }
                    

                }
                else if (char.IsLetter(ch) || ch == '_')
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append(ch);
                    ch = peek();

                    while ((ch == '_' || char.IsLetterOrDigit(ch)) && !symbs.Contains(ch))
                    {
                        if (endof()) throwException("identifyer expected");
                        Takepeek();
                        sb.Append(ch);
                        ch = peek();
                    }
                    additem(sb.ToString().ToLower());



                }
                else if (char.IsPunctuation(ch) || symbs.Contains(ch))
                {
                    additem(ch);
                }


            }

            return result;
        }








        private bool endof()
        {
            return (index >= length);
        }
        private char pop()
        {

            charno += 1;
            return work[index++];
        }


        private void Takepeek()
        {
            index += 1;
            charno += 1;
        }
        private char peek()
        {
            return work[index];
        }
        private void additem(Object Data)
        {
            result.Add(new Symbol(Data, lineno, charno));
        }
        private void throwException(String message)
        {
            throw new CompileException(message, lineno, charno);
        }
    }
}
