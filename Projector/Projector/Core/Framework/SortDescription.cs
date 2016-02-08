using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Core.Framework
{
    public class SortDescription
    {
        public SortDescription(string propertyName, ListSortDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }
        public string PropertyName { get; set; }
        public ListSortDirection Direction { get; set; }
    }

    public enum ListSortDirection
    {
        Ascending = 0,
        Descending = 1,
    }
}
