using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Entities
{
    public class DeepEntitySecondLevel : BaseEntity<int>
    {
        public string Name { get; set; }

        public DeepEntityThirdLevel ThirdLevel { get; set; }
    }
    
}