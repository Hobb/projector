using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.Entities
{
    public class BasicEntityWithKeyAttributes
    {
        [Key]
        public int NumericId;

        public string Name { get; set; }
    }
}
