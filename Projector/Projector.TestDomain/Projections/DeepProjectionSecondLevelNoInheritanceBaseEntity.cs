namespace Projector.TestDomain.Projections
{
    public class DeepProjectionSecondLevelNoInheritanceBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DeepProjectionThirdLevelNoInheritanceBaseEntity ThirdLevel { get; set; }
    }
}