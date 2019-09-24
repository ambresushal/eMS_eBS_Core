using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.repository.extensions
{
    public static class TreeExtension
    {
        public static IEnumerable<T> DepthFirst<T>(this T head, Func<T, IEnumerable<T>> childrenFunc)
        {
            yield return head;
            foreach (var node in childrenFunc(head))
            {
                foreach (var child in DepthFirst(node, childrenFunc))
                {
                    yield return child;
                }
            }
        }
    }
}
