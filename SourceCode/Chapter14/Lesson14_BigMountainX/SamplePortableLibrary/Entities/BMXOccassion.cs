using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BigMountainX
{
   // [DataContract]
    public class BMXOccasion
    {
        public Guid OccasionID { get; private set; }
        public int AttendanceCount { get; private set; }

        public List<POSTransaction> Transactions { get; set; }

        public BMXOccasion(BigMountainX.BMXEvent bmx_event)
        {
            OccasionID = bmx_event.EventID;
        }

        public void Enter()
        {
            AttendanceCount++;
        }

        public void Leave()
        {
            if (AttendanceCount > 0)
                AttendanceCount--;
        }
    }
}
