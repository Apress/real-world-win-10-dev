using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media.Playback;

namespace BMX.BackgroundTasks
{
	sealed public class AudioPlayback : IBackgroundTask
	{
		private BackgroundTaskDeferral _deferral; // Used to keep task alive
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			_deferral = taskInstance.GetDeferral();
			taskInstance.Task.Completed += TaskCompleted;
		}

		void TaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
		{
			_deferral.Complete();
		}
	}
}
