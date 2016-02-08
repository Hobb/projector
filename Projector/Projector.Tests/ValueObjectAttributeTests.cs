using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Projector.TestDomain.DDD.Attributes;
using Projector.TestDomain.DDD.ValueObjects;

namespace Projector.TestDomain.DDD.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ValueObjectAttributeTests
    {
        [TestMethod]
        public void TheAttributeCanBeCreatedJustWithType_NoException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange));

            var expected = true;
            var actual = (attribute.PropertyMappingsDictionary != null) && (attribute.PropertyMappingsDictionary.Count == 0);

            Assert.IsTrue(expected == actual, message: "Calling the constructor with one parameter should initialize an empty property mapping dictionary");
        }

        [TestMethod]
        public void TheAttributeCanBeCreatedWithCustomMappingParameters_NoException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange), propertyTuples: new string[] { "{Start,FechaInicio}", "{End,FechaFin}" });

            var expected = true;
            var actual = (attribute.PropertyMappingsDictionary != null) && (attribute.PropertyMappingsDictionary.Count == 2);

            Assert.IsTrue(expected == actual, message: "Calling the parametrized constructor should initialize the property mapping dictionary, parsing the string passed as parameter");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),"The attribute should not allow for strings missing members to create a tuple")]
        public void IfTheAttributeIsCreatedWithTooFewMappingParameters_ArgumentException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange), propertyTuples: new string[] { "{,FechaInicio}", "{End,FechaFin}" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The attribute should not allow for strings with too many members to create a tuple")]
        public void IfTheAttributeIsCreatedWithTooManyMappingParameters_ArgumentException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange), propertyTuples: new string[] { "{Start,FechaInicio,InicioFecha}", "{End,FechaFin}" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The attribute should not allow for a mapping property to be defined twice in the tuples")]
        public void IfTheAttributeTriesToInsertSamePropertyTwice_ArgumentException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange), propertyTuples: new string[] { "{Start,FechaInicio}", "{Start,FechaFin}" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The attribute should not allow for a mapping field to be defined twice in the tuples")]
        public void IfTheAttributeTriesToInsertSameFieldTwice_ArgumentException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange), propertyTuples: new string[] { "{Start,FechaInicio}", "{End,FechaInicio}" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The attribute should not allow for properties not existing in the value object to be defined in the tuples")]
        public void IfTheAttributeTriesToInsertAPropertyNotExistantInValueObject_ArgumentException()
        {
            ValueObjectAttribute attribute = new ValueObjectAttribute(typeof(DateRange), propertyTuples: new string[] { "{StartDate,FechaInicio}", "{End,FechaInicio}" });
        }
    }
}
