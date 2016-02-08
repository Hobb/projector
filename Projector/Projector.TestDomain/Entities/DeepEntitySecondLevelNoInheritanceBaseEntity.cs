namespace Projector.TestDomain.Entities
{
    public class DeepEntitySecondLevelNoInheritanceBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DeepEntityThirdLevelNoInheritanceBaseEntity ThirdLevel { get; set; }
    }
}