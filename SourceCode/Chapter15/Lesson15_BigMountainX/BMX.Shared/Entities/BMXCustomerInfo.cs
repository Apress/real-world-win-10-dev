using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigMountainX
{
    public class BMXCustomerInfo
    {
        public Guid CustomerID { get; set; }
        public string CustomerName { get; set; }
        public byte[] CustomerImage { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
    }
}
