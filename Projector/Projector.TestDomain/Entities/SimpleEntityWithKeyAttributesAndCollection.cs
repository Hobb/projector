using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.Entities
{
    public class SimpleEntityWithKeyAttributesAndCollection
    {
        [Key]
        public int NumericId;

        public string Name
        { get; set; }

        public ICollection<BasicEntityWithKeyAttributes> EntityList
        { get; set; }
    }
}
