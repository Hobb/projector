using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class DeepProjectionFirstLevelWithDeepCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public DeepProjectionSecondLevelWithDeepCollection SecondLevel { get; set; }
    }
}