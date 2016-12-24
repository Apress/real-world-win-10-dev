using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BigMountainX
{
    public enum GenderCode
    {
        Male = 0,
        Female = 1,
    }
    public class BMXProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string ImageLocation { get; set; }
        public DateTime? DOB { get; set; }
        public GenderCode? Gender { get; set; }

    }
}
