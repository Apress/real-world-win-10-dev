using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using General.UWP.Library;

namespace BigMountainX
{
	public class AppState : StateAwareObject<AppState>
	{
		public BMXProfile UserProfile { get; set; }

	}
}