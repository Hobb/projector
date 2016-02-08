using Projector.TestDomain.Keys;
using Projector.TestDomain.DDD.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projector.TestDomain.DDD;

namespace Projector.TestDomain.Entities
{
    public class EntityWithFields : BaseEntity<VeryComplexStructKey>
    {
        private int integerProperty;

        public int IntegerProperty
        {
            get { return integerProperty; }
            set { integerProperty = value; }
        }

        public string StringField;

        private DateTime randomDate;

        public DateTime RandomDate
        {
            get { return randomDate; }
            set { randomDate = value; }
        }

        private DateRange range;

        public DateRange Range
        {
            get { return range; }
            set { range = value; }
        }

        private string iAmNotSupposedToEverMap;
        public string IAmNotSupposedToEverMap
        {
            get
            {
                return this.iAmNotSupposedToEverMap;
            }
            set
            {
                this.iAmNotSupposedToEverMap = value;
            }
        }
    }
}
