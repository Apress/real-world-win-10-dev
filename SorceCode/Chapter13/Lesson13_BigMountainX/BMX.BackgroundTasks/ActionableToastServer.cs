using BigMountainX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BMX.BackgroundTasks
{
    sealed public class ActionableToastServer : IBackgroundTask
    {
        async public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            if (details != null)
            {
                var roaming_folder = ApplicationData.Current.RoamingFolder;
                var state = await AppState.FromStorageFileAsync(roaming_folder, "state.xml");

                var arg = details.Argument;
                var input_count = details.UserInput.Count;
                object value;
                details.UserInput.TryGetValue("txt_email", out value);
                if (value != null)
                {
                    state.MailingList.Add(value as string);
                    await state.SaveAsync();
                }

            }
            deferral.Complete();
        }
    }
}
