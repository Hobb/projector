using Projector.TestDomain.Entities;
using System.Collections.Generic;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class SimpleProjectionWithCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public IEnumerable<BasicEntity> EntityList { get; set; }
    }
}