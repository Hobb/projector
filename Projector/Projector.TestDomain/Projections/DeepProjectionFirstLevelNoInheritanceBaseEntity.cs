using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class DeepProjectionFirstLevelNoInheritanceBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DeepProjectionSecondLevelNoInheritanceBaseEntity SecondLevel { get; set; }
    }
}