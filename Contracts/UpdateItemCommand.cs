using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class UpdateItemCommand
    {
        public Guid ClientId { get; set; }
        public Guid Id { get; set; }
        public string NewName { get; set; }
        public bool IsActive { get; set; }

    }
}
