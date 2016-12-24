using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2_ControlCorral
{
	public class ReservationInfo
	{
		public DateTime AppointmentDay { get; set; }
		public TimeSpan AppointmentTime { get; set; }

		public DateTime DOB { get; set; }

		public bool HasPaid { get; set; } = true;

		public string Passphrase { get; set; }
		public string Procedure { get; set; }

		public string CustomerName { get; set; }

		public byte[] CustomerImage { get; set; }

		public double MassageIntensity { get; set; }
		public string ExpirationDate { get; set; }
		public string CCVCode { get; set; }
	}
}
