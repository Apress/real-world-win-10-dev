using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Contacts;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;


namespace BMX.BackgroundTasks
{
    sealed public class ContactsWatcher : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral; // Used to keep task alive
        async public void Run(IBackgroundTaskInstance taskInstance)
        {
            //cost
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;

            _deferral = taskInstance.GetDeferral();
            await ToastAsync("Forward us that contact for a free drink!");
            _deferral.Complete();
        }

        async Task ToastAsync(string toast_message)
        {
            try
            {
                var uri = new Uri("ms-appx:///data/adaptivetoast.xml");
                var adaptive_template = await StorageFile.GetFileFromApplicationUriAsync(uri);

                XmlDocument tile_xml_doc = await XmlDocument.LoadFromFileAsync(adaptive_template);
                var toast = new ToastNotification(tile_xml_doc);
                var toast_notifier = ToastNotificationManager.CreateToastNotifier();
                toast_notifier.Show(toast);
            }
            catch (Exception ex)
            {

            }
        }


    }
}
