using Projector.TestDomain.DDD;
using System.Collections.Generic;

namespace Projector.TestDomain.Projections
{
    public class DeepProjectionThirdLevelWithDeepCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public ICollection<DeepProjectionFourthLevelWithDeepCollection> FourthLevel { get; set; }

    }
}