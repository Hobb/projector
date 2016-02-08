using Projector.TestDomain.DDD;
using System.Collections.Generic;

namespace Projector.TestDomain.Projections
{
    public class DeepProjectionSecondLevelWithDeepCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public ICollection<DeepProjectionThirdLevelWithDeepCollection> ThirdLevel { get; set; }
    }
}