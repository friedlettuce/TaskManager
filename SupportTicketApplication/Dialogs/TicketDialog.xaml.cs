using Library.Models;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Library.Communication;
using System.Diagnostics;
using Newtonsoft.Json;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SupportTicketApplication.Dialogs
{
    public sealed partial class TicketDialog : ContentDialog
    {
        private IList<Item> items;
        public TicketDialog(IList<Item> items)
        {
            InitializeComponent();
            DataContext = new SupportTicket();
            this.items = items;
        }

        public TicketDialog(Item selected, IList<Item> items)
        {
            InitializeComponent();
            DataContext = selected;
            this.items = items;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if ((DataContext as SupportTicket).Priority < 0) return;

            var context = (DataContext as SupportTicket);
            if (context.Priority < 0) return;

            var isNew = string.IsNullOrEmpty(context._id);
            var newAppResponse = await new WebRequestHandler().Post("http://localhost:33059/Ticket/AddOrUpdate", context);
            var newApp = JsonConvert.DeserializeObject<SupportTicket>(newAppResponse);

            if (isNew)
            {
                items.Add(newApp);
            }
            else
            {
                foreach (Item item in items)
                {
                    if (item._id.Equals(newApp._id))
                    {
                        items[items.IndexOf(item)] = newApp;
                        break;
                    }
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
