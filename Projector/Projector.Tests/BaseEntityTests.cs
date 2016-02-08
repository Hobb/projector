using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Projector.TestDomain.DDD.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BaseEntityTests
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "An Entity key cannot be allowed to change once is set")]
        public void AttemptingToChangeKeyWhenItHasBeenSet_InvalidOperationException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;
            firstEntity.Id = 2;
        }

        [TestMethod]
        public void Provided2EntitiesWithSameKeyButDifferentProps_AreEqual_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();
            TestDomain.Entities.BasicEntity secondEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;
            secondEntity.Id = 1;

            firstEntity.Name = "EntityOne";
            secondEntity.Name = "EntityTwo";

            var expected = true;
            var actual = firstEntity == secondEntity;


            Assert.IsTrue(actual == expected, message: "Both entities should be equal due to having the same key");
        }

        [TestMethod]
        public void Provided2EntitiesWithDifferentKeyAndSameProps_AreNotEqual_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();
            TestDomain.Entities.BasicEntity secondEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;
            secondEntity.Id = 2;

            firstEntity.Name = "SameProp";
            secondEntity.Name = "SameProp";

            var expected = true;
            var actual = firstEntity != secondEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should not be equal due to not having the same key");
        }

        [TestMethod]
        public void ComparingEntityWithItself_ReturnsTrue_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;

            firstEntity.Name = "EntityOne";

            var expected = true;
#pragma warning disable CS1718 // Comparison made to same variable
            var actual = firstEntity == firstEntity;
#pragma warning restore CS1718 // Comparison made to same variable

            Assert.IsTrue(actual == expected, message: "The entity should be equal to itself");
        }


        [TestMethod]
        public void ComparingEntityWithNull_ReturnsFalse_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;

            firstEntity.Name = "EntityOne";

            var expected = true;
            var actual = firstEntity != null;

            Assert.IsTrue(actual == expected, message: "The entity should not be equal to null");
        }

        [TestMethod]
        public void ComparingEntityWithDifferentObject_ReturnsFalse_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;

            firstEntity.Name = "EntityOne";

            var expected = true;
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
            var actual = firstEntity != new object();
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast

            Assert.IsTrue(actual == expected, message: "The entity should not be equal to an entity of different type");
        }

        [TestMethod]
        public void ComparingEntityWithDifferentObjectUsingEquals_ReturnsFalse_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;

            firstEntity.Name = "EntityOne";

            var expected = true;
            var actual = !firstEntity.Equals(new object());

            Assert.IsTrue(actual == expected, message: "The entity should not be equal to an entity of different type");
        }

        [TestMethod]
        public void ComparingEntityWithNullObjectUsingEquals_ReturnsFalse_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;
            object nullObject = null;

            firstEntity.Name = "EntityOne";

            var expected = true;
            var actual = !firstEntity.Equals(nullObject);

            Assert.IsTrue(actual == expected, message: "The entity should not be equal to a null entity of different type");
        }

        [TestMethod]
        public void ComparingEntityWithItselfAsObjectUsingEquals_ReturnsTrue_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();

            firstEntity.Id = 1;

            firstEntity.Name = "EntityOne";

            var expected = true;
            var actual = firstEntity.Equals((object)firstEntity);

            Assert.IsTrue(actual == expected, message: "The entity should be equal to itself when using equals even when casted as object (same reference)");
        }

        [TestMethod]
        public void ComparingEntityWithEntityOfDifferentTypeEvenBeingSimilar_IsNotEqual_NoException()
        {
            TestDomain.Entities.BasicEntity firstEntity = new TestDomain.Entities.BasicEntity();
            TestDomain.Entities.DifferentBasicEntity differentEntity = new TestDomain.Entities.DifferentBasicEntity();

            firstEntity.Id = 1;
            differentEntity.Id = 1;

            firstEntity.Name = "BothEntitiesSameData";
            differentEntity.Name = "BothEntitiesSameData";

            var expected = true;
            var actual = firstEntity != differentEntity;

            Assert.IsTrue(actual == expected, message: "The entity should not be equal to an entity of different type, regardless of them being identical in data");
        }

        [TestMethod]
        public void ComparingEntitiesWithSameComplexStringKey_IsEqual_NoException()
        {
            TestDomain.Entities.ComplexStringKeyEntity firstStringEntity = new TestDomain.Entities.ComplexStringKeyEntity();
            TestDomain.Entities.ComplexStringKeyEntity secondStringEntity = new TestDomain.Entities.ComplexStringKeyEntity();

            firstStringEntity.Id = new TestDomain.Keys.StringStructKey() { Id = "1" };
            secondStringEntity.Id = new TestDomain.Keys.StringStructKey() { Id = "1" };

            firstStringEntity.Name = "FirstProp";
            secondStringEntity.Name = "DifferentProp";

            var expected = true;
            var actual = firstStringEntity == secondStringEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should be equal due to having the same complex string key");
        }

        [TestMethod]
        public void ComparingEntitiesWithDifferentComplexStringKey_IsNotEqual_NoException()
        {
            TestDomain.Entities.ComplexStringKeyEntity firstStringEntity = new TestDomain.Entities.ComplexStringKeyEntity();
            TestDomain.Entities.ComplexStringKeyEntity secondStringEntity = new TestDomain.Entities.ComplexStringKeyEntity();

            firstStringEntity.Id = new TestDomain.Keys.StringStructKey() { Id = "1" };
            secondStringEntity.Id = new TestDomain.Keys.StringStructKey() { Id = "2" };

            firstStringEntity.Name = "SameProp";
            secondStringEntity.Name = "SameProp";

            var expected = false;
            var actual = firstStringEntity == secondStringEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should not be equal due to having different complex string key");
        }

        [TestMethod]
        public void ComparingEntitiesWithDifferentGuidKey_IsNotEqual_NoException()
        {
            TestDomain.Entities.GuidKeyEntity firstGuidEntity = new TestDomain.Entities.GuidKeyEntity();
            TestDomain.Entities.GuidKeyEntity secondGuidEntity = new TestDomain.Entities.GuidKeyEntity();

            firstGuidEntity.Id = Guid.NewGuid();
            secondGuidEntity.Id = Guid.NewGuid();

            firstGuidEntity.Name = "SameProp";
            secondGuidEntity.Name = "SameProp";

            var expected = false;
            var actual = firstGuidEntity == secondGuidEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should not be equal due to having different Guids key");
        }

        [TestMethod]
        public void ComparingEntitiesWithSameGuidKey_IsEqual_NoException()
        {
            TestDomain.Entities.GuidKeyEntity firstGuidEntity = new TestDomain.Entities.GuidKeyEntity();
            TestDomain.Entities.GuidKeyEntity secondGuidEntity = new TestDomain.Entities.GuidKeyEntity();

            Guid guidToShare = Guid.NewGuid();

            firstGuidEntity.Id = guidToShare;
            secondGuidEntity.Id = guidToShare;

            firstGuidEntity.Name = "FirstProp";
            secondGuidEntity.Name = "SecondProp";

            var expected = true;
            var actual = firstGuidEntity == secondGuidEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should be equal due to having same Guid key");
        }

        [TestMethod]
        public void ComparingEntitiesWithSameVeryComplexKey_IsEqual_NoException()
        {
            TestDomain.Entities.VeryComplexKeyEntity firstVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();
            TestDomain.Entities.VeryComplexKeyEntity secondVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyToShare = new TestDomain.Keys.VeryComplexStructKey()
                                                                                {
                                                                                    NumericId = 1,
                                                                                    TextId = "2",
                                                                                    UniqueGuidId = Guid.NewGuid()
                                                                                };

            firstVeryComplexEntity.Id = veryComplexKeyToShare;
            secondVeryComplexEntity.Id = veryComplexKeyToShare;

            firstVeryComplexEntity.Name = "FirstProp";
            secondVeryComplexEntity.Name = "SecondProp";

            var expected = true;
            var actual = firstVeryComplexEntity == secondVeryComplexEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should be equal due to having same very complex key");
        }

        [TestMethod]
        public void ComparingEntitiesWithDifferentVeryComplexKey_IsNotEqual_NoException()
        {
            TestDomain.Entities.VeryComplexKeyEntity firstVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();
            TestDomain.Entities.VeryComplexKeyEntity secondVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyOne = new TestDomain.Keys.VeryComplexStructKey()
            {
                NumericId = 1,
                TextId = "2",
                UniqueGuidId = Guid.NewGuid()
            };

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyTwo = new TestDomain.Keys.VeryComplexStructKey()
            {
                NumericId = 3,
                TextId = "4",
                UniqueGuidId = Guid.NewGuid()
            };

            firstVeryComplexEntity.Id = veryComplexKeyOne;
            secondVeryComplexEntity.Id = veryComplexKeyTwo;

            firstVeryComplexEntity.Name = "SameProp";
            secondVeryComplexEntity.Name = "SameProp";

            var expected = false;
            var actual = firstVeryComplexEntity == secondVeryComplexEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should not be equal due to having different very complex key");
        }

        [TestMethod]
        public void ComparingEntitiesWithDifferentVeryComplexKeyCanHandleNulls_IsEqual_NoException()
        {
            TestDomain.Entities.VeryComplexKeyEntity firstVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();
            TestDomain.Entities.VeryComplexKeyEntity secondVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyToShare = new TestDomain.Keys.VeryComplexStructKey()
            {
                NumericId = 1,
                TextId = null,
                UniqueGuidId = Guid.NewGuid()
            };

            firstVeryComplexEntity.Id = veryComplexKeyToShare;
            secondVeryComplexEntity.Id = veryComplexKeyToShare;

            firstVeryComplexEntity.Name = "FirstProp";
            secondVeryComplexEntity.Name = "SecondProp";

            var expected = true;
            var actual = firstVeryComplexEntity == secondVeryComplexEntity;

            Assert.IsTrue(actual == expected, message: "Both entities should be equal due to having same very complex key");
        }

        [TestMethod]
        public void EntitiesCanBeSuccesfullyUsedInDictionariesAsKeys_True_NoException()
        {
            TestDomain.Entities.VeryComplexKeyEntity firstVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();
            TestDomain.Entities.VeryComplexKeyEntity secondVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyOne = new TestDomain.Keys.VeryComplexStructKey()
            {
                NumericId = 1,
                TextId = "2",
                UniqueGuidId = Guid.NewGuid()
            };

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyTwo = new TestDomain.Keys.VeryComplexStructKey()
            {
                NumericId = 3,
                TextId = "4",
                UniqueGuidId = Guid.NewGuid()
            };

            firstVeryComplexEntity.Id = veryComplexKeyOne;
            secondVeryComplexEntity.Id = veryComplexKeyTwo;

            firstVeryComplexEntity.Name = "SameProp";
            secondVeryComplexEntity.Name = "SameProp";

            Dictionary<TestDomain.Entities.VeryComplexKeyEntity, string> entityDict = new Dictionary<TestDomain.Entities.VeryComplexKeyEntity, string>();

            entityDict.Add(firstVeryComplexEntity, value: "FirstEntity");
            entityDict.Add(secondVeryComplexEntity, value: "SecondEntity");

            var expected = true;
            var actual = (entityDict[firstVeryComplexEntity] == "FirstEntity") && (entityDict[secondVeryComplexEntity] == "SecondEntity");

            Assert.IsTrue(actual == expected, message: "You should be able to use the entity as Key, taking into account only its key value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Trying to add the same entity to a dictionary twice should throw ArgumentException")]
        public void YouCannotAddTwiceTheSameEntityAsKeyToADictionary_ArgumentException()
        {
            TestDomain.Entities.VeryComplexKeyEntity firstVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();
            TestDomain.Entities.VeryComplexKeyEntity secondVeryComplexEntity = new TestDomain.Entities.VeryComplexKeyEntity();

            TestDomain.Keys.VeryComplexStructKey veryComplexKeyToShare = new TestDomain.Keys.VeryComplexStructKey()
            {
                NumericId = 1,
                TextId = "2",
                UniqueGuidId = Guid.NewGuid()
            };

            firstVeryComplexEntity.Id = veryComplexKeyToShare;
            secondVeryComplexEntity.Id = veryComplexKeyToShare;

            firstVeryComplexEntity.Name = "FirstProp";
            secondVeryComplexEntity.Name = "SecondProp";

            Dictionary<TestDomain.Entities.VeryComplexKeyEntity, string> entityDict = new Dictionary<TestDomain.Entities.VeryComplexKeyEntity, string>();

            entityDict.Add(firstVeryComplexEntity, value: "FirstEntity");
            entityDict.Add(secondVeryComplexEntity, value: "SecondEntity");
        }


        [TestMethod]
        public void HashCollisionsAreWithinLimits_ReturnsTrue_NoException()
        {
            Dictionary<TestDomain.Entities.BasicEntity, string> entityDict = new Dictionary<TestDomain.Entities.BasicEntity, string>();
            object syncObject = new object();
            int errors = 0;
            int maxIterations = 1000000;

            Parallel.For(fromInclusive: 0, toExclusive: maxIterations, body: (iteration) =>
                     {
                         try
                         {
                             TestDomain.Entities.BasicEntity entity = new TestDomain.Entities.BasicEntity();

                             entity.Id = iteration;

                             entity.Name = "Entity " + iteration;

                             lock (syncObject)
                             {
                                 entityDict.Add(entity, iteration.ToString());
                             }
                         }
                         catch
                         {
                             Interlocked.Increment(ref errors);
                         }
                     });


            var limit = 1;
            var expected = true;
            var actual = (errors < limit);

            Assert.IsTrue(actual == expected, message: "The hashing algoritm generates more than " + limit + " collision per " + maxIterations + " entities");
            Assert.IsTrue(entityDict.Count == maxIterations - errors, message: "Something wrong happened running the test, the right number of elements where not introduced in the dictionary");
        }

        [TestMethod]
        public void HashCollisionsAreWithinLimitsMoreComplexKey_ReturnsTrue_NoException()
        {
            Dictionary<TestDomain.Entities.ComplexStringKeyEntity, string> entityDict = new Dictionary<TestDomain.Entities.ComplexStringKeyEntity, string>();
            object syncObject = new object();
            int errors = 0;
            int maxIterations = 1000000;

            Parallel.For(fromInclusive: 0, toExclusive: maxIterations, body: (iteration) =>
                 {
                     try
                     {
                         TestDomain.Entities.ComplexStringKeyEntity entity = new TestDomain.Entities.ComplexStringKeyEntity();

                         entity.Id = new TestDomain.Keys.StringStructKey() { Id = iteration.ToString() };

                         entity.Name = "Entity " + iteration;

                         lock (syncObject)
                         {
                             entityDict.Add(entity, iteration.ToString());
                         }
                     }
                     catch
                     {
                         Interlocked.Increment(ref errors);
                     }
                 });

            var limit = 1;
            var expected = true;
            var actual = (errors < limit);

            Assert.IsTrue(actual == expected, message: "The hashing algoritm generates more than " + limit + " collision per " + maxIterations + " entities");
            Assert.IsTrue(entityDict.Count == maxIterations - errors, message: "Something wrong happened running the test, the right number of elements where not introduced in the dictionary");
        }

        [TestMethod]
        public void HashCollisionsAreWithinLimitsGuidKey_ReturnsTrue_NoException()
        {
            Dictionary<TestDomain.Entities.GuidKeyEntity, string> entityDict = new Dictionary<TestDomain.Entities.GuidKeyEntity, string>();
            object syncObject = new object();
            int errors = 0;
            int maxIterations = 1000000;

            Parallel.For(fromInclusive: 0, toExclusive: maxIterations, body: (iteration) =>
                 {
                     try
                     {
                         TestDomain.Entities.GuidKeyEntity entity = new TestDomain.Entities.GuidKeyEntity();

                         entity.Id = Guid.NewGuid();

                         entity.Name = "Entity " + iteration;

                         lock (syncObject)
                         {
                             entityDict.Add(entity, iteration.ToString());
                         }
                     }
                     catch
                     {
                         Interlocked.Increment(ref errors);
                     }
                 });

            var limit = 1;
            var expected = true;
            var actual = (errors < limit);

            Assert.IsTrue(actual == expected, message: "The hashing algoritm generates more than " + limit + " collision per " + maxIterations + " entities");
            Assert.IsTrue(entityDict.Count == maxIterations - errors, message: "Something wrong happened running the test, the right number of elements where not introduced in the dictionary");
        }

        [TestMethod]
        public void HashCollisionsAreWithinLimitsVeryComplexKey_ReturnsTrue_NoException()
        {
            Dictionary<TestDomain.Entities.VeryComplexKeyEntity, string> entityDict = new Dictionary<TestDomain.Entities.VeryComplexKeyEntity, string>();
            object syncObject = new object();
            int errors = 0;
            int maxIterations = 1000000;

            Parallel.For(fromInclusive: 0, toExclusive: maxIterations, body: (iteration) =>
                 {
                     try
                     {
                         TestDomain.Entities.VeryComplexKeyEntity entity = new TestDomain.Entities.VeryComplexKeyEntity();

                         entity.Id = new TestDomain.Keys.VeryComplexStructKey()
                         {
                             NumericId = iteration,
                             TextId = iteration.ToString(),
                             UniqueGuidId = Guid.NewGuid()
                         };

                         entity.Name = "Entity " + iteration;

                         lock (syncObject)
                         {
                             entityDict.Add(entity, iteration.ToString());
                         }
                     }
                     catch
                     {
                         Interlocked.Increment(ref errors);
                     }
                 });

            var limit = 1;
            var expected = true;
            var actual = (errors < limit);

            Assert.IsTrue(actual == expected, message: "The hashing algoritm generates more than " + limit + " collision per " + maxIterations + " entities");
            Assert.IsTrue(entityDict.Count == maxIterations - errors, message: "Something wrong happened running the test, the right number of elements where not introduced in the dictionary");
        }

    }
}
