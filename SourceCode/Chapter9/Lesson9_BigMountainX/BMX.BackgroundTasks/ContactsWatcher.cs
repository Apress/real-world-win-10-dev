using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Contacts;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;


namespace BMX.BackgroundTasks
{
    sealed public class ContactsWatcher : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral; // Used to keep task alive
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //cost
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;

            _deferral = taskInstance.GetDeferral();
            Toast("Forward us that contact for a free drink!");
            _deferral.Complete();
        }

        void Toast(string toast_message)
        {
            try
            {
                var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
                var texts = xml.DocumentElement.GetElementsByTagName("text");
                var text_node = texts.FirstOrDefault() as XmlElement;
                text_node.AppendChild(xml.CreateTextNode(toast_message));

                ToastNotification toast = new ToastNotification(xml);
                var notifier = ToastNotificationManager.CreateToastNotifier();
                notifier.Show(toast);
            }
            catch (Exception ex)
            {

            }
        }


    }
}
