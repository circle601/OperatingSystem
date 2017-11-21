using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitCompiler
{
 static  class Operator
    {
        static private int GetPrecedence(string A)
        {
            return ".!^*\\%/+-<<>>&^|&&||,".IndexOf(A);
        }
        static private bool GetAssociativityLeft(string A) //l to r
        {
            //prefix ++ and prefix –– sizeof & * + – ~ !
            //typecasts
            //? :
            //= *= /= %=  += –= <<= >>=&=  ^= |=
            //,
            //TODO
            //return true;
            return A != ".";
        }
        static public bool Compare(string A,string B)
        {
            if (GetAssociativityLeft(A))
            {
                return (GetPrecedence(A) > GetPrecedence(B));
            }else
            {
                return (GetPrecedence(A) >= GetPrecedence(B));
            }
        }

    }
}
