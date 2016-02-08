using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projector
{
    public class ProjectionExpression<TSource> : ProjectionExpressionCore<TSource>
    {
        private readonly TSource source;

        public ProjectionExpression(TSource source)
        {
            this.source = source;
        }

        public TDest To<TDest>(string flatteningToken = null)
        {
                Func<TSource, TDest> compiledAction = GetCachedDelegate<TDest>(flatteningToken);

                if (compiledAction == null)
                {
                    Expression<Func<TSource, TDest>> queryExpression = GetCachedExpression<TDest>(flatteningToken) ?? BuildExpression<TDest>(flatteningToken);

                    compiledAction = queryExpression.Compile();

                    string key = GetCacheKey<Expression<Func<TSource,TDest>>>(flatteningToken);

                    DelegateCache.TryAdd(key, compiledAction);
                }

                return compiledAction.Invoke(source);
        }

        public TDest To<TDest>(TDest destination,string keyMemberName = null, string flatteningToken = null, bool clearCollections = false)
        {
            if (destination != null)
            {
                Action<TSource,TDest> compiledAction = GetCachedActionOverDestination<TDest>(keyMemberName, clearCollections, flatteningToken);

                if (compiledAction == null)
                {
                    Expression<Action<TSource, TDest>> queryExpression = GetCachedExpressionOverDestination<TDest>(keyMemberName, clearCollections, flatteningToken) ?? BuildExpressionOverDestination<TDest>(clearCollections, keyMemberName, flatteningToken);
                    
                    compiledAction = queryExpression.Compile();

                    string key = GetCacheKey<Action<TSource, TDest>>(flatteningToken, keyMemberName, clearCollections);

                    DelegateCache.TryAdd(key, compiledAction);
                }

                compiledAction.Invoke(source, destination);
            }
            else
            {
                return this.To<TDest>(flatteningToken);
            }

            return destination;
        }
    }
}

