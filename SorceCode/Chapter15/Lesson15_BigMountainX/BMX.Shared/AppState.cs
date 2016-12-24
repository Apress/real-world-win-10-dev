using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using General.UWP.Library;

namespace BigMountainX
{
    public class AppState : StateAwareObject<AppState>
    {
        public BMXProfile UserProfile { get; set; }

        public List<OpenMicRequest> OpenMicRequests { get; set; }

        public List<BMXCustomerInfo> Customers { get; set; } = new List<BMXCustomerInfo>();

        public BMXEvent NextEvent { get; set; }

        public List<string> MailingList { get; set; } = new List<string>();

        public List<BMXEvent> Events { get; set; } = new List<BMXEvent>();

    }
}