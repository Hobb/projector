using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Extensions focused on reducing the use of reflection by caching all the possible data related to the types
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// All the property infos for the types being interrogated by reflection will be cached in this dictionary, to make it thread safe we will
    /// make it static only inside the thread scope
    /// </summary>
    [ThreadStatic]
    private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> cachedPropertyDictionary = new Dictionary<Type, IEnumerable<PropertyInfo>>();

    /// <summary>
    /// All the field infos for the types being interrogated by reflection will be cached in this dictionary, to make it thread safe we will
    /// make it static only inside the thread scope
    /// </summary>
    [ThreadStatic]
    private static readonly IDictionary<Type, IEnumerable<FieldInfo>> cachedFieldDictionary = new Dictionary<Type, IEnumerable<FieldInfo>>();


    /// <summary>
    /// Will get the property info objects from the type trying to retrieve them first from cache
    /// </summary>
    /// <param name="type">Type from which we want to know info about its properties</param>
    /// <returns>Enumerable with all the PropertyInfo objects related to the type</returns>
    public static IEnumerable<PropertyInfo> GetRuntimePropertiesEx(this Type type)
    {
        if (!cachedPropertyDictionary.ContainsKey(type))
        {
            cachedPropertyDictionary.Add(type, type.GetRuntimeProperties());
        }

        return cachedPropertyDictionary[type];
    }

    /// <summary>
    /// Will get a specific property info object, matching the name passed as a parameter, trying to retrieve it first from cache.
    /// If the property cannot be found, null will be returned.
    /// </summary>
    /// <param name="type">Type from which we want to know info about the property</param>
    /// <param name="name">The name of the property we want to retrieve</param>
    /// <returns>The property info object with information about the property or null in case no property could be found</returns>
    public static PropertyInfo GetRuntimePropertyEx(this Type type, string name)
    {
        return GetRuntimePropertiesEx(type).Where(c => c.Name == name).FirstOrDefault();
    }

    /// <summary>
    /// Will get the field info objects from the type trying to retrieve them first from cache
    /// </summary>
    /// <param name="type">Type from which we want to know info about its field</param>
    /// <returns>Enumerable with all the FieldInfo objects related to the type</returns>
    public static IEnumerable<FieldInfo> GetRuntimeFieldsEx(this Type type)
    {
        if (!cachedFieldDictionary.ContainsKey(type))
        {
            cachedFieldDictionary.Add(type, type.GetRuntimeFields());
        }

        return cachedFieldDictionary[type];
    }

    /// <summary>
    /// Will get a specific field info object, matching the name passed as a parameter, trying to retrieve it first from cache.
    /// If the field cannot be found, null will be returned.
    /// </summary>
    /// <param name="type">Type from which we want to know info about the field</param>
    /// <param name="name">The name of the field we want to retrieve</param>
    /// <returns>The field info object with information about the field or null in case no field could be found</returns>
    public static FieldInfo GetRuntimeFieldEx(this Type type, string name)
    {
        return GetRuntimeFieldsEx(type).Where(c => c.Name == name).FirstOrDefault();
    }

    /// <summary>
    /// Will get all the field and property members of the type irrespective of their type, properties without a setter or getter will not be retrieved.
    /// </summary>
    /// <param name="type">The type whose fields and properties we want to obtain</param>
    /// <param name="includePrivateFields">Specifies if we want to include private fields</param>
    /// <param name="includePrivatePropertyGetters">Specifies if we want to include properties with a private getter</param>
    /// <param name="includePrivatePropertySetters">Specifies if we want to include properties with a private setter</param>
    /// <returns>An enumerable with all the MemberInfo objects representing the fields and properties of said type</returns>
    public static IEnumerable<MemberInfo> GetRuntimeFieldsAndPropertiesEx(this Type type, bool includePrivateFields = false, bool includePrivatePropertySetters = false, bool includePrivatePropertyGetters = false)
    {
        return GetRuntimePropertiesEx(type).Where(c => c.CanWrite && (c.SetMethod != null && (c.SetMethod.IsPublic != includePrivatePropertySetters))
                                                                  && (c.GetMethod != null && (c.GetMethod.IsPublic != includePrivatePropertyGetters)))
            .Union<MemberInfo>(GetRuntimeFieldsEx(type).Where(c => c.IsPublic != includePrivateFields));
    }

    /// <summary>
    /// Gets the underlying type of a memberinfo without having to cast it to the derived type
    /// </summary>
    /// <param name="member">A normal memberinfo</param>
    /// <returns>The type of the member or the return type in case of a method</returns>
    public static Type GetUnderlyingType(this MemberInfo member)
    {
        if (member is PropertyInfo)
        {
            return ((PropertyInfo)member).PropertyType;
        }
        else if (member is FieldInfo)
        {
            return ((FieldInfo)member).FieldType;
        }
        else if (member is MethodInfo)
        {
            return ((MethodInfo)member).ReturnType;
        }
        else if (member is EventInfo)
        {
            return ((EventInfo)member).EventHandlerType;
        }
        else
            throw new ArgumentException
            (
                message: "Input MemberInfo must be of type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
            );
    }
}

