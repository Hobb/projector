using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Entities
{
    public class GuidKeyEntity : BaseEntity<Guid>
    {
        public string Name { get; set; }
    }
}
