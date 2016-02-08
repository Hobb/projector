using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace Projector.Helpers
{
    internal static class ExpressionHelper
    {

        /// <summary>
        /// WORKAROUND: To build a Expression.Condition, both branches have to have the same type.
        ///             Because we are using Expression.Block, the type of the Block gets determined by the type of the
        ///             last expression in the enumerable, beats me why did they follow that approach, you can check the source code here
        ///             http://referencesource.microsoft.com/System.Core/R/f6653beeb1036346.html
        ///             To work around this limitation we add a silly comparison to both blocks so they apparently match in type.
        /// </summary>
        /// <param name="expressions">Expression array to create the block</param>
        /// <returns>Expression block with its type set to boolean</returns>
        internal static BlockExpression ExpressionBlockHelper(params Expression[] expressions)
        {
            Expression dummyExpression = Expression.Equal(Expression.Constant(value: true, type: typeof(bool)), Expression.Constant(value: true, type: typeof(bool)));

            List<Expression> expressionList = expressions.ToList();
            expressionList.Add(dummyExpression);

            return Expression.Block(expressionList);
        }

        /// <summary>
        /// WORKAROUND: To build a Expression.Condition, both branches have to have the same type.
        ///             Because we are using Expression.Block, the type of the Block gets determined by the type of the
        ///             last expression in the enumerable, beats me why did they follow that approach, you can check the source code here
        ///             http://referencesource.microsoft.com/System.Core/R/f6653beeb1036346.html
        ///             To work around this limitation we add a silly comparison to both blocks so they apparently match in type.
        /// </summary>
        /// <param name="expressions">Expression list to create the block</param>
        /// <returns>Expression block with its type set to boolean</returns>
        internal static BlockExpression ExpressionBlockHelper(IEnumerable<Expression> expressions, IEnumerable<ParameterExpression> variables = null)
        {
            Expression dummyExpression = Expression.Equal(Expression.Constant(value: true, type: typeof(bool)), Expression.Constant(value: true, type: typeof(bool)));

            List<Expression> expressionList = expressions.ToList();
            expressionList.Add(dummyExpression);
            if (variables == null)
            {
                return Expression.Block(expressionList);
            }
            else
            {
                return Expression.Block(variables, expressionList);
            }
        }

        /// <summary>
        /// Resulting expression: memberExp == null
        /// </summary>
        /// <param name="memberExp"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Expression GetCompareToNullExpression(Expression memberExp, Type type)
        {
            return Expression.Equal(memberExp, Expression.Constant(value: null, type: type));
        }

        /// <summary>
        /// Resulting expression: source.ToList()
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetToListOverEnumerable(Type sourceType, Expression source)
        {
            MethodInfo toListMethod = typeof(Enumerable).GetRuntimeMethods()
                                                        .FirstOrDefault(c => c.Name == "ToList");

            MethodCallExpression listExpression = Expression.Call(
                instance: null,
                method: toListMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arguments: new System.Linq.Expressions.Expression[] { source });

            return listExpression;
        }

        /// <summary>
        /// Resulting expression: source.Clear()
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetClearOverICollection(Type sourceType, Expression source)
        {
            MethodInfo clearMethod = typeof(ICollection<>).MakeGenericType(sourceType)
                                                        .GetRuntimeMethods()
                                                        .FirstOrDefault(c => c.Name == "Clear");

            MethodCallExpression clearExpression = Expression.Call(
                source,
                clearMethod);

            return clearExpression;
        }

        /// <summary>
        /// Resulting expression: source.Project()
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetProject(Type sourceType, Expression source)
        {
            MethodInfo projectMethod = typeof(ProjectionExtensions).GetRuntimeMethods()
                                                                   .FirstOrDefault(c => c.Name == "Project");

            MethodCallExpression projectExpression = Expression.Call(
                instance: null,
                method: projectMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arguments: new System.Linq.Expressions.Expression[] { source });

            return projectExpression;
        }

        /// <summary>
        /// Resulting expression: left == right
        /// </summary>
        /// <param name="leftSideType"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetEquals(Expression left, Expression right, Type leftSideType = null)
        {
            if(leftSideType == null)
            {
                leftSideType = typeof(object);
            }

            MethodInfo equalsMethod = leftSideType.GetRuntimeMethods()
                                                  .FirstOrDefault(c => c.Name == "Equals");

            MethodCallExpression projectExpression = Expression.Call(
                left,
                equalsMethod,
                Expression.Convert( right,leftSideType));

            return projectExpression;
        }

        /// <summary>
        /// Resulting expression: source.To<destinationType>()
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="source"></param>
        /// <param name="flatteningToken"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetProjectionExpressionTo(Type sourceType, Type destinationType, Expression source, string flatteningToken)
        {
            MethodInfo projectExpressionToMethod = typeof(ProjectionExpression<>).MakeGenericType(sourceType)
                                                                                 .GetRuntimeMethods()
                                                                                 .FirstOrDefault(c => c.Name == "To" &&
                                                                                                      c.GetParameters().Count() == 1);

            ConstantExpression flatteningTokenAsExpression = Expression.Constant(flatteningToken, typeof(string));

            MethodCallExpression projectExpressionToExpression = Expression.Call(
                source,
                projectExpressionToMethod.MakeGenericMethod(new Type[]
                {
                    destinationType
                }),
                flatteningTokenAsExpression);

            return projectExpressionToExpression;
        }

        /// <summary>
        /// Resulting expression: source.To<destinationType>(destination)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="clearCollections"></param>
        /// <param name="flatteningToken"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetProjectionOverDestinationExpressionTo(Type sourceType, Type destinationType, Expression source, Expression destination, string keyMemberName, bool clearCollections, string flatteningToken)
        {
            MethodInfo projectExpressionToMethod = typeof(ProjectionExpression<>).MakeGenericType(sourceType)
                                                                                 .GetRuntimeMethods()
                                                                                 .FirstOrDefault(c => c.Name == "To" &&
                                                                                                      c.GetParameters().Count() == 4);

            ConstantExpression keyMemberNameTokenAsExpression = Expression.Constant(keyMemberName, typeof(string));
            ConstantExpression flatteningTokenAsExpression = Expression.Constant(flatteningToken, typeof(string));
            ConstantExpression clearCollectionAsExpression = Expression.Constant(clearCollections, typeof(bool));

            MethodCallExpression projectExpressionToExpression = Expression.Call(
                source,
                projectExpressionToMethod.MakeGenericMethod(new Type[]
                {
                    destinationType
                }),
                destination,
                keyMemberNameTokenAsExpression,
                flatteningTokenAsExpression,
                clearCollectionAsExpression);

            return projectExpressionToExpression;
        }

        /// <summary>
        /// Resulting expression: source.Add(item)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetAddItemByMethodName(Type sourceType, Expression source, Expression item)
        {
            MethodInfo addMethod = source.Type.GetRuntimeMethods()
                                                        .FirstOrDefault(c => c.Name == "Add");

            MethodCallExpression addExpression = Expression.Call(
                source,
                addMethod,
                item);

            return addExpression;
        }

        /// <summary>
        /// Resulting expression: source.Contains(value)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetContainsOverEnumerable(Type sourceType, Expression source, Expression value)
        {
            MethodInfo containsMethod;
            Type enumerableType;

            if (source.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IQueryable)))
            {
                enumerableType = typeof(Queryable);
            }
            else
            {
                enumerableType = typeof(Enumerable);
            }

            containsMethod = enumerableType.GetRuntimeMethods()
                                                        .FirstOrDefault(c => c.Name == "Contains" &&
                                                                             c.GetParameters().Count() == 2);

            MethodCallExpression containsExpression = Expression.Call(
                instance: null,
                method: containsMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arg0: source, arg1: value);

            return containsExpression;
        }

        /// <summary>
        /// Resulting expression: source.Except(value)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetExceptOverEnumerable(Type sourceType, Expression source, Expression value)
        {
            MethodInfo exceptMethod;
            Type enumerableType;

            if (source.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IQueryable)))
            {
                enumerableType = typeof(Queryable);
            }
            else
            {
                enumerableType = typeof(Enumerable);
            }

            exceptMethod = enumerableType.GetRuntimeMethods()
                                                        .FirstOrDefault(c => c.Name == "Except" &&
                                                                             c.GetParameters().Count() == 2);

            MethodCallExpression exceptExpression = Expression.Call(
                instance: null,
                method: exceptMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arg0: source, arg1: value);

            return exceptExpression;
        }

        /// <summary>
        /// Resulting expression: source.Remove(item)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetRemoveItemByMethodName(Type sourceType, Expression source, Expression item)
        {
            MethodInfo removeMethod = source.Type.GetRuntimeMethods()
                                                            .FirstOrDefault(c => c.Name == "Remove");

            MethodCallExpression removeExpression = Expression.Call(
                source,
                removeMethod,
                item);

            return removeExpression;
        }

        /// <summary>
        /// Resulting expression: source.ForEach(actionAsExpression)
        /// </summary>
        /// <param name="destinationType"></param>
        /// <param name="source"></param>
        /// <param name="actionAsExpression"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetForEachOverIEnumerable(Type sourceType, Expression source, Expression forEachBody, ParameterExpression forEachParameter)
        {
            LambdaExpression actionAsExpression = Expression.Lambda(typeof(Action<>).MakeGenericType(sourceType), forEachBody, forEachParameter);

            MethodInfo forEachMethod = typeof(IEnumerableExtensions).GetRuntimeMethods()
                                                                    .FirstOrDefault(c => c.Name == "ForEach");

            MethodCallExpression forEachExpression = Expression.Call(
                instance: null,
                method: forEachMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arg0: source, arg1: actionAsExpression);

            return forEachExpression;
        }

        /// <summary>
        /// Resulting expression: source.FirstOrDefault(predicate)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetFirstOrDefaultOverEnumerable(Type sourceType, ParameterExpression firstOrDefaultParameter, Expression source, Expression predicate)
        {
            LambdaExpression firstOrDefaultPredicateAsFunction = Expression.Lambda(typeof(Func<,>).MakeGenericType(sourceType, typeof(bool)), predicate, firstOrDefaultParameter);

            MethodInfo firstOrDefaultMethod;
            Type enumerableType;

            if (source.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IQueryable)))
            {
                enumerableType = typeof(Queryable);
            }
            else
            {
                enumerableType = typeof(Enumerable);
            }

            firstOrDefaultMethod = enumerableType.GetRuntimeMethods()
                                                                .FirstOrDefault(c => c.Name == "FirstOrDefault" &&
                                                                                     c.GetParameters()
                                                                                      .Where(p => p.Name.Equals(value: "predicate")&&
                                                                                                  p.ParameterType.GenericTypeArguments != null &&
                                                                                                  p.ParameterType.GenericTypeArguments.Count() == 2)
                                                                                      .Any());

            MethodCallExpression firstOrDefaultExpression = Expression.Call(
                instance: null,
                method: firstOrDefaultMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arg0: source, arg1: firstOrDefaultPredicateAsFunction);

            return firstOrDefaultExpression;
        }

        /// <summary>
        /// Resulting expression: source.Where(predicate)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetWhereOverEnumerable(Type sourceType, ParameterExpression whereParameter, Expression source, Expression predicate)
        {
            LambdaExpression predicateAsExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(sourceType, typeof(bool)), predicate, whereParameter);

            MethodInfo whereMethod;
            Type enumerableType;

            if (source.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IQueryable)))
            {
                enumerableType = typeof(Queryable);
            }
            else
            {
                enumerableType = typeof(Enumerable);
            }

            whereMethod = enumerableType.GetRuntimeMethods()
                                                                .FirstOrDefault(c => c.Name == "Where" &&
                                                                                     c.GetParameters()
                                                                                      .Where(p => p.Name.Equals(value: "predicate")&&
                                                                                                  p.ParameterType.GenericTypeArguments != null &&
                                                                                                  p.ParameterType.GenericTypeArguments.Count() == 2)
                                                                                      .Any());

            MethodCallExpression whereExpression = Expression.Call(
                instance: null,
                method: whereMethod.MakeGenericMethod(new Type[]
                {
                    sourceType
                }), arg0: source, arg1: predicateAsExpression);

            return whereExpression;
        }

        /// <summary>
        /// Resulting expression: source.Aggregate(seed, aggregateFunction)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="selectParameter"></param>
        /// <param name="source"></param>
        /// <param name="transformFunction"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetAggregateWithSeedOverEnumerable(Type sourceType, Type destinationType,Expression seedParameter,
                                                                                ParameterExpression nextParameter, ParameterExpression accumulateParameter,
                                                                                Expression source, Expression aggregateFunction)
        {

            LambdaExpression aggregateFunctionAsExpression = Expression.Lambda(typeof(Func<,,>).MakeGenericType(destinationType, destinationType, destinationType),
                                                                                                    aggregateFunction, accumulateParameter, nextParameter);

            MethodInfo aggregateMethod;
            Type enumerableType;

            if (source.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IQueryable)))
            {
                enumerableType = typeof(Queryable);
            }
            else
            {
                enumerableType = typeof(Enumerable);
            }

            aggregateMethod = enumerableType.GetRuntimeMethods()
                                           .FirstOrDefault(c => c.Name == "Aggregate" &&
                                                                c.GetParameters().Count() == 3);

            MethodCallExpression aggregateExpression = Expression.Call(
                instance: null,
                method: aggregateMethod.MakeGenericMethod(new Type[]
                {
                    sourceType,
                    destinationType
                }), arg0: source, arg1: seedParameter, arg2: aggregateFunctionAsExpression);

            return aggregateExpression;
        }

        /// <summary>
        /// Resulting expression: source.Select(transformFunction)
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="source"></param>
        /// <param name="transformFunction"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetSelectOverEnumerable(Type sourceType, Type destinationType, ParameterExpression selectParameter, Expression source, Expression transformFunction)
        {
            LambdaExpression transformFunctionAsExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(sourceType, destinationType), transformFunction, selectParameter);

            MethodInfo selectMethod;
            Type enumerableType;

            if (source.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IQueryable)))
            {
                enumerableType = typeof(Queryable);
            }
            else
            {
                enumerableType = typeof(Enumerable);
            }

            selectMethod = enumerableType.GetRuntimeMethods()
                                           .FirstOrDefault(c => c.Name == "Select" &&
                                                                c.GetParameters()
                                                                 .Where(p => p.Name.Equals(value: "selector")&&
                                                                             (p.ParameterType.GenericTypeArguments != null &&
                                                                             p.ParameterType.GenericTypeArguments.Count() == 2) ||
                                                                             (p.ParameterType.GenericTypeArguments != null &&
                                                                             p.ParameterType.GenericTypeArguments[0].GenericTypeArguments != null &&
                                                                             p.ParameterType.GenericTypeArguments[0].GenericTypeArguments.Count() ==2))
                                                                 .Any());
            MethodCallExpression selectExpression = Expression.Call(
                instance: null,
                method: selectMethod.MakeGenericMethod(new Type[]
                {
                    sourceType,
                    destinationType
                }), arg0: source, arg1: transformFunctionAsExpression);

            return selectExpression;
        }



    }
}
