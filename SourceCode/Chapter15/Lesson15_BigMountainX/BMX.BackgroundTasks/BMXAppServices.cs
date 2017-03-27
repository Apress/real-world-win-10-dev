using BigMountainX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace BMX.BackgroundTasks
{
    sealed public class BMXAppServices : IBackgroundTask
    {
        AppState State { get; set; }
        BackgroundTaskDeferral _deferral;
        async public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            var local = ApplicationData.Current.LocalFolder;
            State = await AppState.FromStorageFileAsync(local, "state.xml");
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (details.Name == "bmx-service")
            {
                HandleBMXServiceRequests(details);
            }
        }

        private void HandleBMXServiceRequests(AppServiceTriggerDetails details)
        {
            var retval = new ValueSet();

            details.AppServiceConnection.RequestReceived += async (sender, args) =>
            {
                var message = args.Request.Message;
                var command = message["command"] as string;
                if (command == "customer-file-share")
                {
                    var token = message["token"] as string;
                    var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                    if (file != null)
                    {

                        var xml_text = await file.ReadTextAsync();
                        var xml = XElement.Parse(xml_text);

                        var customer = new BMXCustomerInfo
                        {
                            CustomerID = Guid.NewGuid(),
                            CustomerName = xml.Element("name").Value,
                            Email = xml.Element("email").Value,
                            DOB = DateTime.Parse(xml.Element("dob").Value)
                        };

                        State.Customers.Add(customer);
                        await State.SaveAsync();

                        retval.Add("result", "File was copied");
                    }
                    else
                    {
                        retval.Add("result", "Could not retrieve file");
                    }
                    await args.Request.SendResponseAsync(retval);
                }
                _deferral.Complete();
            };
        }
    }
}
