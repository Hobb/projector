using Projector.Core.Contracts;
using Projector.Helpers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Projector
{
    public abstract class ProjectionExpressionCore<TSource>
    {
        #region Expression Cache

        protected static readonly ConcurrentDictionary<string, Expression> ExpressionCache = new ConcurrentDictionary<string, Expression>();

        protected static readonly ConcurrentDictionary<string, Delegate> DelegateCache = new ConcurrentDictionary<string, Delegate>();

        protected Expression<Func<TSource, TDest>> GetCachedExpression<TDest>(string flatteningToken)
        {
            string key = GetCacheKey<Expression<Func<TSource, TDest>>>(flatteningToken);

            return ExpressionCache.ContainsKey(key) ? ExpressionCache[key] as Expression<Func<TSource, TDest>> : null;
        }

        protected Func<TSource, TDest> GetCachedDelegate<TDest>(string flatteningToken)
        {
            string key = GetCacheKey<Func<TSource, TDest>>(flatteningToken);

            return DelegateCache.ContainsKey(key) ? DelegateCache[key] as Func<TSource, TDest> : null;
        }

        protected Expression<Action<IQueryable<TSource>, IQueryable<TDest>>> GetCachedExpressionOverQueryableDestination<TDest>(string keyMemberName, bool clearCollections, string flatteningToken)
        {
            string key = GetCacheKey<Action<IQueryable<TSource>, IQueryable<TDest>>>(flatteningToken, keyMemberName, clearCollections);

            return ExpressionCache.ContainsKey(key) ? ExpressionCache[key] as Expression<Action<IQueryable<TSource>, IQueryable<TDest>>> : null;
        }

        protected Expression<Action<TSource, TDest>> GetCachedExpressionOverDestination<TDest>(string keyMemberName, bool clearCollections, string flatteningToken)
        {
            string key = GetCacheKey<Action<IQueryable<TSource>, IQueryable<TDest>>>(flatteningToken, keyMemberName, clearCollections);

            return ExpressionCache.ContainsKey(key) ? ExpressionCache[key] as Expression<Action<TSource, TDest>> : null;
        }

        protected Action<TSource, TDest> GetCachedActionOverDestination<TDest>(string keyMemberName, bool clearCollections, string flatteningToken)
        {
            string key = GetCacheKey<Action<TSource, TDest>>(flatteningToken, keyMemberName, clearCollections);

            return DelegateCache.ContainsKey(key) ? DelegateCache[key] as Action<TSource, TDest> : null;
        }

        protected Delegate GetCachedActionOverQueryableDestination<TDest>(string keyMemberName, bool clearCollections, string flatteningToken)
        {
            string key = GetCacheKey<Delegate>(flatteningToken, keyMemberName, clearCollections);

            return DelegateCache.ContainsKey(key) ? DelegateCache[key] : null;
        }

        #endregion

        #region Dynamic types

        private static readonly AssemblyName assemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
        private static readonly ModuleBuilder moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(assemblyName.Name);
        private static readonly Dictionary<string, TypeInfo> constructedTypesCache = new Dictionary<string, TypeInfo>();

        #endregion

        #region Mapping methods

        protected IEnumerable<MemberAssignment> GetPropertyAndFieldBindings(Type sourceType, Type destinationType, Expression parameterExpression, string flatteningToken = null, List<string> recursePropertyList = null)
        {
            IEnumerable<MemberInfo> sourceFieldsAndProperties = sourceType.GetRuntimeFieldsAndPropertiesEx();

            IEnumerable<MemberInfo> destinationFieldsAndProperties = destinationType.GetRuntimeFieldsAndPropertiesEx();

            return destinationFieldsAndProperties.Select(destinationMember => BuildBinding(parameterExpression, destinationMember, sourceFieldsAndProperties, flatteningToken, recursePropertyList))
                                                 .Where(binding => binding != null);
        }

        protected IEnumerable<MemberAssignment> GetPropertyAndFieldBindings<SourceType, DestinationType>(Expression parameterExpression, string flatteningToken = null, List<string> recursePropertyList = null)
        {
            return GetPropertyAndFieldBindings(typeof(SourceType), typeof(DestinationType), parameterExpression, flatteningToken, recursePropertyList);
        }

        protected IEnumerable<Expression> GetMemberAssignmentExpressions(Type sourceType, Type destinationType, Expression parameterExpression, Expression destinationParameterExpression, string keyMemberName, bool clearCollections, string flatteningToken = null, List<string> recursePropertyList = null)
        {
            IEnumerable<MemberInfo> sourceFieldsAndProperties = sourceType.GetRuntimeFieldsAndPropertiesEx();

            IEnumerable<MemberInfo> destinationFieldsAndProperties = destinationType.GetRuntimeFieldsAndPropertiesEx();

            return destinationFieldsAndProperties.Select(destinationMember => BuildAssignment(parameterExpression, destinationParameterExpression, destinationMember, sourceFieldsAndProperties, keyMemberName, clearCollections, flatteningToken, recursePropertyList))
                                                 .Where(assignment => assignment != null);
        }

        protected IEnumerable<Expression> GetMemberAssignmentExpressions<SourceType, DestinationType>(Expression parameterExpression, Expression destinationParameterExpression, string keyMemberName, bool clearCollections, string flatteningToken = null, List<string> recursePropertyList = null)
        {
            return GetMemberAssignmentExpressions(typeof(SourceType), typeof(DestinationType), parameterExpression, destinationParameterExpression, keyMemberName, clearCollections, flatteningToken, recursePropertyList);
        }

        protected Expression<Action<TSource, TDest>> BuildExpressionOverDestination<TDest>(bool clearCollections, string keyMemberName = null, string flatteningToken = null)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource), name: "src");

            ParameterExpression destinationParameterExpression = Expression.Parameter(typeof(TDest), name: "dst");

            IEnumerable<Expression> expressionList = GetMemberAssignmentExpressions<TSource, TDest>(parameterExpression, destinationParameterExpression, keyMemberName, clearCollections, flatteningToken);

            BlockExpression block = ExpressionHelper.ExpressionBlockHelper(expressionList);

            Expression<Action<TSource, TDest>> expression = Expression.Lambda<Action<TSource, TDest>>(block, parameterExpression, destinationParameterExpression);

            string key = GetCacheKey<Action<TSource, TDest>>(flatteningToken, keyMemberName, clearCollections);

            ExpressionCache.TryAdd(key, expression);

            return expression;
        }

        protected BinaryExpression GetElementsToRemove(Type destinationGenericType, Type sourceGenericType, Type sourceKeyType,
            Type destinationKeyType, Expression srcMemberExp,
            Expression dstMemberExp, ref ParameterExpression toRemoveListVariable,
            List<string> recursePropertyList, string keyMemberName)
        {
            #region WHERE OVER DESTINATION PARAMETER

            ParameterExpression destinationWhereParameter = Expression.Parameter(destinationGenericType, name: "d_" + string.Join(String.Empty, recursePropertyList));

            #endregion

            #region INTENTION: source.Select(s => s.Id)

            ParameterExpression selectOverSourceParameter = Expression.Parameter(sourceGenericType, name: "s_" + string.Join(String.Empty, recursePropertyList));
            Expression selectOverSourceMemberExp = Expression.PropertyOrField(selectOverSourceParameter, keyMemberName);
            MethodCallExpression selectOverSource = ExpressionHelper.GetSelectOverEnumerable(sourceGenericType, sourceKeyType, selectOverSourceParameter, srcMemberExp, selectOverSourceMemberExp);

            #endregion

            #region INTENTION: !source.Select(s => s.Id).Contains(d.Id)

            MethodCallExpression containsOverSourceIds = ExpressionHelper.GetContainsOverEnumerable(destinationKeyType, selectOverSource, Expression.PropertyOrField(destinationWhereParameter, keyMemberName));

            #endregion

            #region INTENTION: var toRemove = destination.Where(d =>!source.Select(s => s.Id).Contains(d.Id)).ToList()

            MethodCallExpression whereOverDestination = ExpressionHelper.GetWhereOverEnumerable(destinationGenericType, destinationWhereParameter, dstMemberExp, Expression.Not(containsOverSourceIds));
            MethodCallExpression toRemoveToListExpression = ExpressionHelper.GetToListOverEnumerable(destinationGenericType, whereOverDestination);
            toRemoveListVariable = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(destinationGenericType), name: "toRemove");
            BinaryExpression assignToRemoveToList = Expression.Assign(toRemoveListVariable, toRemoveToListExpression);

            #endregion

            return assignToRemoveToList;
        }

        protected BinaryExpression GetElementsToAdd(Type destinationGenericType, Type sourceGenericType, Type sourceKeyType,
            Type destinationKeyType, Expression srcMemberExp,
            Expression dstMemberExp, ref ParameterExpression toAddListVariable,
            List<string> recursePropertyList, string keyMemberName)
        {
            #region WHERE OVER SOURCE PARAMETER

            ParameterExpression sourceWhereParameter = Expression.Parameter(sourceGenericType, name: "s_" + string.Join(String.Empty, recursePropertyList));

            #endregion

            #region INTENTION: destination.Select(d => d.Id)

            ParameterExpression selectOverDestinationParameter = Expression.Parameter(destinationGenericType, name: "d_" + string.Join(String.Empty, recursePropertyList));
            Expression selectOverDestinationMemberExp = Expression.PropertyOrField(selectOverDestinationParameter, keyMemberName);
            MethodCallExpression selectOverDestination = ExpressionHelper.GetSelectOverEnumerable(destinationGenericType, destinationKeyType, selectOverDestinationParameter, dstMemberExp, selectOverDestinationMemberExp);

            #endregion

            #region INTENTION: !destination.Select(d => d.Id).Contains(s.Id)

            MethodCallExpression containsOverDestinationIds = ExpressionHelper.GetContainsOverEnumerable(sourceKeyType, selectOverDestination, Expression.PropertyOrField(sourceWhereParameter, keyMemberName));

            #endregion

            #region INTENTION: var toAdd = source.Where(s => !destination.Select(d => d.Id).Contains(s.Id))

            MethodCallExpression whereOverSource = ExpressionHelper.GetWhereOverEnumerable(sourceGenericType, sourceWhereParameter, srcMemberExp, Expression.Not(containsOverDestinationIds));
            toAddListVariable = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(sourceGenericType), name: "toAdd");
            BinaryExpression assignToAddToList = Expression.Assign(toAddListVariable, whereOverSource);

            #endregion

            return assignToAddToList;
        }

        protected BinaryExpression GetElementsToRemoveComplexKey(Type destinationGenericType, Type sourceGenericType, Expression srcMemberExp,
                                                                Expression dstMemberExp, ref ParameterExpression toRemoveListVariable,
                                                                List<string> recursePropertyList, List<MemberInfo> keyMemberList)
        {
            #region WHERE OVER DESTINATION PARAMETER

            ParameterExpression destinationWhereParameter = Expression.Parameter(destinationGenericType, name: "d_" + string.Join(String.Empty, recursePropertyList));

            #endregion

            #region SELECT OVER SOURCE PARAMETER

            ParameterExpression selectOverSourceParameter = Expression.Parameter(sourceGenericType, name: "s_" + string.Join(String.Empty, recursePropertyList));

            #endregion

            #region INTENTION: new { NumericId = s.NumericId, StringId = s.StringId }

            MemberInitExpression sourceKeyInitializatorExpression = GetAnonKeyTypeInit(selectOverSourceParameter, recursePropertyList, keyMemberList);

            #endregion

            #region INTENTION: new { NumericId = d.NumericId, StringId = d.StringId }

            MemberInitExpression destinationKeyInitializatorExpression = GetAnonKeyTypeInit(destinationWhereParameter, recursePropertyList, keyMemberList);

            #endregion

            #region INTENTION: source.Select()

            MethodCallExpression selectOverSource = ExpressionHelper.GetSelectOverEnumerable(sourceGenericType, sourceKeyInitializatorExpression.Type, selectOverSourceParameter, srcMemberExp, sourceKeyInitializatorExpression);

            #endregion

            #region INTENTION: .Contains()

            MethodCallExpression containsOverSourceHashCodes = ExpressionHelper.GetContainsOverEnumerable(destinationKeyInitializatorExpression.Type, selectOverSource, destinationKeyInitializatorExpression);

            #endregion

            #region INTENTION: var toRemove = destination.Where(d =>!source.Select().Contains()).ToList()

            MethodCallExpression whereOverDestination = ExpressionHelper.GetWhereOverEnumerable(destinationGenericType, destinationWhereParameter, dstMemberExp, Expression.Not(containsOverSourceHashCodes));
            MethodCallExpression toRemoveToListExpression = ExpressionHelper.GetToListOverEnumerable(destinationGenericType, whereOverDestination);
            toRemoveListVariable = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(destinationGenericType), name: "toRemove");
            BinaryExpression assignToRemoveToList = Expression.Assign(toRemoveListVariable, toRemoveToListExpression);

            #endregion

            return assignToRemoveToList;
        }

        private MethodCallExpression GetClassCombinedKeyHashCode(ParameterExpression sourceParameter, List<string> recursePropertyList, List<MemberInfo> keyMemberList)
        {

            #region INTENTION (Hash calculation function): (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor)

            ParameterExpression nextAggregateParameter = Expression.Parameter(typeof(int), name: "n_" + string.Join(String.Empty, recursePropertyList));
            ParameterExpression accumulateAggregateParameter = Expression.Parameter(typeof(int), name: "a_" + string.Join(String.Empty, recursePropertyList));
            Expression aggregateFunction = Expression.Add(Expression.Multiply(accumulateAggregateParameter, Expression.Constant(value: 31)), nextAggregateParameter);

            #endregion

            #region INTENTION: new int[] { s.NumericId.GetHashCode(), s.StringId.GetHashCode() }

            List<MethodCallExpression> sourceKeysGetHashCodesExpressions = new List<MethodCallExpression>();
            foreach (MemberInfo member in keyMemberList)
            {
                MemberExpression keyAccessorExpression = Expression.PropertyOrField(sourceParameter, member.Name);
                MethodCallExpression getHashCodeCall = Expression.Call(keyAccessorExpression, methodName: "GetHashCode", typeArguments: null);

                sourceKeysGetHashCodesExpressions.Add(getHashCodeCall);
            }

            NewArrayExpression sourceHashCodeListInitializationExpression = Expression.NewArrayInit(typeof(int), sourceKeysGetHashCodesExpressions);

            #endregion

            #region INTENTION: new int[] { s.NumericId.GetHashCode(), s.StringId.GetHashCode() }.Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor)))

            return ExpressionHelper.GetAggregateWithSeedOverEnumerable(typeof(int),
                                                                                                                typeof(int),
                                                                                                                Expression.Constant(value: 17)as Expression,
                                                                                                                nextAggregateParameter,
                                                                                                                accumulateAggregateParameter,
                                                                                                                sourceHashCodeListInitializationExpression,
                                                                                                                aggregateFunction);

            #endregion
        }

        private MemberInitExpression GetAnonKeyTypeInit(ParameterExpression sourceParameter, List<string> recursePropertyList, List<MemberInfo> keyMemberList)
        {
            #region INTENTION: new { NumericId = s.NumericId, StringId = s.StringId }

            string cacheKey = string.Join(separator: "'", value:  keyMemberList.Select(c => c.Name + "_" + c.GetUnderlyingType().Name).ToArray() );
            TypeInfo createdType;
            List<MemberBinding> keyBindingList = new List<MemberBinding>();

            if (constructedTypesCache.ContainsKey(cacheKey))
            {
                createdType = constructedTypesCache[cacheKey];
            }
            else { 

                TypeBuilder typeBuilder = moduleBuilder.DefineType(name: cacheKey, attr: TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (MemberInfo member in keyMemberList)
                {
                    MemberExpression keyAccessorExpression = Expression.PropertyOrField(sourceParameter, member.Name);

                    typeBuilder.DefineField(member.Name, member.GetUnderlyingType(), FieldAttributes.Public);
                }

                createdType = typeBuilder.CreateTypeInfo();

                constructedTypesCache.Add(cacheKey, createdType);
            }

            foreach(var member in createdType.DeclaredMembers.OfType<FieldInfo>())
            {
                MemberExpression keyAccessorExpression = Expression.PropertyOrField(sourceParameter, member.Name);

                keyBindingList.Add(Expression.Bind(member, keyAccessorExpression));
            }

            return Expression.MemberInit(Expression.New(createdType.DeclaredConstructors.First()), keyBindingList);

            #endregion
        }

        protected BinaryExpression GetElementsToAddComplexKey(Type destinationGenericType, Type sourceGenericType, Expression srcMemberExp,
                                                        Expression dstMemberExp, ref ParameterExpression toRemoveListVariable,
                                                        List<string> recursePropertyList, List<MemberInfo> keyMemberList)
        {
            #region WHERE OVER SOURCE PARAMETER

            ParameterExpression sourceWhereParameter = Expression.Parameter(sourceGenericType, name: "s_" + string.Join(String.Empty, recursePropertyList));

            #endregion

            #region SELECT OVER DESTINATION PARAMETER

            ParameterExpression selectOverDestinationParameter = Expression.Parameter(destinationGenericType, name: "d_" + string.Join(String.Empty, recursePropertyList));

            #endregion

            #region INTENTION: new { s.NumericId, s.StringId }

            MemberInitExpression sourceKeyInitializatorExpression = GetAnonKeyTypeInit(sourceWhereParameter, recursePropertyList, keyMemberList);

            #endregion

            #region INTENTION: new { d.NumericId, d.StringId }

            MemberInitExpression destinationKeyInitializatorExpression = GetAnonKeyTypeInit(selectOverDestinationParameter, recursePropertyList, keyMemberList);

            #endregion

            #region INTENTION: destination.Select()

            MethodCallExpression selectOverDestination = ExpressionHelper.GetSelectOverEnumerable(destinationGenericType, destinationKeyInitializatorExpression.Type, selectOverDestinationParameter, dstMemberExp, destinationKeyInitializatorExpression);

            #endregion

            #region INTENTION: .Contains()

            MethodCallExpression containsOverDestinationHashCodes = ExpressionHelper.GetContainsOverEnumerable(sourceKeyInitializatorExpression.Type, selectOverDestination, sourceKeyInitializatorExpression);

            #endregion

            #region INTENTION: var toAdd = source.Where(s => !destination.Select(d => d.Id).Contains(s.Id))

            MethodCallExpression whereOverSource = ExpressionHelper.GetWhereOverEnumerable(sourceGenericType, sourceWhereParameter, srcMemberExp, Expression.Not(containsOverDestinationHashCodes));
            toRemoveListVariable = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(sourceGenericType), name: "toAdd");
            BinaryExpression assignToAddToList = Expression.Assign(toRemoveListVariable, whereOverSource);

            #endregion

            return assignToAddToList;
        }


        protected MethodCallExpression AddElementsFromSourceToDestination(Type destinationGenericType, Type sourceGenericType,
                                                        Expression dstMemberExp, ref ParameterExpression toAddListVariable,
                                                        List<string> recursePropertyList, string flatteningToken)
        {
            ParameterExpression forEachAddParameter = Expression.Parameter(sourceGenericType, name: "a_" + string.Join(String.Empty, recursePropertyList));

            #region INTENTION: a.Project().To<BasicEntity>()

            MethodCallExpression projectAddItemsExpression = ExpressionHelper.GetProject(sourceGenericType, forEachAddParameter);
            MethodCallExpression projectionExpressionAddItemsExpression = ExpressionHelper.GetProjectionExpressionTo(sourceGenericType, destinationGenericType, projectAddItemsExpression, flatteningToken);

            #endregion

            #region INTENTION: destination.Add(a.Project().To<BasicEntity>())

            MethodCallExpression forEachAddBodyExpression = ExpressionHelper.GetAddItemByMethodName(destinationGenericType, dstMemberExp, projectionExpressionAddItemsExpression);

            #endregion

            #region INTENTION: toAdd.ForEach(a => destination.Add(a.Project().To<BasicEntity>()))

            MethodCallExpression forEachAdd = ExpressionHelper.GetForEachOverIEnumerable(sourceGenericType, toAddListVariable, forEachAddBodyExpression, forEachAddParameter);

            #endregion

            return forEachAdd;
        }

        protected MethodCallExpression UpdateRemainingElements(Type destinationGenericType, Type sourceGenericType,
                                                        Expression srcMemberExp, Expression dstMemberExp,
                                                        ref ParameterExpression toAddListVariable,
                                                        string keyMemberName, bool clearCollections,
                                                        List<string> recursePropertyList, string flatteningToken)
        {
            ParameterExpression forEachUpdateParameter = Expression.Parameter(sourceGenericType, name: "u_" + string.Join(String.Empty, recursePropertyList));

            #region INTENTION: destination.FirstOrDefault(d => d.Id == u.Id)

            ParameterExpression firstOrDefaultDestinationParameter = Expression.Parameter(destinationGenericType, name: "d_" + string.Join(String.Empty, recursePropertyList));
            Expression firstOrDefaultDestinationMemberExp = Expression.PropertyOrField(firstOrDefaultDestinationParameter, keyMemberName);
            Expression firstOrDefaultToUpdateItemsMemberExp = Expression.PropertyOrField(forEachUpdateParameter, keyMemberName);

            MethodCallExpression firstOrDefaultDestinationPredicate = ExpressionHelper.GetEquals(firstOrDefaultDestinationMemberExp, firstOrDefaultToUpdateItemsMemberExp);

            MethodCallExpression firstOrDefaultOverDestination = ExpressionHelper.GetFirstOrDefaultOverEnumerable(destinationGenericType, firstOrDefaultDestinationParameter, dstMemberExp, firstOrDefaultDestinationPredicate);

            #endregion

            #region INTENTION: u.Project().To<BasicEntity>(destination.FirstOrDefault(d => d.Id == u.Id)))

            MethodCallExpression projectOverExistingExpressionOverSource = ExpressionHelper.GetProject(sourceGenericType, forEachUpdateParameter);
            MethodCallExpression projectionOverExistingExpressionToDestinationExpression = ExpressionHelper.GetProjectionOverDestinationExpressionTo(sourceGenericType, destinationGenericType, projectOverExistingExpressionOverSource, firstOrDefaultOverDestination, keyMemberName, clearCollections, flatteningToken);

            #endregion

            #region INTENTION: source.Except(toAdd).ForEach(u => u.Project().To<BasicEntity>(destination.FirstOrDefault(d => d.Id == u.Id)));

            MethodCallExpression sourceExceptToAddExpression = ExpressionHelper.GetExceptOverEnumerable(sourceGenericType, srcMemberExp, toAddListVariable);

            MethodCallExpression forEachSourceExceptToAddUpdate = ExpressionHelper.GetForEachOverIEnumerable(sourceGenericType, sourceExceptToAddExpression, projectionOverExistingExpressionToDestinationExpression, forEachUpdateParameter);

            #endregion

            return forEachSourceExceptToAddUpdate;
        }

        protected MethodCallExpression UpdateRemainingElementsComplexKey(Type destinationGenericType, Type sourceGenericType,
                                                                        Expression srcMemberExp, Expression dstMemberExp,
                                                                        ref ParameterExpression toAddListVariable,
                                                                        List<MemberInfo> keyMemberList, bool clearCollections,
                                                                        List<string> recursePropertyList, string flatteningToken)
        {
            ParameterExpression forEachUpdateParameter = Expression.Parameter(sourceGenericType, name: "u_" + string.Join(String.Empty, recursePropertyList));

            #region INTENTION: FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)

            ParameterExpression firstOrDefaultDestinationParameter = Expression.Parameter(destinationGenericType, name: "d_" + string.Join(String.Empty, recursePropertyList));

            Expression firstOrDefaultDestinationPredicate = null;

            foreach (MemberInfo keyMemberInfo in keyMemberList)
            {
                Expression firstOrDefaultDestinationMemberExp = Expression.PropertyOrField(firstOrDefaultDestinationParameter, keyMemberInfo.Name);
                Expression firstOrDefaultToUpdateItemsMemberExp = Expression.PropertyOrField(forEachUpdateParameter, keyMemberInfo.Name);
                MethodCallExpression equalsPredicate = ExpressionHelper.GetEquals(firstOrDefaultDestinationMemberExp, firstOrDefaultToUpdateItemsMemberExp);

                if(firstOrDefaultDestinationPredicate == null)
                {
                    firstOrDefaultDestinationPredicate = equalsPredicate;
                }
                else
                {
                    firstOrDefaultDestinationPredicate = Expression.AndAlso(firstOrDefaultDestinationPredicate, equalsPredicate);
                }
            }

            MethodCallExpression firstOrDefaultOverDestination = ExpressionHelper.GetFirstOrDefaultOverEnumerable(destinationGenericType, firstOrDefaultDestinationParameter, dstMemberExp, firstOrDefaultDestinationPredicate);

            #endregion

            #region INTENTION: u.Project().To<BasicEntity>(destination.FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)))

            MethodCallExpression projectOverExistingExpressionOverSource = ExpressionHelper.GetProject(sourceGenericType, forEachUpdateParameter);
            MethodCallExpression projectionOverExistingExpressionToDestinationExpression = ExpressionHelper.GetProjectionOverDestinationExpressionTo(sourceGenericType, destinationGenericType, projectOverExistingExpressionOverSource, firstOrDefaultOverDestination, keyMemberName: null, clearCollections: clearCollections, flatteningToken: flatteningToken);

            #endregion

            #region INTENTION: source.Except(toAdd).ForEach(u => u.Project().To<BasicEntity>(destination.FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)));

            MethodCallExpression sourceExceptToAddExpression = ExpressionHelper.GetExceptOverEnumerable(sourceGenericType, srcMemberExp, toAddListVariable);

            MethodCallExpression forEachSourceExceptToAddUpdate = ExpressionHelper.GetForEachOverIEnumerable(sourceGenericType, sourceExceptToAddExpression, projectionOverExistingExpressionToDestinationExpression, forEachUpdateParameter);

            #endregion

            return forEachSourceExceptToAddUpdate;
        }

        protected Expression ProjectEnumerableOverEnumerable(Type sourceGenericType, Type destinationGenericType,
            Expression srcMemberExp, Expression dstMemberExp,
            string keyMemberName, bool clearCollections,
            string flatteningToken = null, List<string> recursePropertyList = null)
        {
            Expression clearCollectionExpression = null;
            Type destinationKeyType = null;
            Type sourceKeyType = null;

            #region GET KEY INFO
            
            List<MemberInfo> keyMemberList = destinationGenericType.GetRuntimeFieldsAndPropertiesEx().Where(c => c.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute)).Any()).ToList();

            if (destinationGenericType.GetTypeInfo()
                                      .ImplementedInterfaces
                                      .Any(x => x.GetTypeInfo().IsGenericType &&
                                                x.GetGenericTypeDefinition() == typeof(IKeyedItem<>)) &&
                keyMemberName == null)
            {
                keyMemberName = "Id";
            }

            if (keyMemberList != null && keyMemberList.Count == 1)
            {
                keyMemberName = keyMemberList.Single().Name;
            }

            if (keyMemberName == null || clearCollections)
            {
                clearCollectionExpression = ExpressionHelper.GetClearOverICollection(destinationGenericType, dstMemberExp);
            }
            else
            {
                destinationKeyType = destinationGenericType.GetRuntimeFieldsAndPropertiesEx().FirstOrDefault(c => c.Name == keyMemberName).GetUnderlyingType();
                sourceKeyType = sourceGenericType.GetRuntimeFieldsAndPropertiesEx().FirstOrDefault(c => c.Name == keyMemberName).GetUnderlyingType();
            }

            #endregion

            #region Sample code

            // ********************** SIMPLE KEY *************************
            //var toRemove = destination.Where(d => !source.Select(s => s.Id).Contains(d.Id))
            //                          .ToList();
            //toRemove.ForEach(r => destination.Remove(r));
            //var toAdd = source.Where(s => !destination.Select(d => d.Id).Contains(s.Id));
            //toAdd.ForEach(a => destination.Add(a.Project().To<BasicEntity>()));
            //source.Except(toAdd)
            //        .ForEach(u => u.Project()
            //                        .To<BasicEntity>(destination.
            //                                            FirstOrDefault(d => d.Id == u.Id)));
            //
            // ********************** COMPLEX KEY ************************
            //var toRemove = destination.Where(d => !source.Select(s => new int[] { s.NumericId.GetHashCode(), s.StringId.GetHashCode() }
            //                                                                        .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor)))
            //                                            .Contains(new int[] { d.NumericId.GetHashCode(), d.StringId.GetHashCode() }
            //                                                                        .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor))))
            //                                            .ToList();
            //
            //toRemove.ForEach(r => destination.Remove(r));
            //
            //var toAdd = source.Where(s => !destination.Select(d => new int[] () { d.NumericId.GetHashCode(), d.StringId.GetHashCode() }
            //                                                                                    .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor)))
            //                                                        .Contains(new int[] { s.NumericId.GetHashCode(), s.StringId.GetHashCode() }
            //                                                                                    .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor))));
            //
            //toAdd.ForEach(a => destination.Add(a.Project().To<BasicEntityWithComplexKeyAttributes>()));
            //
            //source.Except(toAdd)
            //        .ForEach(u => u.Project()
            //                        .To<BasicEntityWithComplexKeyAttributes>(destination.
            //                                            FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)));

            #endregion

            #region PROJECT ENUMERABLE OVER ENUMERABLE

            #region GET ELEMENTS TO REMOVE

            ParameterExpression toRemoveListVariable = null;
            BinaryExpression assignToRemoveToList = null;

            if (keyMemberList != null && keyMemberList.Count > 1)
            {

                #region INTENTION: var toRemove = destination.Where(d => !source.Select(s => new int[]{SourceHashCodes}.Aggregate(HashingFunction).Contains(new int[]{DestinationHashCodes}.Aggregate(HashingFunction)))).ToList()

                assignToRemoveToList = GetElementsToRemoveComplexKey(destinationGenericType, sourceGenericType,
                    srcMemberExp, dstMemberExp,
                    ref toRemoveListVariable,
                    recursePropertyList, keyMemberList);

                #endregion
            }
            else
            {
                #region INTENTION: var toRemove = destination.Where(d =>!source.Select(s => s.Id).Contains(d.Id)).ToList()

                assignToRemoveToList = GetElementsToRemove(destinationGenericType, sourceGenericType,
                    sourceKeyType, destinationKeyType,
                    srcMemberExp, dstMemberExp,
                    ref toRemoveListVariable,
                    recursePropertyList, keyMemberName);

                #endregion
            }

            #endregion

            #region REMOVE ELEMENTS FROM DESTINATION

            #region INTENTION: toRemove.ForEach(r => destination.Remove(r));

            ParameterExpression forEachRemoveParameter = Expression.Parameter(destinationGenericType, name: "r_" + string.Join(String.Empty, recursePropertyList));
            MethodCallExpression forEachRemoveBodyExpression = ExpressionHelper.GetRemoveItemByMethodName(destinationGenericType, dstMemberExp, forEachRemoveParameter);
            MethodCallExpression forEachRemove = ExpressionHelper.GetForEachOverIEnumerable(destinationGenericType, toRemoveListVariable, forEachRemoveBodyExpression, forEachRemoveParameter);

            #endregion

            #endregion

            #region GET ELEMENTS TO ADD 

            ParameterExpression toAddListVariable = null;
            BinaryExpression assignToAddToList = null;

            if (keyMemberList != null && keyMemberList.Count > 1)
            {
                #region INTENTION: var toAdd = source.Where(d => !destination.Select(d => new int[]{DestinationHashCodes}.Aggregate(HashingFunction).Contains(new int[]{SourceHashCodes}.Aggregate(HashingFunction))))

                assignToAddToList = GetElementsToAddComplexKey(destinationGenericType, sourceGenericType,
                                                                    srcMemberExp, dstMemberExp,
                                                                    ref toAddListVariable,
                                                                    recursePropertyList, keyMemberList);

                #endregion
            }
            else
            {
                #region INTENTION var toAdd = source.Where(s => !destination.Select(d => d.Id).Contains(s.Id))

                assignToAddToList = GetElementsToAdd(destinationGenericType, sourceGenericType,
                                                        sourceKeyType, destinationKeyType,
                                                        srcMemberExp, dstMemberExp,
                                                        ref toAddListVariable,
                                                        recursePropertyList, keyMemberName);

                #endregion
            }

            #endregion

            #region ADD ELEMENTS FROM SOURCE TO DESTINATION

            MethodCallExpression forEachAdd = AddElementsFromSourceToDestination(destinationGenericType, sourceGenericType,
                dstMemberExp, ref toAddListVariable, recursePropertyList,
                flatteningToken);

            #endregion

            #region UPDATE REMAINING ITEMS

            MethodCallExpression forEachSourceExceptToAddUpdate = null;

            if (keyMemberList != null && keyMemberList.Count > 1)
            {
                #region INTENTION: source.Except(toAdd).ForEach(u => u.Project().To<BasicEntityWithComplexKeyAttributes>(destination.FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)))

                forEachSourceExceptToAddUpdate = UpdateRemainingElementsComplexKey(destinationGenericType, sourceGenericType,
                                                                srcMemberExp, dstMemberExp,
                                                                ref toAddListVariable, keyMemberList,
                                                                clearCollections, recursePropertyList,
                                                                flatteningToken);
                #endregion
            }
            else
            {
                #region INTENTION: source.Except(toAdd).ForEach(u => u.Project().To<BasicEntity>(destination.FirstOrDefault(d => d.Id == u.Id)))

                forEachSourceExceptToAddUpdate = UpdateRemainingElements(destinationGenericType, sourceGenericType,
                                                                            srcMemberExp, dstMemberExp,
                                                                            ref toAddListVariable, keyMemberName,
                                                                            clearCollections, recursePropertyList,
                                                                            flatteningToken);

                #endregion
            }

            #endregion

            List<Expression> enumerableProjectionExpressionList = new List<Expression>()
            {
                assignToRemoveToList,
                forEachRemove,
                assignToAddToList,
                forEachAdd,
                forEachSourceExceptToAddUpdate
            };

            List<ParameterExpression> enumerableProjectionExpressionVariablesList = new List<ParameterExpression>()
            {
                toRemoveListVariable,
                toAddListVariable
            };

            if (clearCollectionExpression != null)
            {
                enumerableProjectionExpressionList.Insert(index: 0, item: clearCollectionExpression);
            }

            BlockExpression enumerableProjectionExpressionListBlock = ExpressionHelper.ExpressionBlockHelper(enumerableProjectionExpressionList, enumerableProjectionExpressionVariablesList);

            return enumerableProjectionExpressionListBlock;

            #endregion
        }

        protected Expression<Func<TSource, TDest>> BuildExpression<TDest>(string flatteningToken = null)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource), name: "src");

            IEnumerable<MemberAssignment> bindings = GetPropertyAndFieldBindings<TSource, TDest>(parameterExpression, flatteningToken);

            Expression<Func<TSource, TDest>> expression = Expression.Lambda<Func<TSource, TDest>>(Expression.MemberInit(Expression.New(typeof(TDest)), bindings), parameterExpression);

            string key = GetCacheKey<Expression<Func<TSource, TDest>>>(flatteningToken);

            ExpressionCache.TryAdd(key, expression);

            return expression;
        }

        protected Expression BuildAssignment(Expression parameterExpression, Expression destinationParameterExpression, MemberInfo destinationMember, IEnumerable<MemberInfo> sourceMembers, string keyMemberName, bool clearCollections, string flatteningToken = null, List<string> recursePropertyList = null)
        {
            if (recursePropertyList == null)
            {
                recursePropertyList = new List<string>();
            }

            MemberInfo sourceMember = GetSourceMember(destinationMember.Name, sourceMembers, recursePropertyList, flatteningToken);

            if (sourceMember != null)
            {
                Type underlyingType = sourceMember.GetUnderlyingType();
                TypeInfo typeInfo = underlyingType.GetTypeInfo();

                MemberExpression srcMemberExp = BuildMemberExpression(parameterExpression, sourceMember.Name, recursePropertyList);
                MemberExpression dstMemberExp = BuildMemberExpression(destinationParameterExpression, destinationMember.Name, recursePropertyList);

                if (typeInfo.IsPrimitive || typeInfo.IsValueType || underlyingType == typeof(string))
                {
                    #region Member Type is primitive, struct or string

                    return Expression.Assign(dstMemberExp, srcMemberExp);

                    #endregion
                }
                else if (typeInfo.ImplementedInterfaces.Contains(typeof(IEnumerable)))
                {
                    #region Member Type is enumerable

                    Type sourceUnderlyingEnumerableType = sourceMember.GetUnderlyingType();
                    Type sourceUnderlyingGenericEnumerableType = sourceUnderlyingEnumerableType.GenericTypeArguments[0];

                    Type destinationUndelyingEnumerableType = destinationMember.GetUnderlyingType();
                    Type destinationUnderlyingGenericEnumerableType = destinationUndelyingEnumerableType.GenericTypeArguments[0];

                    Expression projectEnumerableOverEnumerableExpression = ProjectEnumerableOverEnumerable(sourceUnderlyingGenericEnumerableType,
                        destinationUnderlyingGenericEnumerableType,
                        srcMemberExp,
                        dstMemberExp,
                        keyMemberName,
                        clearCollections,
                        flatteningToken,
                        recursePropertyList);

                    return projectEnumerableOverEnumerableExpression;

                    #endregion
                }
                else
                {
                    #region Member Type is a class

                    Type destinationUnderlyingType = destinationMember.GetUnderlyingType();

                    recursePropertyList.Add(sourceMember.Name);

                    List<Expression> assignments = GetMemberAssignmentExpressions(underlyingType, destinationUnderlyingType, parameterExpression, destinationParameterExpression, keyMemberName, clearCollections, flatteningToken, recursePropertyList).ToList();

                    BlockExpression assignToNullBlock = ExpressionHelper.ExpressionBlockHelper(Expression.Assign(dstMemberExp, Expression.Constant(value: null, type: destinationUnderlyingType)));
                    BlockExpression assignBlock = ExpressionHelper.ExpressionBlockHelper(assignments);

                    Expression ifSourceNullThenExpression = Expression.Condition(
                        ExpressionHelper.GetCompareToNullExpression(srcMemberExp, underlyingType),
                        assignToNullBlock,
                        assignBlock);

                    recursePropertyList.Remove(sourceMember.Name);

                    return ifSourceNullThenExpression;

                    #endregion
                }
            }

            return null;
        }

        protected MemberAssignment BuildBinding(Expression parameterExpression, MemberInfo destinationMember, IEnumerable<MemberInfo> sourceMembers, string flatteningToken = null, List<string> recursePropertyList = null)
        {
            if (recursePropertyList == null)
            {
                recursePropertyList = new List<string>();
            }

            MemberInfo sourceMember = GetSourceMember(destinationMember.Name, sourceMembers, recursePropertyList, flatteningToken);

            if (sourceMember != null)
            {
                Type underlyingType = sourceMember.GetUnderlyingType();
                TypeInfo typeInfo = underlyingType.GetTypeInfo();

                MemberExpression memberExp = BuildMemberExpression(parameterExpression, sourceMember.Name, recursePropertyList);

                if (typeInfo.IsPrimitive || typeInfo.IsValueType || underlyingType == typeof(string))
                {
                    #region Member Type is primitive, struct or string

                    return Expression.Bind(destinationMember, memberExp);

                    #endregion
                }
                else if (typeInfo.ImplementedInterfaces.Contains(typeof(IEnumerable)))
                {
                    #region Member Type is enumerable

                    Type sourceUnderlyingEnumerableType = sourceMember.GetUnderlyingType();
                    Type sourceUnderlyingGenericEnumerableType = sourceUnderlyingEnumerableType.GenericTypeArguments[0];

                    Type destinationUndelyingEnumerableType = destinationMember.GetUnderlyingType();
                    Type destinationGenericUndelyingType = destinationUndelyingEnumerableType.GenericTypeArguments[0];

                    ParameterExpression enumerableParameter = Expression.Parameter(sourceUnderlyingGenericEnumerableType, name: "en");
                    IEnumerable<MemberAssignment> bindings = GetPropertyAndFieldBindings(sourceUnderlyingGenericEnumerableType, destinationGenericUndelyingType, enumerableParameter, flatteningToken);

                    Expression destinationMemberExp = Expression.MemberInit(Expression.New(destinationGenericUndelyingType), bindings);

                    MethodCallExpression selectExpression = ExpressionHelper.GetSelectOverEnumerable(sourceUnderlyingGenericEnumerableType, destinationGenericUndelyingType, enumerableParameter, memberExp, destinationMemberExp);
                    MethodCallExpression listExpression = ExpressionHelper.GetToListOverEnumerable(destinationGenericUndelyingType, selectExpression);

                    Expression ifNullThenExpression = Expression.Condition(
                        ExpressionHelper.GetCompareToNullExpression(memberExp, underlyingType),
                        Expression.Bind(destinationMember, Expression.Constant(value: null, type: listExpression.Type)).Expression,
                        listExpression);

                    return Expression.Bind(destinationMember, ifNullThenExpression);

                    #endregion
                }
                else
                {
                    #region Member Type is a class

                    Type destinationUnderlyingType = destinationMember.GetUnderlyingType();

                    recursePropertyList.Add(sourceMember.Name);

                    IEnumerable<MemberAssignment> bindings = GetPropertyAndFieldBindings(underlyingType, destinationUnderlyingType, parameterExpression, flatteningToken, recursePropertyList);

                    Expression ifNullThenExpression = Expression.Condition(
                        ExpressionHelper.GetCompareToNullExpression(memberExp, underlyingType),
                        Expression.Bind(destinationMember, Expression.Constant(value: null, type: destinationUnderlyingType)).Expression,
                        Expression.MemberInit(Expression.New(destinationUnderlyingType), bindings));

                    recursePropertyList.Remove(sourceMember.Name);

                    return Expression.Bind(destinationMember, ifNullThenExpression);

                    #endregion
                }
            }

            return null;
        }

        #endregion

        #region Support methods

        private MemberExpression BuildMemberExpression(Expression parameterExpression, string sourceMemberName, List<string> recursePropertyList)
        {
            MemberExpression pathExpression = null;

            if (recursePropertyList != null)
            {
                foreach (string recurseProperty in recursePropertyList)
                {
                    if (pathExpression == null)
                    {
                        pathExpression = Expression.PropertyOrField(parameterExpression, recurseProperty);
                    }
                    else
                    {
                        pathExpression = Expression.PropertyOrField(pathExpression, recurseProperty);
                    }
                }
            }
            MemberExpression memberExp = Expression.PropertyOrField(pathExpression ?? parameterExpression, sourceMemberName);

            return memberExp;
        }

        private MemberInfo GetSourceMember(string destinationMemberName, IEnumerable<MemberInfo> sourceMembers, List<string> recursePropertyList, string flatteningToken = null)
        {
            if (recursePropertyList == null)
            {
                throw new ArgumentNullException(paramName: "The recurse property list cannot be null, it has to be provided initialized previously");
            }

            MemberInfo sourceMember = sourceMembers.FirstOrDefault(src => src.Name == destinationMemberName);

            if (sourceMember == null)
            {
                #region Attempting mapping by convention

                string[] memberNames = null;

                if (flatteningToken == null || flatteningToken == string.Empty)
                {
                    memberNames = SplitCamelCase(destinationMemberName);
                }
                else
                {
                    memberNames = SplitByToken(destinationMemberName, flatteningToken);
                }

                if (memberNames.Length % 2 == 0)
                {
                    for (int n = 0; memberNames.Length > n; n++)
                    {
                        if (n != memberNames.Length - 1)
                        {
                            recursePropertyList.Add(memberNames[n]);
                        }
                        else
                        {
                            sourceMember = sourceMembers.FirstOrDefault(src => src.Name == memberNames[n]);
                        }
                    }
                }

                #endregion
            }
            return sourceMember;
        }

        protected string GetCacheKey<TDest>(string flatteningToken, string keyMemberName = null, bool clearCollections = false)
        {
            return string.Concat(typeof(TSource).FullName, typeof(TDest).FullName, keyMemberName ?? string.Empty, flatteningToken ?? string.Empty, clearCollections.ToString());
        }

        protected string[] SplitCamelCase(string input)
        {
            return Regex.Replace(input, pattern: "([A-Z])", replacement: " $1", options: RegexOptions.None).Trim().Split(separator: new char[] { ' ' });
        }

        protected string[] SplitByToken(string input, string splitTokken)
        {
            return input.Trim().Split(new string[] { splitTokken }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}