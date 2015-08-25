using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{


    public class ClientNotification
    {
        public Guid ClientId { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }

}
