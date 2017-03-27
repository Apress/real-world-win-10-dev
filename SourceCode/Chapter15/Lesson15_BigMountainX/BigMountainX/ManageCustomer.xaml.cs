using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Xml.Linq;
using Windows.Storage;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageCustomer : Page
    {
        CameraCaptureUI ccui = new CameraCaptureUI();
        BMXCustomerInfo _customer;
        bool _share_activated = false;
        ShareOperation _share;

        public ManageCustomer()
        {
            this.InitializeComponent();
        }

        async public Task Activated(ShareTargetActivatedEventArgs args)
        {
            _share = args.ShareOperation;
            var xml_text = await args.ShareOperation.Data.GetTextAsync();
            var xml = XElement.Parse(xml_text);

            _customer = new BMXCustomerInfo
            {
                CustomerID = Guid.NewGuid(),
            };

            //red xml into local values
            var customer_name = xml.Element("name").Value;
            var customer_email = xml.Element("email").Value;
            var customer_dob = xml.Element("dob").Value;
            var dob = DateTime.Parse(customer_dob);

            //set values of controls
            txt_name.Text = customer_name;
            txt_email.Text = customer_email;
            control_dob.Date = dob;
            grid_overlay.Visibility = Visibility.Collapsed;
            _share_activated = true;
            //set this as the window
            Window.Current.Content = this;
            Window.Current.Activate();
        }

        async private void ReplaceImage(object sender, RoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            control_image.Source = image;

            ccui.PhotoSettings.AllowCropping = true;
            ccui.PhotoSettings.MaxResolution =
                CameraCaptureUIMaxPhotoResolution.HighestAvailable;
            var result =
                await ccui.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                await image.SetSourceAsync(stream);

                //get the image data and store it
                stream.Seek(0);
                BinaryReader reader =
                    new BinaryReader(stream.AsStreamForRead());
                _customer.CustomerImage = new byte[stream.Size];
                reader.Read(_customer.CustomerImage,
                    0,
                    _customer.CustomerImage.Length);
            }
        }

        private void CancelCustomer(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        async private void SaveCustomer(object sender, RoutedEventArgs e)
        {
            _customer.CustomerName = txt_name.Text;
            _customer.Email = txt_email.Text;
            _customer.DOB = control_dob.Date.Date;
            App.State.Customers.Add(_customer);
            await App.State.SaveAsync();

            if (_share_activated)
                _share.ReportCompleted();
            else
                Frame.GoBack();
        }

        private void CreateNew(object sender, RoutedEventArgs e)
        {
            grid_overlay.Visibility = Visibility.Collapsed;
            _customer = new BMXCustomerInfo
            {
                CustomerID = Guid.NewGuid(),
            };
        }

        async private void LoadToken(object sender, RoutedEventArgs e)
        {
            var token = txt_token.Text;
            var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
            if (file != null)
            {
                //initialize customer
                _customer = new BMXCustomerInfo
                {
                    CustomerID = Guid.NewGuid(),
                };

                //load xml
                var xml_text = await file.ReadTextAsync();
                var xml = XElement.Parse(xml_text);

                //red xml into local values
                var customer_name = xml.Element("name").Value;
                var customer_email = xml.Element("email").Value;
                var customer_dob = xml.Element("dob").Value;
                var dob = DateTime.Parse(customer_dob);

                //set values of controls
                txt_name.Text = customer_name;
                txt_email.Text = customer_email;
                control_dob.Date = dob;

                //hide overlay
                grid_overlay.Visibility = Visibility.Collapsed;
            }
        }

        async private void OnLoadCachedCustomer(object sender, RoutedEventArgs e)
        {
            var folder = ApplicationData.Current.GetPublisherCacheFolder("bxm_shared_state");
            var file = await folder.GetFileAsync("customer.xml");
            //initialize customer
            _customer = new BMXCustomerInfo
            {
                CustomerID = Guid.NewGuid(),
            };

            //load xml
            var xml_text = await file.ReadTextAsync();
            var xml = XElement.Parse(xml_text);

            //red xml into local values
            var customer_name = xml.Element("name").Value;
            var customer_email = xml.Element("email").Value;
            var customer_dob = xml.Element("dob").Value;
            var dob = DateTime.Parse(customer_dob);

            //set values of controls
            txt_name.Text = customer_name;
            txt_email.Text = customer_email;
            control_dob.Date = dob;

            //hide overlay
            grid_overlay.Visibility = Visibility.Collapsed;
        }
    }
}