using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxPlayground
{
    public static class EnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> @enum, Action<T> @do)
        {
            foreach (var item in @enum) @do(item);
        }
    }
}
