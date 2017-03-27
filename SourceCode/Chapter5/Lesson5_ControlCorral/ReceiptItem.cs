using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2_ControlCorral
{
    public class ReceiptItem
    {
        public Guid ItemID { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public double Tax { get; set; }
    }

}
