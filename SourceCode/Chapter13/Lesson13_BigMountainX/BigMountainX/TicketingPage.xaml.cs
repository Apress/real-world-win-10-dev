using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicketingPage : Page
    {
        public TicketingPage()
        {
            this.InitializeComponent();
            this.Loaded += TicketingPage_Loaded;
        }

        private void TicketingPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonLabel("bmx_openmic", btn_openmic);
            SetButtonLabel("bmx_ticket", btn_ticket);
            SetButtonLabel("bmx_table", btn_vipticket);
        }

        public void SetButtonLabel(string key, Button button)
        {
            var product_name = App.ListingInfo.ProductListings[key].Name;
            var product_price = App.ListingInfo.ProductListings[key].FormattedPrice;
            button.Content = $"{product_name} - {product_price}";
        }

        async void BuyApp(IUICommand c)
        {
            await CurrentAppSimulator.RequestAppPurchaseAsync(true);
        }

        async Task PurchaseFeatureAsync(string feature_name)
        {
            var feature = App.License.ProductLicenses[feature_name];
            if (App.ListingInfo.ProductListings.ContainsKey(feature_name))
            {
                var feature_listinginfo = App.ListingInfo.ProductListings[feature_name];
                if (feature.IsActive)
                {
                    MessageDialog dlg = new MessageDialog($"You already have {feature_listinginfo.Name}");
                    await dlg.ShowAsync();
                }
                else
                {
                    MessageDialog dlg = new MessageDialog($"{feature_listinginfo.FormattedPrice} costs {feature_listinginfo.Name}", $"Buy {feature_listinginfo.Name}?");
                    dlg.Commands.Add(new UICommand($"Buy {feature_listinginfo.Name}", async delegate (IUICommand c)
                    {
                        await CurrentAppSimulator.RequestProductPurchaseAsync(feature.ProductId);
                //CurrentAppSimulator.
            }));
                    dlg.Commands.Add(new UICommand("No thanks", cmd =>
                    {

                    }));
                    await dlg.ShowAsync();
                }
            }
            else
            {
                MessageDialog dlg = new MessageDialog($"{feature_name} does not exist in this application");
                await dlg.ShowAsync();
            }
        }

        async private void OnStandardTicket(object sender, RoutedEventArgs e)
        {
            if (App.License.IsTrial)
            {

                MessageDialog dlg = new MessageDialog(string.Format("{1}: I cost {0}", App.ListingInfo.FormattedPrice, App.ListingInfo.Description), "app is trial, why not buy it?");
                dlg.Commands.Add(new UICommand("Buy It", BuyApp));
                dlg.Commands.Add(new UICommand("No thanks", cmd =>
                {

                }));
                dlg.DefaultCommandIndex = 1;
                await dlg.ShowAsync();
            }
            else
            {
                await PurchaseFeatureAsync("bmx_ticket");
            }
        }

        async private void OnVIPTicket(object sender, RoutedEventArgs e)
        {
            if (App.License.IsTrial)
            {

                MessageDialog dlg = new MessageDialog(string.Format("{1}: I cost {0}", App.ListingInfo.FormattedPrice, App.ListingInfo.Description), "app is trial, why not buy it?");
                dlg.Commands.Add(new UICommand("Buy It", BuyApp));
                dlg.Commands.Add(new UICommand("No thanks", cmd =>
                {

                }));
                dlg.DefaultCommandIndex = 1;
                await dlg.ShowAsync();
            }
            else
            {
                await PurchaseFeatureAsync("bmx_table");
            }
        }

        async private void OnOpenMicTicket(object sender, RoutedEventArgs e)
        {
            if (App.License.IsTrial)
            {

                MessageDialog dlg = new MessageDialog(string.Format("{1}: I cost {0}", App.ListingInfo.FormattedPrice, App.ListingInfo.Description), "app is trial, why not buy it?");
                dlg.Commands.Add(new UICommand("Buy It", BuyApp));
                dlg.Commands.Add(new UICommand("No thanks", cmd =>
                {

                }));
                dlg.DefaultCommandIndex = 1;
                await dlg.ShowAsync();
            }
            else
            {
                await PurchaseFeatureAsync("bmx_openmic");
            }
        }
    }
}
