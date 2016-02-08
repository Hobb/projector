using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            System.Diagnostics.Debug.WriteLine(message: "Start ForEach");
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }

