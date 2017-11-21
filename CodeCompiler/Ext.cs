using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCompiler
{
    static class Ext
    {
        public static IEnumerable<TR> ForEach<T, TR>(this IEnumerable<T> source, Func<T, TR> action)
        {
            foreach (T item in source)
                yield return action(item);
        }

    }
}
