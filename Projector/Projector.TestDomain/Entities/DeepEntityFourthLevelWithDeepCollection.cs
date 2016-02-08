using Projector.TestDomain.DDD;
using System.Collections.Generic;

namespace Projector.TestDomain.Entities
{
    public class DeepEntityFourthLevelWithDeepCollection : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}