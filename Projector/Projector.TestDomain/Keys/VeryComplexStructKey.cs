using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.Keys
{
    public struct VeryComplexStructKey
    {
        public int NumericId;
        public string TextId;
        public Guid UniqueGuidId;
    }
}
