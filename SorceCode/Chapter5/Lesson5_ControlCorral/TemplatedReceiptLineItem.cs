using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Lesson2_ControlCorral
{
[
	TemplatePart(Name ="PART_Price",Type =typeof(TextBlock)),
	TemplatePart(Name = "PART_Tax", Type = typeof(TextBlock))
]
public sealed class TemplatedReceiptLineItem : Control
{
	TextBlock PART_Tax, PART_Price;
	public ReceiptItem Item { get; private set; }

	public TemplatedReceiptLineItem()
	{
		this.DefaultStyleKey = typeof(TemplatedReceiptLineItem);
	}

	protected override void OnApplyTemplate()
	{
		PART_Price = this.GetTemplateChild("PART_Price") as TextBlock;
		PART_Tax = this.GetTemplateChild("PART_Tax") as TextBlock;

			//set the values
			if (PART_Price != null && PART_Tax != null)
			{
				PART_Price.Text = $"{Item.Price:C}";
				PART_Tax.Text = $"tax - {Item.Tax:C}";
			}
	}

	public void AddItem(ReceiptItem item)
	{	
		Item = item;
	}
}
}
