using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Core.Contracts
{
    public interface IKeyedItem<K>
    {
        K Id { get; set; }
    }
}
