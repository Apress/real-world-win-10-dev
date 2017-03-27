using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2_ControlCorral
{
	public class ControlCorralModel
	{
		public List<POSTransaction> Transactions { get; set; } = new List<POSTransaction>();
		public List<CustomerInfo> Customers { get; set; } = new List<CustomerInfo>();
		public List<ReservationInfo> Reservations { get; set; } = new List<ReservationInfo>();
		public List<MassageType> MassageTypes { get; set; } = new List<MassageType>();
	}
}
