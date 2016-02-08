using Projector.TestDomain.Entities;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class NotSoFlatProjection : BaseEntity<int>
    {
        string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        BasicEntity nestedentity;

        public BasicEntity Nestedentity
        {
            get
            {
                return nestedentity;
            }
            set
            {
                nestedentity = value;
            }
        }

        public BasicEntity Nestedfield;
    }

}