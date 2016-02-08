using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class DeepProjectionFirstLevel : BaseEntity<int>
    {
        public string Name { get; set; }

        public DeepProjectionSecondLevel SecondLevel { get; set; }
    }
}