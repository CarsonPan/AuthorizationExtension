using System.Collections.Generic;
using System.Linq;

namespace src.Core
{
    public static class EnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source==null||!source.Any();
        }

        public static T GetOrDefault<T>(this IList<T> source,int index)
        {
            if (source ==null)
            {
                return default;
            }
            if(index<source.Count)
            {
                return source[index];
            }
            return default;
        }
    }
}