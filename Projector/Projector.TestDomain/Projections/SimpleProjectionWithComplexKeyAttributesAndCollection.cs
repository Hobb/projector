using Projector.TestDomain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.Projections.Projections
{
    public class SimpleProjectionWithComplexKeyAttributesAndCollection
    {
        [Key]
        public int NumericId;
        [Key]
        public string StringId;

        public string Name
        { get; set; }

        public ICollection<BasicEntityWithKeyAttributes> EntityList
        { get; set; }
    }
}
