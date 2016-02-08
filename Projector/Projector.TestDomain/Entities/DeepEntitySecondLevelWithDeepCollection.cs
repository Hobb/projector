using Projector.TestDomain.DDD;
using System.Collections.Generic;

namespace Projector.TestDomain.Entities
{
    public class DeepEntitySecondLevelWithDeepCollection : BaseEntity<int>
    {
        public string Name { get; set; }

        public ICollection<DeepEntityThirdLevelWithDeepCollection> ThirdLevel { get; set; }
    }
}