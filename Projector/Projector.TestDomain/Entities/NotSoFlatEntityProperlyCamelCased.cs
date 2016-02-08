using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Entities
{
    public class NotSoFlatEntityProperlyCamelCased : BaseEntity<int>
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

        BasicEntity nestedEntity;

        public BasicEntity NestedEntity
        {
            get
            {
                return nestedEntity;
            }
            set
            {
                nestedEntity = value;
            }
        }

        public BasicEntity NestedField;
    }
}