using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class BasicEntityProjection : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}