using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Entities
{
    public class NotSoFlatEntity : BaseEntity<int>
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        BasicEntity nestedentity;

        public BasicEntity Nestedentity
        {
            get { return nestedentity; }
            set { nestedentity = value; }
        }

        public BasicEntity Nestedfield;


        private BasicEntity iDoNotMapWithAnyThing;

        public BasicEntity IDoNotMapWithAnyThing
        {
            get { return iDoNotMapWithAnyThing; }
            set { iDoNotMapWithAnyThing = value; }
        }
    }

}
