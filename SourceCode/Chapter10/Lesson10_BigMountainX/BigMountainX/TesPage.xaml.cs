using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BigMountainX
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        MediaCapture _capture;
        public TestPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            _capture = new MediaCapture();
        }

        async private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _capture.InitializeAsync();
        }

        private void OnImageCaptureInvoked(object sender, RoutedEventArgs e)
        {


            //_capture.PrepareLowLagPhotoCaptureAsync(
        }

        async private void OnDownloadInvoked(object sender, RoutedEventArgs e)
        {
            var uri = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4";
            var result_file = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync("bigbunny.wmv");


            var downloader = new BackgroundDownloader();
            await downloader.CreateDownload(new Uri(uri), result_file).StartAsync();
        }

        int _count = 0;
        private void OnTextToast(object sender, RoutedEventArgs e)
        {
            XmlDocument toast_xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var str = toast_xml.DocumentElement.GetXml();
            if (str != null)
            {
            }
            var text_nodes = toast_xml.DocumentElement.GetElementsByTagName("text");
            var text_node = text_nodes.FirstOrDefault() as XmlElement;
            text_node.AppendChild(toast_xml.CreateTextNode($"toast number {_count++}"));

            ToastNotification toast = new ToastNotification(toast_xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }

        private void OnTextImageToast(object sender, RoutedEventArgs e)
        {
            XmlDocument toast_xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText03);

            var text_nodes = toast_xml.DocumentElement.GetElementsByTagName("text");
            text_nodes[0].AppendChild(toast_xml.CreateTextNode("demo message"));
            text_nodes[1].AppendChild(toast_xml.CreateTextNode($"toast number {_count++}"));

            //image MUST be smaller than 800x800 and less than 256kb
            //app data can be reached with ms-appdata:/// protocol (this is isolated storage for the most part)
            var image_node = toast_xml.DocumentElement.GetElementsByTagName("image")[0] as XmlElement;
            image_node.SetAttribute("src", "ms-appx:///assets/tile-square.png");

            ToastNotification toast = new ToastNotification(toast_xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }

        private void OnSoundToast(object sender, RoutedEventArgs e)
        {
            XmlDocument toast_xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var text_node = toast_xml.DocumentElement.GetElementsByTagName("text").FirstOrDefault() as XmlElement;
            text_node.AppendChild(toast_xml.CreateTextNode("custom sound notification"));


            //add a custom audio sound to the notification (in this case it is the new mail sound)
            /**
                * it can also be:
                * ms-winsoundevent:Notification.Default
                * ms-winsoundevent:Notification.SMS
                * ms-winsoundevent:Notification.IM
                * ms-winsoundevent:Notification.Reminder
                * Silent
                * 
                * */
            var audio_node = toast_xml.CreateElement("audio");
            audio_node.SetAttribute("src", "ms-winsoundevent:Notification.IM");
            audio_node.SetAttribute("loop", "true");
            IXmlNode toastNode = toast_xml.SelectSingleNode("/toast");
            toastNode.AppendChild(audio_node);

            var v = toast_xml.GetXml();
            if (v != null)
            {
            }
            ToastNotification toast = new ToastNotification(toast_xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }


        private void OnScheduleToast(object sender, RoutedEventArgs e)
        {
            XmlDocument toast_xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var text_node = toast_xml.DocumentElement.GetElementsByTagName("text").FirstOrDefault() as XmlElement;
            text_node.AppendChild(toast_xml.CreateTextNode("scheduled toast!"));

            ToastNotification toast = new ToastNotification(toast_xml);

            ScheduledToastNotification stoast = new ScheduledToastNotification(toast_xml,
                DateTime.Now.AddSeconds(10),
                TimeSpan.FromMinutes(1), 5);

            stoast.Id = "hello";
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.AddToSchedule(stoast);
        }

        private void OnTextImageToastReact(object sender, RoutedEventArgs e)
        {
            XmlDocument toast_xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText03);

            var text_nodes = toast_xml.DocumentElement.GetElementsByTagName("text");
            text_nodes[0].AppendChild(toast_xml.CreateTextNode("demo message"));
            text_nodes[1].AppendChild(toast_xml.CreateTextNode($"toast number {_count++}"));

            //image MUST be smaller than 800x800 and less than 256kb
            //app data can be reached with ms-appdata:/// protocol (this is isolated storage for the most part)
            var image_node = toast_xml.DocumentElement.GetElementsByTagName("image")[0] as XmlElement;
            image_node.SetAttribute("src", "ms-appx:///assets/tile-square.png");

            var audio_node = toast_xml.CreateElement("audio");
            audio_node.SetAttribute("src", "ms-winsoundevent:Notification.IM");
            audio_node.SetAttribute("loop", "true");
            IXmlNode toastNode = toast_xml.SelectSingleNode("/toast");
            toastNode.AppendChild(audio_node);


            ToastNotification toast = new ToastNotification(toast_xml);
            toast.Failed += async (a, b) =>
            {
                if (b.ErrorCode != null)
                {
                    await new MessageDialog("Failed!").ShowAsync();
                }
            };
            toast.Activated += async delegate (ToastNotification n, object s)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await new MessageDialog("Activated!").ShowAsync();
                });

            };
            toast.Dismissed += async delegate (ToastNotification a, ToastDismissedEventArgs b)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await new MessageDialog("Dismissed!").ShowAsync();
                });
            };
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }

        async private void OnAdaptiveToast(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("ms-appx:///data/adaptivetoast.xml");
            var adaptive_template = await StorageFile.GetFileFromApplicationUriAsync(uri);

            XmlDocument tile_xml_doc = await XmlDocument.LoadFromFileAsync(adaptive_template);
            var toast = new ToastNotification(tile_xml_doc);
            var toast_notifier = ToastNotificationManager.CreateToastNotifier();
            toast_notifier.Show(toast);
        }
    }
}
