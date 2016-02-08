namespace Projector.TestDomain.Entities
{
    public class DeepEntityFirstLevelNoInheritanceBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DeepEntitySecondLevelNoInheritanceBaseEntity SecondLevel { get; set; }
    }
}