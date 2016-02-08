using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.Projections
{
    public class BasicEntityProjectionWithComplexKeyAttributes
    {
        [Key]
        public int NumericId;
        [Key]
        public string StringId;

        public string Name { get; set; }
    }
}
