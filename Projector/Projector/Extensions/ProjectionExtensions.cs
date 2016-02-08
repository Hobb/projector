using Projector;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


    public static class ProjectionExtensions
    {
        public static ProjectionExpression<TSource> Project<TSource>(this TSource source) 
        {
            return new ProjectionExpression<TSource>(source);
        }

        public static EnumerableProjectionExpression<TSource> ProjectEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            return new EnumerableProjectionExpression<TSource>(source);
        }

    }

