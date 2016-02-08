using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Linq.Expressions;
using System;

namespace Projector
{
    public class EnumerableProjectionExpression<TSource> : ProjectionExpressionCore<TSource>
    {
        private readonly IEnumerable<TSource> queryableSource;

        public EnumerableProjectionExpression(IEnumerable<TSource> source)
        {
            queryableSource = source;
        }

        public IEnumerable<TDest> To<TDest>(string flatteningToken = null)
        {
            Func<TSource, TDest> compiledAction = GetCachedDelegate<TDest>(flatteningToken);
            if(compiledAction == null)
            {
                Expression<Func<TSource, TDest>> queryExpression = GetCachedExpression<TDest>(flatteningToken) ?? BuildExpression<TDest>(flatteningToken);

                compiledAction = queryExpression.Compile();

                string cacheKey = GetCacheKey<Func<TSource,TDest>>(flatteningToken);
                DelegateCache.TryAdd(cacheKey, compiledAction);
            }

            return queryableSource.Select(compiledAction);
        }

        public IEnumerable<TDest> To<TDest>(IEnumerable<TDest> destination, string keyMemberName = null, string flatteningToken = null, bool clearCollections = false)
        {
            if (destination != null)
            {
                Delegate compiledAction = GetCachedActionOverQueryableDestination<TDest>(keyMemberName, clearCollections, flatteningToken);

                if (compiledAction == null)
                {
                    Expression<Action<TSource,TDest>> queryExpression = GetCachedExpressionOverDestination<TDest>(keyMemberName, clearCollections, flatteningToken) ?? BuildExpressionOverDestination<TDest>(clearCollections, keyMemberName, flatteningToken);

                    ParameterExpression srcMemberExp = Expression.Parameter(queryableSource.GetType(), name: "src");

                    ParameterExpression dstMemberExp = Expression.Parameter(destination.GetType(), name: "dst");

                    Expression projectEnumerableOverEnumerableExpression = ProjectEnumerableOverEnumerable(typeof(TSource),
                                                                                                    typeof(TDest),
                                                                                                    srcMemberExp,
                                                                                                    dstMemberExp,
                                                                                                    keyMemberName,
                                                                                                    clearCollections,
                                                                                                    flatteningToken,
                                                                                                    new List<string>());

                    Type builtDelegateType = typeof(Action<,>).MakeGenericType(queryableSource.GetType(), destination.GetType());

                    LambdaExpression lambda = Expression.Lambda(builtDelegateType,projectEnumerableOverEnumerableExpression, srcMemberExp, dstMemberExp);
                    
                    compiledAction = lambda.Compile();

                    string key = GetCacheKey<Delegate>(flatteningToken,keyMemberName, clearCollections);
                    DelegateCache.TryAdd(key, compiledAction);
                }

                compiledAction.DynamicInvoke(queryableSource, destination);
            }
            else
            {
                return this.To<TDest>(flatteningToken);
            }
            return destination;
        }
    }
}