using Library.Communication;
using Library.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SupportTicketApplication.Dialogs
{
    public sealed partial class AppointmentDialog : ContentDialog
    {
        private IList<Item> items;
        public AppointmentDialog(IList<Item> items)
        {
            InitializeComponent();
            DataContext = new Appointment();
            this.items = items;
        }
        public AppointmentDialog(Item selected, IList<Item> items)
        {
            InitializeComponent();
            DataContext = selected;
            this.items = items;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if ((DataContext as Appointment).Priority < 0) return;

            var context = (DataContext as Appointment);
            if (context.Priority < 0) return;

            var isNew = string.IsNullOrEmpty(context._id);
            var newAppResponse = await new WebRequestHandler().Post("http://localhost:33059/Appointment/AddOrUpdate", context);
            var newApp = JsonConvert.DeserializeObject<Appointment>(newAppResponse);

            if (isNew)
            {
                items.Add(newApp);
            }
            else
            {
                foreach(Item item in items)
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
