using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigMountainX
{
    public class BMXEvent
    {
        public Guid EventID { get; set; }

        public string EventTitle { get; set; }

        public string Description { get; set; }

        public DateTime StartDateTime { get; set; }

        public TimeSpan Duration { get; set; }

        public string Address { get; set; }

        public double? Latitude { get; set; } = 40.7484;

        public double? Longitude { get; set; } = -73.9857;
        public DateTime CreateDate { get; set; }

        public BMXFeaturedPerformer Feature { get; set; }

    }
}
