using Projector.TestDomain.DDD;
using System.Collections.Generic;

namespace Projector.TestDomain.Entities
{
    public class DeepEntityThirdLevelWithDeepCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public ICollection<DeepEntityFourthLevelWithDeepCollection> FourthLevel { get; set; }

    }
}