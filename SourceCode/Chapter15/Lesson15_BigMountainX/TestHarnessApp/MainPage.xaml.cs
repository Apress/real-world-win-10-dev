using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.AppService;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestHarnessApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int _name_count = 0;
        public MainPage()
        {
            this.InitializeComponent();
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }



        async private void TestUriLaunch1(object sender, RoutedEventArgs e)
        {
            LauncherOptions options = new LauncherOptions();
            ValueSet data = new ValueSet();
            data.Add("request", txt_page.Text);
            options.TargetApplicationPackageFamilyName = "748f8ea9-5de1-48be-a132-323bdc3c5fcc_hxmhn7vrhn3vp";
            var result = await Launcher.LaunchUriForResultsAsync(new Uri($"bmx:?page={txt_page.Text}"), options, data);
            var response = result.Result?["response"] as string;
            txt_message.Text = $"response is \"{ response}\"";
        }

        async private void OnQueryUri(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var result = await Launcher.QueryUriSupportAsync(new Uri($"{txt_uri.Text}:"), LaunchQuerySupportType.Uri);
            button.Content = result.ToString();
        }

        async private void OnFileActivate(object sender, RoutedEventArgs e)
        {
            LauncherOptions options = new LauncherOptions();
            options.TargetApplicationPackageFamilyName = "748f8ea9-5de1-48be-a132-323bdc3c5fcc_hxmhn7vrhn3vp";
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("test.txt", CreationCollisionOption.ReplaceExisting);
            await file.AppendTextAsync("?page=street");
            await Launcher.LaunchFileAsync(file, options);
        }

        async private void OnShareFile(object sender, RoutedEventArgs e)
        {
            //create or replace the file
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("customer.xml", CreationCollisionOption.ReplaceExisting);

            //write the message
            var xml = @"<customer>
                    <name>john</name>
                        <dob>10/19/1990</dob>
                        <email>john@test.com</email>
                    </customer>";
            await file.AppendTextAsync(xml);

            //share the file
            var token = SharedStorageAccessManager.AddFile(file);
            txt_token.Text = token;
        }


        async private void OnAppServiceShareFile(object sender, RoutedEventArgs e)
        {
            _name_count++;
            //create or replace the file
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("customer.xml", CreationCollisionOption.ReplaceExisting);

            //write the message
            var xml = $"<customer><name>john_{_name_count}</name>"
                + @"<dob>10/19/1990</dob>
                <email>john@test.com</email>
            </customer>";
            await file.AppendTextAsync(xml);

            //share the file
            var token = SharedStorageAccessManager.AddFile(file);

            AppServiceConnection connection = new AppServiceConnection();
            connection.AppServiceName = "bmx-service";
            connection.PackageFamilyName = "748f8ea9-5de1-48be-a132-323bdc3c5fcc_hxmhn7vrhn3vp";

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status == AppServiceConnectionStatus.Success)
            {
                //Send data to the service   
                var message = new ValueSet();
                message.Add("command", "customer-file-share");
                message.Add("token", token);

                //Send message and wait for response   
                AppServiceResponse response = await connection.SendMessageAsync(message);
                if (response.Status == AppServiceResponseStatus.Success)
                {
                    await new MessageDialog("Sent").ShowAsync();
                }
                else
                    await new MessageDialog("Failed when sending message to app").ShowAsync();
            }
            else
            {
                await new MessageDialog(status.ToString()).ShowAsync();
            }
            connection.Dispose();
        }

        async private void OnWriteToPublicherCache(object sender, RoutedEventArgs e)
        {
            _name_count++;
            var folder = ApplicationData.Current.GetPublisherCacheFolder("bxm_shared_state");
            var file = await folder.CreateFileAsync("customer.xml", CreationCollisionOption.ReplaceExisting);

            //write the message
            var xml = $"<customer><name>james_{_name_count}</name>"
                + @"<dob>10/19/1990</dob>
                    <email>john@test.com</email>
                    </customer>";
            await file.AppendTextAsync(xml);
        }

        private void OnShareCustomer(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _name_count++;
            var xml = $"<customer><name>james_{_name_count}</name>"
                + @"<dob>10/19/1990</dob>
    <email>john@test.com</email>
    </customer>";
            args.Request.Data.Properties.Title = "New Customer";
            args.Request.Data.Properties.Description = "Create a new Customer";
            args.Request.Data.SetText(xml);
        }
    }
}
