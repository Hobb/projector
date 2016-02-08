using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projector.TestDomain.Entities;
using Projector.TestDomain.Projections;
using Projector.TestDomain;
using Projector.TestDomain.Keys;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Projector.TestDomain.DDD.ValueObjects;

namespace Projector.Core.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ProjectionsTests
    {
        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedTwoFlatEntitiesWithMatchingNamesProjectWorks_ProjectedEntity_NoException()
        {
            EntityWithFields entity = new EntityWithFields();
            entity.Id = new VeryComplexStructKey() { NumericId = 1, TextId = "1", UniqueGuidId = Guid.NewGuid() };
            entity.IntegerProperty = 2;
            entity.RandomDate = DateTime.MinValue;
            entity.Range = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            entity.StringField = "Something";

            ProjectionWithFields projection = entity.Project().To<ProjectionWithFields>();

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entity.IntegerProperty == projection.IntegerProperty, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.RandomDate == projection.RandomDate, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.Range.Equals(projection.Range), message: "ValueObjects should be exactly the same");
            Assert.IsTrue(entity.StringField == projection.StringField, message: "Fields should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedTwoFlatEntitiesWithMatchingNamesProjectWorksOverQueryable_ProjectedEntity_NoException()
        {
            EntityWithFields entity = new EntityWithFields();
            entity.Id = new VeryComplexStructKey() { NumericId = 1, TextId = "1", UniqueGuidId = Guid.NewGuid() };
            entity.IntegerProperty = 2;
            entity.RandomDate = DateTime.MinValue;
            entity.Range = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            entity.StringField = "Something";

            List<EntityWithFields> entityList = new List<EntityWithFields>() { entity };

            List<ProjectionWithFields> projectionList = entityList.ProjectEnumerable().To<ProjectionWithFields>().ToList();

            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Same number of elements should be in both lists");
            Assert.IsTrue(entityList[0].Id.Equals(projectionList[0].Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entityList[0].IntegerProperty == projectionList[0].IntegerProperty, message: "Properties should be exactly the same");
            Assert.IsTrue(entityList[0].RandomDate == projectionList[0].RandomDate, message: "Properties should be exactly the same");
            Assert.IsTrue(entityList[0].Range.Equals(projectionList[0].Range), message: "ValueObjects should be exactly the same");
            Assert.IsTrue(entityList[0].StringField == projectionList[0].StringField, message: "Fields should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedTwoNonFlatEntitiesWithMatchingNamesProjectWorks_ProjectedEntity_NoException()
        {
            NotSoFlatEntity entity = new NotSoFlatEntity();
            entity.Id = 1;
            entity.Name = "I am the entity";
            entity.Nestedentity = new BasicEntity()
            {
                Id = 2,
                Name = "I am the nested entity"
            };
            entity.Nestedfield = new BasicEntity()
            {
                Id = 3,
                Name = "I am the nested field"
            };

            NotSoFlatProjection projection = entity.Project().To<NotSoFlatProjection>();

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.Nestedentity.Id == projection.Nestedentity.Id, message: "Nested entity should be exactly the same");
            Assert.IsTrue(entity.Nestedentity.Name == projection.Nestedentity.Name, message: "Nested entity should be exactly the same");
            Assert.IsTrue(entity.Nestedfield.Id == projection.Nestedfield.Id, message: "Nested field should be exactly the same");
            Assert.IsTrue(entity.Nestedfield.Name == projection.Nestedfield.Name, message: "Nested field should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedTwoNonFlatEntitiesWithMatchingNamesProjectWorksOverQueryable_ProjectedEntity_NoException()
        {
            NotSoFlatEntity entity = new NotSoFlatEntity();
            entity.Id = 1;
            entity.Name = "I am the entity";
            entity.Nestedentity = new BasicEntity()
            {
                Id = 2,
                Name = "I am the nested entity"
            };
            entity.Nestedfield = new BasicEntity()
            {
                Id = 3,
                Name = "I am the nested field"
            };

            List<NotSoFlatEntity> entityList = new List<NotSoFlatEntity>() { entity };

            List<NotSoFlatProjection> projectionList = entityList.ProjectEnumerable().To<NotSoFlatProjection>().ToList();

            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Same number of elements should be in both lists");
            Assert.IsTrue(entityList[0].Id.Equals(projectionList[0].Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entityList[0].Name == projectionList[0].Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entityList[0].Nestedentity.Id == projectionList[0].Nestedentity.Id, message: "Nested entity should be exactly the same");
            Assert.IsTrue(entityList[0].Nestedentity.Name == projectionList[0].Nestedentity.Name, message: "Nested entity should be exactly the same");
            Assert.IsTrue(entityList[0].Nestedfield.Id == projectionList[0].Nestedfield.Id, message: "Nested field should be exactly the same");
            Assert.IsTrue(entityList[0].Nestedfield.Name == projectionList[0].Nestedfield.Name, message: "Nested field should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedOneNonFlatEntityAndAConventionBasedFlattenedProjectionWithMatchingNamesProjectWorksOneWay_ProjectedEntity_NoException()
        {
            NotSoFlatEntity entity = new NotSoFlatEntity();
            entity.Id = 1;
            entity.Name = "I am the entity";
            entity.Nestedentity = new BasicEntity()
            {
                Id = 2,
                Name = "I am the nested entity"
            };
            entity.Nestedfield = new BasicEntity()
            {
                Id = 3,
                Name = "I am the nested field"
            };

            FlatConventionBasedProjection projection = entity.Project().To<FlatConventionBasedProjection>();

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.Nestedentity.Id == projection.NestedentityId, message: "Nested entity should have been mapped against flattened properties");
            Assert.IsTrue(entity.Nestedentity.Name == projection.NestedentityName, message: "Nested entity should have been mapped against flattened properties");
            Assert.IsTrue(entity.Nestedfield.Id == projection.NestedfieldId, message: "Nested field should be exactly the same");
            Assert.IsTrue(entity.Nestedfield.Name == projection.NestedfieldName, message: "Nested field should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedOneNonFlatEntityAndAConventionBasedFlattenedProjectionWithMatchingNamesProjectWorksOverQueryableOneWay_ProjectedEntity_NoException()
        {
            NotSoFlatEntity entity = new NotSoFlatEntity();
            entity.Id = 1;
            entity.Name = "I am the entity";

            entity.Nestedentity = new BasicEntity()
            {
                Id = 2,
                Name = "I am the nested entity"
            };
            entity.Nestedfield = new BasicEntity()
            {
                Id = 3,
                Name = "I am the nested field"
            };
            entity.IDoNotMapWithAnyThing = new BasicEntity()
            {
                Id = 4,
                Name = "I will never be mapped"
            };

            List<NotSoFlatEntity> entityList = new List<NotSoFlatEntity>() { entity };

            List<FlatConventionBasedProjection> projectionList = entityList.ProjectEnumerable().To<FlatConventionBasedProjection>().ToList();

            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Same number of elements should be in both lists");
            Assert.IsTrue(entityList[0].Id.Equals(projectionList[0].Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entityList[0].Name == projectionList[0].Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entityList[0].Nestedentity.Id == projectionList[0].NestedentityId, message: "Nested entity should have been mapped against flattened properties");
            Assert.IsTrue(entityList[0].Nestedentity.Name == projectionList[0].NestedentityName, message: "Nested entity should have been mapped against flattened properties");
            Assert.IsTrue(entityList[0].Nestedfield.Id == projectionList[0].NestedfieldId, message: "Nested field should be exactly the same");
            Assert.IsTrue(entityList[0].Nestedfield.Name == projectionList[0].NestedfieldName, message: "Nested field should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedOneNonFlatEntityAndATokenBasedFlattenedProjectionWithMatchingNamesProjectWorksOneWay_ProjectedEntity_NoException()
        {
            NotSoFlatEntityProperlyCamelCased entity = new NotSoFlatEntityProperlyCamelCased();
            entity.Id = 1;
            entity.Name = "I am the entity";
            entity.NestedEntity = new BasicEntity()
            {
                Id = 2,
                Name = "I am the nested entity"
            };
            entity.NestedField = new BasicEntity()
            {
                Id = 3,
                Name = "I am the nested field"
            };

            FlatTokenBasedProjection projection = entity.Project().To<FlatTokenBasedProjection>(flatteningToken: "_");

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.NestedEntity.Id == projection.NestedEntity_Id, message: "Nested entity should have been mapped against flattened properties");
            Assert.IsTrue(entity.NestedEntity.Name == projection.NestedEntity_Name, message: "Nested entity should have been mapped against flattened properties");
            Assert.IsTrue(entity.NestedField.Id == projection.NestedField_Id, message: "Nested field should be exactly the same");
            Assert.IsTrue(entity.NestedField.Name == projection.NestedField_Name, message: "Nested field should be exactly the same");
            Assert.IsTrue(projection.NobodyWillEverMapWithMe == null, message: "No value should ever be mapped in this property as it does not exist in the source");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedOneEntityWithThreeDepthLevelsWithProjectionMatchingNamesProjectWorksOneWay_ProjectedEntity_NoException()
        {
            DeepEntityFirstLevel firstEntity = new DeepEntityFirstLevel();
            firstEntity.Id = 1;
            firstEntity.Name = "I am the first level entity";
            firstEntity.SecondLevel = new DeepEntitySecondLevel()
            {
                Id = 2,
                Name = "I am the second level entity",
                ThirdLevel = new DeepEntityThirdLevel()
                {
                    Id = 3,
                    Name = "I am the third level entity",
                }
            };

            DeepProjectionFirstLevel projection = firstEntity.Project().To<DeepProjectionFirstLevel>();

            Assert.IsTrue(firstEntity.Id == projection.Id, message: "Id should be exactly the same");
            Assert.IsTrue(firstEntity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Id == projection.SecondLevel.Id, message: "Second level entity should have been mapped against second level projection properties");
            Assert.IsTrue(firstEntity.SecondLevel.Name == projection.SecondLevel.Name, message: "Second level entity should have been mapped against second level projection properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.Id == projection.SecondLevel.ThirdLevel.Id, message: "Third level entity should have been mapped against third level projection properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.Name == projection.SecondLevel.ThirdLevel.Name, message: "Third level entity should have been mapped against third level projection properties");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnEntityWithACollectionOfEntitiesAndAMatchingProjectionProjectWorksOneWay_ProjectedEntity_NoException()
        {
            SimpleEntityWithCollection entity = new SimpleEntityWithCollection();
            entity.Id = 1;
            entity.Name = "I have a collection";
            entity.EntityList = new List<BasicEntity>()
            {
                new BasicEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                }
            };

            SimpleProjectionWithCollection projection = entity.Project().To<SimpleProjectionWithCollection>();

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(projection.EntityList != null, message: "Collection should have been created");
            Assert.IsTrue(entity.EntityList.Count() == projection.EntityList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Id == projection.EntityList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Name == projection.EntityList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Id == projection.EntityList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Name == projection.EntityList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnEntityWithACollectionOfEntitiesAndAMatchingProjectionProjectWorksBothWays_ProjectedEntity_NoException()
        {
            SimpleEntityWithCollection entity = new SimpleEntityWithCollection();
            entity.Id = 1;
            entity.Name = "I have a collection";
            entity.EntityList = new List<BasicEntity>()
            {
                new BasicEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                }
            };

            SimpleProjectionWithCollection projection = entity.Project().To<SimpleProjectionWithCollection>();

            SimpleEntityWithCollection entityToProjectOver = projection.Project().To<SimpleEntityWithCollection>();

            Assert.IsTrue(entity.Id.Equals(entityToProjectOver.Id), message: "Id should be exactly the same");
            Assert.IsTrue(entity.Name == entityToProjectOver.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entityToProjectOver.EntityList != null, message: "Collection should have been created");
            Assert.IsTrue(entity.EntityList.Count() == entityToProjectOver.EntityList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Id == entityToProjectOver.EntityList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Name == entityToProjectOver.EntityList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Id == entityToProjectOver.EntityList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Name == entityToProjectOver.EntityList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnEntityWithACollectionOfEntitiesAndAMatchingProjectionWithDifferentTypesProjectWorksBothWays_ProjectedEntity_NoException()
        {
            SimpleEntityWithCollection entity = new SimpleEntityWithCollection();
            entity.Id = 1;
            entity.Name = "I have a collection";
            entity.EntityList = new List<BasicEntity>()
            {
                new BasicEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                }
            };

            SimpleProjectionDifferentTypesWithCollection projection = entity.Project().To<SimpleProjectionDifferentTypesWithCollection>();

            SimpleEntityWithCollection entityToProjectOver = projection.Project().To<SimpleEntityWithCollection>();

            Assert.IsTrue(entity.Id.Equals(entityToProjectOver.Id), message: "Id should be exactly the same");
            Assert.IsTrue(entity.Name == entityToProjectOver.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entityToProjectOver.EntityList != null, message: "Collection should have been created");
            Assert.IsTrue(entity.EntityList.Count() == entityToProjectOver.EntityList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Id == entityToProjectOver.EntityList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Name == entityToProjectOver.EntityList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Id == entityToProjectOver.EntityList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Name == entityToProjectOver.EntityList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedOneEntityWithThreeDepthLevelsWithProjectionMatchingNamesButNullSecondLevelProjectWorksOneWay_ProjectedEntity_NoException()
        {
            DeepEntityFirstLevel firstEntity = new DeepEntityFirstLevel();
            firstEntity.Id = 1;
            firstEntity.Name = "I am the first level entity";
            firstEntity.SecondLevel = null;

            DeepProjectionFirstLevel projection = firstEntity.Project().To<DeepProjectionFirstLevel>();

            Assert.IsTrue(firstEntity.Id == projection.Id, message: "Id should be exactly the same");
            Assert.IsTrue(firstEntity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel == null, message: "This value should be null as it was null in the entity");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnEntityWithACollectionOfEntitiesAndAMatchingProjectionButNullCollectionProjectWorksBothWays_ProjectedEntity_NoException()
        {
            SimpleEntityWithCollection entity = new SimpleEntityWithCollection();
            entity.Id = 1;
            entity.Name = "I have a collection";
            entity.EntityList = null;

            SimpleProjectionWithCollection projection = entity.Project().To<SimpleProjectionWithCollection>();

            SimpleEntityWithCollection entityToProjectOver = projection.Project().To<SimpleEntityWithCollection>();

            Assert.IsTrue(entity.Id.Equals(entityToProjectOver.Id), message: "Id should be exactly the same");
            Assert.IsTrue(entity.Name == entityToProjectOver.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entityToProjectOver.EntityList == null, message: "Collection should be null as it was in source");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingEntityItsProjectionUpdatesItProjectWorks_UpdatedEntity_NoException()
        {
            EntityWithFields entity = new EntityWithFields();
            entity.Id = new VeryComplexStructKey() { NumericId = 1, TextId = "1", UniqueGuidId = Guid.NewGuid() };
            entity.IntegerProperty = 2;
            entity.RandomDate = DateTime.MinValue;
            entity.Range = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            entity.StringField = "Something";
            entity.IAmNotSupposedToEverMap = "IShouldNeverChange";

            ProjectionWithFields projection = entity.Project().To<ProjectionWithFields>();

            projection.IntegerProperty = 3;
            projection.RandomDate = DateTime.MinValue.AddDays(value: 1);
            projection.Range = new DateRange(DateTime.MinValue.AddDays(value: 1), DateTime.MaxValue);
            projection.StringField = "SomethingSomething";

            projection.Project().To<EntityWithFields>(entity);

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id struct should be exactly the same");
            Assert.IsTrue(entity.IntegerProperty == projection.IntegerProperty, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.RandomDate == projection.RandomDate, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.Range.Equals(projection.Range), message: "ValueObjects should be exactly the same");
            Assert.IsTrue(entity.StringField == projection.StringField, message: "Fields should be exactly the same");
            Assert.IsTrue(entity.IAmNotSupposedToEverMap == "IShouldNeverChange", message: "This was an exisiting non-mapped field, should retain value");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedExistingEntityWithThreeDepthLevelsProjectionUpdatesProjectWorks_UpdatedEntity_NoException()
        {
            DeepEntityFirstLevel firstEntity = new DeepEntityFirstLevel();
            firstEntity.Id = 1;
            firstEntity.Name = "I am the first level entity";
            firstEntity.SecondLevel = new DeepEntitySecondLevel()
            {
                Id = 2,
                Name = "I am the second level entity",
                ThirdLevel = new DeepEntityThirdLevel()
                {
                    Id = 3,
                    Name = "I am the third level entity",
                }
            };

            DeepProjectionFirstLevel projection = firstEntity.Project().To<DeepProjectionFirstLevel>();

            projection.Name = "I got changed";
            projection.SecondLevel.Name = "I got changed too";
            projection.SecondLevel.ThirdLevel.Name = "I also got changed as the other two";

            projection.Project().To<DeepEntityFirstLevel>(firstEntity);

            Assert.IsTrue(firstEntity.Id == projection.Id, message: "Id should be exactly the same");
            Assert.IsTrue(firstEntity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Id == projection.SecondLevel.Id, message: "Second level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Name == projection.SecondLevel.Name, message: "Second level projection should have been mapped against second level entity properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.Id == projection.SecondLevel.ThirdLevel.Id, message: "Third level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.Name == projection.SecondLevel.ThirdLevel.Name, message: "Third level projection should have been mapped against third level entity properties");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingEntityWithACollectionOfEntitiesWeCanManuallyUpdateEntityGraph_UpdatedEntity_NoException()
        {
            SimpleEntityWithCollection entity = new SimpleEntityWithCollection();
            entity.Id = 1;
            entity.Name = "I have a collection";
            entity.EntityList = new List<BasicEntity>()
            {
                new BasicEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                },
                new BasicEntity()
                {
                    Id = 5,
                    Name = "I will disappear"
                }
            };

            SimpleProjectionDifferentTypesWithCollection projection = entity.Project().To<SimpleProjectionDifferentTypesWithCollection>();

            projection.Name = "I got changed";
            projection.EntityList.ElementAt(index: 0).Name = "I also got changed";
            projection.EntityList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projection.EntityList.Remove(projection.EntityList.ElementAt(index: 2));
            projection.EntityList.Add(new BasicEntityProjection()
            {
                Id = 4,
                Name = "I am completely new"
            });

            entity.Id = projection.Id;
            entity.Name = projection.Name;

            ICollection<BasicEntityProjection> source = projection.EntityList;
            ICollection<BasicEntity> destination = entity.EntityList;

            List<BasicEntity> toRemove = destination.Where(d => !source.Select(s => s.Id).Contains(d.Id))
                                      .ToList();
            toRemove.ForEach(r => destination.Remove(r));

            IEnumerable<BasicEntityProjection> toAdd = source.Where(s => !destination.Select(d => d.Id).Contains(s.Id));
            toAdd.ForEach(a => destination.Add(a.Project().To<BasicEntity>()));

            source.Except(toAdd)
                    .ForEach(u => u.Project()
                                    .To<BasicEntity>(destination.
                                                        FirstOrDefault(d => d.Id == u.Id)));

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.EntityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entity.EntityList.Count() == projection.EntityList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Id == projection.EntityList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Name == projection.EntityList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Id == projection.EntityList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Name == projection.EntityList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 2).Id == projection.EntityList.ElementAt(index: 2).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 2).Name == projection.EntityList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingEntityWithACollectionOfEntitiesProjectionUpdatesEntityGraph_UpdatedEntity_NoException()
        {
            SimpleEntityWithCollection entity = new SimpleEntityWithCollection();
            entity.Id = 1;
            entity.Name = "I have a collection";
            entity.EntityList = new List<BasicEntity>()
            {
                new BasicEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                },
                new BasicEntity()
                {
                    Id = 5,
                    Name = "I will disappear"
                }
            };

            SimpleProjectionDifferentTypesWithCollection projection = entity.Project().To<SimpleProjectionDifferentTypesWithCollection>();

            projection.Name = "I got changed";

            projection.EntityList.ElementAt(index: 0).Name = "I also got changed";
            projection.EntityList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projection.EntityList.Remove(projection.EntityList.ElementAt(index: 2));
            projection.EntityList.Add(new BasicEntityProjection()
            {
                Id = 4,
                Name = "I am completely new"
            });
            projection.Project().To<SimpleEntityWithCollection>(entity);

            Assert.IsTrue(entity.Id.Equals(projection.Id), message: "Id should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.EntityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entity.EntityList.Count() == projection.EntityList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Id == projection.EntityList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Name == projection.EntityList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Id == projection.EntityList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Name == projection.EntityList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 2).Id == projection.EntityList.ElementAt(index: 2).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 2).Name == projection.EntityList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingCollectionOfEntitiesProjectionCollectionUpdatesEntityGraph_UpdatedCollection_NoException()
        {
            List<BasicEntity> entityList = new List<BasicEntity>()
            {
                new BasicEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                },
                new BasicEntity()
                {
                    Id = 5,
                    Name = "I will disappear"
                }
            };

            List<BasicEntityProjection> projectionList = entityList.ProjectEnumerable().To<BasicEntityProjection>().ToList();

            projectionList.ElementAt(index: 0).Name = "I also got changed";
            projectionList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projectionList.Remove(projectionList.ElementAt(index: 2));
            projectionList.Add(new BasicEntityProjection()
            {
                Id = 4,
                Name = "I am completely new"
            });

            projectionList.ProjectEnumerable().To<BasicEntity>(entityList);

            Assert.IsTrue(entityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entityList.ElementAt(index: 0).Id == projectionList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 0).Name == projectionList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Id == projectionList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Name == projectionList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Id == projectionList.ElementAt(index: 2).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Name == projectionList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");

        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingCollectionOfComplexKeyEntitiesProjectionCollectionUpdatesEntityGraph_UpdatedCollection_NoException()
        {
            List<VeryComplexKeyEntity> entityList = new List<VeryComplexKeyEntity>()
            {
                new VeryComplexKeyEntity()
                {
                    Id = new VeryComplexStructKey()
                    {
                        NumericId = 1,
                        TextId = "1",
                        UniqueGuidId = Guid.NewGuid()
                    },
                    Name = "I am inside a collection"
                },
                new VeryComplexKeyEntity()
                {
                    Id = new VeryComplexStructKey()
                    {
                        NumericId = 2,
                        TextId = "2",
                        UniqueGuidId = Guid.NewGuid()
                    },
                    Name = "I am inside a collection too"
                },
                new VeryComplexKeyEntity()
                {
                    Id = new VeryComplexStructKey()
                    {
                        NumericId = 3,
                        TextId = "3",
                        UniqueGuidId = Guid.NewGuid()
                    },
                    Name = "I will disappear"
                }
            };

            List<VeryComplexKeyProjection> projectionList = entityList.ProjectEnumerable().To<VeryComplexKeyProjection>().ToList();

            projectionList.ElementAt(index: 0).Name = "I also got changed";
            projectionList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projectionList.Remove(projectionList.ElementAt(index: 2));
            projectionList.Add(new VeryComplexKeyProjection()
            {
                Id = new VeryComplexStructKey()
                {
                    NumericId = 4,
                    TextId = "4",
                    UniqueGuidId = Guid.NewGuid()
                },
                Name = "I am completely new"
            });

            projectionList.ProjectEnumerable().To<VeryComplexKeyEntity>(entityList);

            Assert.IsTrue(entityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entityList.ElementAt(index: 0).Id.Equals(projectionList.ElementAt(index: 0).Id), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 0).Name == projectionList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Id.Equals(projectionList.ElementAt(index: 1).Id), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Name == projectionList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Id.Equals(projectionList.ElementAt(index: 2).Id), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Name == projectionList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");

        }


        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingCollectionOfEntitiesWithNoInheritanceFromBaseEntityGraphsUpdateIfKeyMemberSupplied_UpdatedCollection_NoException()
        {
            List<BasicEntityNoInheritanceFromBaseEntity> entityList = new List<BasicEntityNoInheritanceFromBaseEntity>()
            {
                new BasicEntityNoInheritanceFromBaseEntity()
                {
                    Id = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntityNoInheritanceFromBaseEntity()
                {
                    Id = 3,
                    Name = "I am inside a collection too"
                },
                new BasicEntityNoInheritanceFromBaseEntity()
                {
                    Id = 5,
                    Name = "I will disappear"
                }
            };

            List<BasicEntityProjectionNoInheritanceFromBaseEntity> projectionList = entityList.ProjectEnumerable().To<BasicEntityProjectionNoInheritanceFromBaseEntity>().ToList();

            projectionList.ElementAt(index: 0).Name = "I also got changed";
            projectionList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projectionList.Remove(projectionList.ElementAt(index: 2));
            projectionList.Add(new BasicEntityProjectionNoInheritanceFromBaseEntity()
            {
                Id = 4,
                Name = "I am completely new"
            });

            projectionList.ProjectEnumerable().To<BasicEntityNoInheritanceFromBaseEntity>(entityList, keyMemberName: "Id");

            Assert.IsTrue(entityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entityList.ElementAt(index: 0).Id == projectionList.ElementAt(index: 0).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 0).Name == projectionList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Id == projectionList.ElementAt(index: 1).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Name == projectionList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Id == projectionList.ElementAt(index: 2).Id, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Name == projectionList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");

        }


        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedTwoFlatEntitiesWithKeyAttributesWithMatchingNamesProjectWorks_ProjectedEntity_NoException()
        {
            BasicEntityWithKeyAttributes entity = new BasicEntityWithKeyAttributes();
            entity.NumericId = 1;
            entity.Name = "Something";

            BasicEntityProjectionWithKeyAttribute projection = entity.Project().To<BasicEntityProjectionWithKeyAttribute>();

            Assert.IsTrue(entity.NumericId.Equals(projection.NumericId), message: "NumericId should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedTwoFlatEntitiesWithKeyAttributesWithMatchingNamesProjectWorksOverQueryable_ProjectedEntity_NoException()
        {
            BasicEntityWithKeyAttributes entity = new BasicEntityWithKeyAttributes();
            entity.NumericId = 1;
            entity.Name = "Something";

            List<BasicEntityWithKeyAttributes> entityList = new List<BasicEntityWithKeyAttributes>() { entity };

            List<BasicEntityProjectionWithKeyAttribute> projectionList = entityList.ProjectEnumerable().To<BasicEntityProjectionWithKeyAttribute>().ToList();

            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Same number of elements should be in both lists");
            Assert.IsTrue(entityList[0].NumericId.Equals(projectionList[0].NumericId), message: "NumericId should be exactly the same");
            Assert.IsTrue(entityList[0].Name == projectionList[0].Name, message: "Properties should be exactly the same");
        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingEntityWithKeyAttributesWithACollectionOfEntitiesProjectionWithKeyAttributesUpdatesEntityGraph_UpdatedEntity_NoException()
        {
            SimpleEntityWithKeyAttributesAndCollection entity = new SimpleEntityWithKeyAttributesAndCollection();
            entity.NumericId = 1;
            entity.Name = "I have a collection";
            entity.EntityList = new List<BasicEntityWithKeyAttributes>()
            {
                new BasicEntityWithKeyAttributes()
                {
                    NumericId = 2,
                    Name = "I am inside a collection"
                },
                new BasicEntityWithKeyAttributes()
                {
                    NumericId = 3,
                    Name = "I am inside a collection too"
                },
                new BasicEntityWithKeyAttributes()
                {
                    NumericId = 5,
                    Name = "I will disappear"
                }
            };

            SimpleProjectionWithKeyAttributesAndCollection projection = entity.Project().To<SimpleProjectionWithKeyAttributesAndCollection>();

            projection.Name = "I got changed";

            projection.EntityList.ElementAt(index: 0).Name = "I also got changed";
            projection.EntityList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projection.EntityList.Remove(projection.EntityList.ElementAt(index: 2));
            projection.EntityList.Add(new BasicEntityProjectionWithKeyAttribute()
            {
                NumericId = 4,
                Name = "I am completely new"
            });
            projection.Project().To<SimpleEntityWithKeyAttributesAndCollection>(entity);

            Assert.IsTrue(entity.NumericId.Equals(projection.NumericId), message: "NumericId should be exactly the same");
            Assert.IsTrue(entity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(entity.EntityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entity.EntityList.Count() == projection.EntityList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).NumericId == projection.EntityList.ElementAt(index: 0).NumericId, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 0).Name == projection.EntityList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).NumericId == projection.EntityList.ElementAt(index: 1).NumericId, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 1).Name == projection.EntityList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 2).NumericId == projection.EntityList.ElementAt(index: 2).NumericId, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entity.EntityList.ElementAt(index: 2).Name == projection.EntityList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");
        }


        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnExistingCollectionOfComplexKeyAttributesEntitiesProjectionCollectionUpdatesEntityGraph_UpdatedCollection_NoException()
        {
            #region Initialization

            List<BasicEntityWithComplexKeyAttributes> entityList = new List<BasicEntityWithComplexKeyAttributes>()
            {
                new BasicEntityWithComplexKeyAttributes()
                {
                    NumericId = 1,
                    StringId = "1",
                    Name = "I am inside a collection"
                },
                new BasicEntityWithComplexKeyAttributes()
                {
                    NumericId = 2,
                    StringId = "2",
                    Name = "I am inside a collection too"
                },
                new BasicEntityWithComplexKeyAttributes()
                {
                    NumericId = 2,
                    StringId = "3",
                    Name = "I will disappear"
                }
            };

            #endregion

            #region Entity to projection

            List<BasicEntityProjectionWithComplexKeyAttributes> projectionList = entityList.ProjectEnumerable().To<BasicEntityProjectionWithComplexKeyAttributes>().ToList();

            projectionList.ElementAt(index: 0).Name = "I also got changed";
            projectionList.ElementAt(index: 1).Name = "I got changed as the other two did";
            projectionList.Remove(projectionList.ElementAt(index: 2));
            projectionList.Add(new BasicEntityProjectionWithComplexKeyAttributes()
            {
                NumericId = 4,
                StringId = "4",
                Name = "I am completely new"
            });

            #endregion

            #region Sample code

            //var toRemove = entityList.Where(d => !projectionList.Select(s => new List<int>() { s.NumericId.GetHashCode(), s.StringId.GetHashCode() }
            //                                                                                    .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor)))
            //                                                        .Contains(new List<int>() { d.NumericId.GetHashCode(), d.StringId.GetHashCode() }
            //                                                                                    .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor))))
            //                                                        .ToList();

            //toRemove.ForEach(r => entityList.Remove(r));

            //var toAdd = projectionList.Where(s => !entityList.Select(d => new List<int>() { d.NumericId.GetHashCode(), d.StringId.GetHashCode() }
            //                                                                                    .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor)))
            //                                                        .Contains(new List<int>() { s.NumericId.GetHashCode(), s.StringId.GetHashCode() }
            //                                                                                    .Aggregate(17, (runningProduct, nextFactor) => unchecked(runningProduct * 31 + nextFactor))));

            //toAdd.ForEach(a => entityList.Add(a.Project().To<BasicEntityWithComplexKeyAttributes>()));

            //projectionList.Except(toAdd)
            //        .ForEach(u => u.Project()
            //                        .To<BasicEntityWithComplexKeyAttributes>(entityList.
            //                                            FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)));

            #endregion

            #region Sample Code 2

            //var toRemove = entityList.Where(d => !projectionList.Select(s => new { NumericId = s.NumericId, StringId = s.StringId })
            //                                                        .Contains(new { NumericId = d.NumericId, StringId = d.StringId }))
            //                                                        .ToList();

            //toRemove.ForEach(r => entityList.Remove(r));

            //var toAdd = projectionList.Where(s => !entityList.Select(d => new { NumericId = d.NumericId, StringId = d.StringId })
            //                                                        .Contains(new { NumericId = s.NumericId, StringId = s.StringId }));

            //toAdd.ForEach(a => entityList.Add(a.Project().To<BasicEntityWithComplexKeyAttributes>()));

            //projectionList.Except(toAdd)
            //        .ForEach(u => u.Project()
            //                        .To<BasicEntityWithComplexKeyAttributes>(entityList.
            //                                            FirstOrDefault(d => d.NumericId == u.NumericId && d.StringId == u.StringId)));

            #endregion

            projectionList.ProjectEnumerable().To<BasicEntityWithComplexKeyAttributes>(entityList);

            Assert.IsTrue(entityList != null, message: "Collection should be respected as it existed before");
            Assert.IsTrue(entityList.Count() == projectionList.Count(), message: "Collection should have same number of items");
            Assert.IsTrue(entityList.ElementAt(index: 0).NumericId.Equals(projectionList.ElementAt(index: 0).NumericId), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 0).StringId.Equals(projectionList.ElementAt(index: 0).StringId), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 0).Name == projectionList.ElementAt(index: 0).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).NumericId.Equals(projectionList.ElementAt(index: 1).NumericId), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).StringId.Equals(projectionList.ElementAt(index: 1).StringId), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 1).Name == projectionList.ElementAt(index: 1).Name, message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).NumericId.Equals(projectionList.ElementAt(index: 2).NumericId), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).StringId.Equals(projectionList.ElementAt(index: 2).StringId), message: "Items inside the collection should be exactly the same");
            Assert.IsTrue(entityList.ElementAt(index: 2).Name == projectionList.ElementAt(index: 2).Name, message: "Items inside the collection should be exactly the same");

        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnEntityWithNestedDeepLevelCollectionsTheMappingSucceds_ProjectedEntity_NoException()
        {
            DeepEntityFirstLevelWithDeepCollection firstEntity = new DeepEntityFirstLevelWithDeepCollection();
            firstEntity.Id = 1;
            firstEntity.Name = "I am the first level entity";
            firstEntity.SecondLevel = new DeepEntitySecondLevelWithDeepCollection()
            {
                Id = 2,
                Name = "I am the second level entity",
                ThirdLevel = new List<DeepEntityThirdLevelWithDeepCollection>()
                {
                    new DeepEntityThirdLevelWithDeepCollection()
                    {
                        Id = 3,
                        Name = "I am the third level entity",
                        FourthLevel = new List<DeepEntityFourthLevelWithDeepCollection>()
                        {
                            new DeepEntityFourthLevelWithDeepCollection()
                            {
                                Id = 4,
                                Name = "I am the fourth level entity",
                            }
                        }
                    }
                }
            };

            DeepProjectionFirstLevelWithDeepCollection projection = firstEntity.Project().To<DeepProjectionFirstLevelWithDeepCollection>();

            Assert.IsTrue(firstEntity.Id == projection.Id, message: "Id should be exactly the same");
            Assert.IsTrue(firstEntity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Id == projection.SecondLevel.Id, message: "Second level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Name == projection.SecondLevel.Name, message: "Second level projection should have been mapped against second level entity properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().Id == projection.SecondLevel.ThirdLevel.First().Id, message: "Third level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().Name == projection.SecondLevel.ThirdLevel.First().Name, message: "Third level projection should have been mapped against third level entity properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().FourthLevel.First().Id == projection.SecondLevel.ThirdLevel.First().FourthLevel.First().Id, message: "Fourth level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().FourthLevel.First().Name == projection.SecondLevel.ThirdLevel.First().FourthLevel.First().Name, message: "Fourth level projection should have been mapped against third level entity properties");

        }

        [TestMethod]
        [TestCategory("Projections")]
        public void ProvidedAnEntityWithNestedDeepLevelCollectionsProjectionUpdatesGraphs_UpdatedGraph_NoException()
        {
            DeepEntityFirstLevelWithDeepCollection firstEntity = new DeepEntityFirstLevelWithDeepCollection();
            firstEntity.Id = 1;
            firstEntity.Name = "I am the first level entity";
            firstEntity.SecondLevel = new DeepEntitySecondLevelWithDeepCollection()
            {
                Id = 2,
                Name = "I am the second level entity",
                ThirdLevel = new List<DeepEntityThirdLevelWithDeepCollection>()
                {
                    new DeepEntityThirdLevelWithDeepCollection()
                    {
                        Id = 3,
                        Name = "I am the third level entity",
                        FourthLevel = new List<DeepEntityFourthLevelWithDeepCollection>()
                        {
                            new DeepEntityFourthLevelWithDeepCollection()
                            {
                                Id = 4,
                                Name = "I am the fourth level entity",
                            }
                        }
                    }
                }
            };

            DeepProjectionFirstLevelWithDeepCollection projection = firstEntity.Project().To<DeepProjectionFirstLevelWithDeepCollection>();

            projection.Name = "I got changed";
            projection.SecondLevel.Name = "I got changed too";
            projection.SecondLevel.ThirdLevel.First().Name = "I also got changed as the other two";
            projection.SecondLevel.ThirdLevel.First().FourthLevel.First().Name = "I also got changed as the other three";

            projection.Project().To<DeepEntityFirstLevelWithDeepCollection>(firstEntity);

            Assert.IsTrue(firstEntity.Id == projection.Id, message: "Id should be exactly the same");
            Assert.IsTrue(firstEntity.Name == projection.Name, message: "Properties should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Id == projection.SecondLevel.Id, message: "Second level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.Name == projection.SecondLevel.Name, message: "Second level projection should have been mapped against second level entity properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().Id == projection.SecondLevel.ThirdLevel.First().Id, message: "Third level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().Name == projection.SecondLevel.ThirdLevel.First().Name, message: "Third level projection should have been mapped against third level entity properties");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().FourthLevel.First().Id == projection.SecondLevel.ThirdLevel.First().FourthLevel.First().Id, message: "Fourth level Id should be exactly the same");
            Assert.IsTrue(firstEntity.SecondLevel.ThirdLevel.First().FourthLevel.First().Name == projection.SecondLevel.ThirdLevel.First().FourthLevel.First().Name, message: "Fourth level projection should have been mapped against third level entity properties");

        }

    }
}