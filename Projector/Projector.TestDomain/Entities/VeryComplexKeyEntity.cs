using Projector.TestDomain.DDD.Attributes;
using Projector.TestDomain.DDD.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Entities
{
    public class VeryComplexKeyEntity : BaseEntity<Keys.VeryComplexStructKey>
    {
        public string Name { get; set; }
    }
}
