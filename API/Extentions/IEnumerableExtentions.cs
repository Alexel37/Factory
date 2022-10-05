using System;
using System.Collections.Generic;

namespace API.Extentions
{
    public static class IEnumerableExtentions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if(items is null || action is null)
            {
                throw new ArgumentNullException();
            }

            foreach(var item in items)
            {
                action(item);
            }
        }
    }
}
