using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class DeleteItemCommand
    {
        public Guid ClientId;
        public Guid Id;
    }
}
