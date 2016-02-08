using System.Collections.Generic;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class SimpleProjectionDifferentTypesWithCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public ICollection<BasicEntityProjection> EntityList { get; set; }
    }
}