using General.UWP.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.UserProfile;

namespace BMX.BackgroundTasks
{
    sealed public class WallpaperSwitcherTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral; // Used to keep task alive
        async public void Run(IBackgroundTaskInstance taskInstance)
        {

            //cost
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;

            //handle cancelation if needed
            var cancel = new CancellationTokenSource();
            taskInstance.Canceled += (s, e) =>
                {
                    cancel.Cancel();
                    cancel.Dispose();
                };

            //get deferral
            _deferral = taskInstance.GetDeferral();


            try
            {
                //progress
                var result = await UserSettings.ChangeWallpaperAsync();
                taskInstance.Progress = result;
            }
            catch (Exception ex)
            {
                _deferral.Complete();
            }
        }
    }
}
