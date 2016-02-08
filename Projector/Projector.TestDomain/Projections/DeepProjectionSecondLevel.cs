using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class DeepProjectionSecondLevel : BaseEntity<int>
    {
        public string Name { get; set; }

        public DeepProjectionThirdLevel ThirdLevel { get; set; }
    }
    
}