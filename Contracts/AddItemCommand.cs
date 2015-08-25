using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class AddItemCommand
    {
        public Guid ClientId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
