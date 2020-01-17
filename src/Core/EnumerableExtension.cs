using System;
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
        
        // public static bool IsExist<T>(this IEnumerable<T> source,T item,Func<T,T,bool> predicate)
        // {
        //     if(source.IsNullOrEmpty())
        //     {
        //         return false;
        //     }
        //     return source.Any(t=>predicate(t,item));
        // }

        // public static bool IsExist<T>(this IEnumerable<T> source,T item)
        // {
        //     return source.IsExist(item,(left,right)=>left.Equals(right));
        // }
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