using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Lesson2_ControlCorral
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class POS : Page
	{
		POSTransaction Transaction { get; set; } = new POSTransaction();

		public POS()
		{
			this.InitializeComponent();
			this.Loaded += POS_Loaded;

			Transaction.DefaultTaxAmount = .15;
		}

		private void POS_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void NumberClicked(object sender, RoutedEventArgs e)
		{
			if (txt_currentamount.Text.Trim() == "0")
				txt_currentamount.Text = "";

			//test to see if two decimal places have already been specified
			var parts = txt_currentamount.Text.Split('.');
			if (parts.Length > 1)
			{
				if (parts[1].Length >= 2)
					return;
			}
			Button target = sender as Button;
			var number = target.Tag as string;
			txt_currentamount.Text += number;

		}

		private void DotClicked(object sender, RoutedEventArgs e)
		{
			if (!txt_currentamount.Text.Contains("."))
				txt_currentamount.Text += ".";

		}

		private void AddItemClicked(object sender, RoutedEventArgs e)
		{
			var reciept_item = Transaction
				.AddItemByPrice(double.Parse(txt_currentamount.Text));

			//add a reciept line item as a textblock
			TemplatedReceiptLineItem line_item = new TemplatedReceiptLineItem();
			line_item.AddItem(reciept_item);

			reciept.Children.Insert(0, line_item);

			txt_currentamount.Text = "0";
		}

		private void DeleteClicked(object sender, RoutedEventArgs e)
		{
			if (txt_currentamount.Text.Length > 0)
				txt_currentamount.Text = txt_currentamount.Text.Remove(txt_currentamount.Text.Length - 1);
			if (txt_currentamount.Text.Length == 0)
				txt_currentamount.Text = "0";
		}

		async private void PayClicked(object sender, RoutedEventArgs e)
		{
			App.Model.Transactions.Add(Transaction);
			await App.SaveModelAsync();
			Frame.GoBack();
		}
	}
}