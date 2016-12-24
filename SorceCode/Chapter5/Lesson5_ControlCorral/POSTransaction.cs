using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2_ControlCorral
{
    public class POSTransaction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        List<ReceiptItem> Items { get; } = new List<ReceiptItem>();
        public double DefaultTaxAmount { get; set; }
        public double SubTotal
        {
            get
            {
                var total = Items.Select(i => i.Price).Sum();
                return total;
            }
        }
        public double TotalTax
        {
            get
            {
                return Items.Select(i => i.Tax).Sum();
            }
        }

        public double Total
        {
            get
            {
                return SubTotal + TotalTax;
            }
        }

        public ReceiptItem AddItemByPrice(double price)
        {
            var tax_amount = price * DefaultTaxAmount;
            var item = new ReceiptItem
            {
                ItemID = Guid.NewGuid(),
                ItemName = "",
                Price = price,
                Tax = tax_amount,
            };
            Items.Add(item);
            PropertyChanged?
                .Invoke(this,
                new PropertyChangedEventArgs("SubTotal"));
            PropertyChanged?
                .Invoke(this,
                new PropertyChangedEventArgs("TotalTax"));
            PropertyChanged?
                .Invoke(this,
                new PropertyChangedEventArgs("Total"));
            return item;
        }
        public void RemoveItem(ReceiptItem item)
        {
            Items.Remove(item);
        }

        public void ClearItems()
        {
            Items.Clear();
        }
    }

}
