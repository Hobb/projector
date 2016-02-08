using Projector.TestDomain.DDD;
using Projector.TestDomain.DDD.Attributes;
using Projector.TestDomain.DDD.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.Entities
{
    public class BasicEntity : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}
