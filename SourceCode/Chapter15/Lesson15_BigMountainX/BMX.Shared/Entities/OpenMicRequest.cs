using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigMountainX
{
    public class OpenMicRequest
    {
        public bool HasPaid { get; set; } = true;
        public string AdditionalInfo { get; set; }
        public BMXCustomerInfo Customer { get; set; }
    }
}
