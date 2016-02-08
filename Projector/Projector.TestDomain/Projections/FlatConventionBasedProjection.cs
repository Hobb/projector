using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Projections
{
    public class FlatConventionBasedProjection : BaseEntity<int>
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

        int nestedentityId;

        public int NestedentityId
        {
            get
            {
                return nestedentityId;
            }
            set
            {
                nestedentityId = value;
            }
        }

        string nestedentityName;

        public string NestedentityName
        {
            get
            {
                return nestedentityName;
            }
            set
            {
                nestedentityName = value;
            }
        }

        public int NestedfieldId;

        public string NestedfieldName;
    }

}