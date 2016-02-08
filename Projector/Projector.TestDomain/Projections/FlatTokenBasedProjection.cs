using Projector.TestDomain.Entities;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class FlatTokenBasedProjection : BaseEntity<int>
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

        int nestedEntityId;

        public int NestedEntity_Id
        {
            get
            {
                return nestedEntityId;
            }
            set
            {
                nestedEntityId = value;
            }
        }

        string nestedEntityName;

        public string NestedEntity_Name
        {
            get
            {
                return nestedEntityName;
            }
            set
            {
                nestedEntityName = value;
            }
        }

        public int NestedField_Id;

        public string NestedField_Name;

        private BasicEntity nobodyWillEverMapWithMe;

        public BasicEntity NobodyWillEverMapWithMe
        {
            get { return nobodyWillEverMapWithMe; }
            set { nobodyWillEverMapWithMe = value; }
        }
    }
}