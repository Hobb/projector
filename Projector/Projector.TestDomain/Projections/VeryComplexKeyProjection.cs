using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class VeryComplexKeyProjection : BaseEntity<Keys.VeryComplexStructKey>
    {
        public string Name { get; set; }
    }
}