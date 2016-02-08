using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.DDD.Attributes
{
    /// <summary>
    /// Marks the property as a ValueObject for the entity and it will provide an abstraction
    /// to fields of the entity. If we want to customize the name of the fields in which it will be
    /// persisted, we can provide parameters to theconstructor in which we will pass strings defining
    /// tuples between brackets (Eg: "{ValueObjectField,DBField}").
    /// The type of the value object has to be supplied as a parameter, if no property mappings are supplied
    /// the default names of the properties will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueObjectAttribute : Attribute
    {

        private ReadOnlyDictionary<string, string> propertyMappingsDictionary;
        /// <summary>
        /// Dictionary containing the customized mappings between properties of the value object
        /// and fields in the table of the persistence system.
        /// </summary>
        public ReadOnlyDictionary<string, string> PropertyMappingsDictionary
        {
            get { return propertyMappingsDictionary; }
            private set { propertyMappingsDictionary = value; }
        }


        private Type valueObjectType;
        /// <summary>
        /// The type of the value object the property decorated with this attributes belongs to
        /// </summary>
        public Type ValueObjectType
        {
            get { return valueObjectType; }
            private set { valueObjectType = value; }
        }

        /// <summary>
        /// The type of the value object needs to be supplied as a parameter.
        /// Also this constructor accepts a collection of tuples defined between brackets
        /// (Eg: "{ValueObjectField,DBField}") expressing a custom mapping between
        /// a specific value object property and a field in the persistence system.
        /// If no tuples are provided as parameters, the existing ValueObject property names will 
        /// be used to provide persistence
        /// </summary>
        /// <param name="propertyTuples">Collection of tuples defined between brackets
        /// (Eg: "{ValueObjectField,DBField}").</param>
        /// <param name="ValueObjectType">The type of value object this attribute is going to be applied on</param>
        public ValueObjectAttribute(Type valueObjectType, params string[] propertyTuples)
        {
            this.ValueObjectType = valueObjectType;
            Dictionary<string, string> tempPropertyMappingDictionary = new Dictionary<string, string>();

            if (propertyTuples != null)
            {
                foreach (string tuple in propertyTuples)
                {
                    List<string> properties = tuple.Replace(oldValue: "{", newValue: string.Empty).Replace(oldValue: "}", newValue: string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (properties.Count() != 2)
                    {
                        throw new ArgumentException(message: "The property/field mapping should be expressed in tuples defined following the \"{ValueObjectField,DBField}\" pattern. The defined pattern was missing items or having too many items");
                    }

                    if (tempPropertyMappingDictionary.ContainsKey(properties[0]))
                    {
                        throw new ArgumentException(message: "Duplicated property name, a property name can only appear once");
                    }
                    else
                    {
                        if (ValueObjectType.GetRuntimePropertyEx(properties[0]) == null)
                        {
                            throw new ArgumentException(message: "The property doesn't exist in the Value Object Type");
                        }

                        if (tempPropertyMappingDictionary.Values.Contains(properties[1]))
                        {
                            throw new ArgumentException(message: "Duplicated field name, a field name can only appear once");
                        }
                        tempPropertyMappingDictionary.Add(properties[0], properties[1]);
                    }
                }
            }

            PropertyMappingsDictionary = new ReadOnlyDictionary<string, string>(tempPropertyMappingDictionary);
        }
    }
}
