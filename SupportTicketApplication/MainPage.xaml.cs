using SupportTicketApplication.Dialogs;
using SupportTicketApplication.ViewModels;
using Library.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Library.Communication;
using Newtonsoft.Json;

namespace SupportTicketApplication
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var mainViewModel = new MainViewModel();
            var ticketString = new WebRequestHandler().Get("http://localhost:33059/Ticket").Result;
            var tickets = new List<SupportTicket>();
            var appointments = new List<Appointment>();
            if(ticketString != null)
                tickets = JsonConvert.DeserializeObject<List<SupportTicket>>(ticketString);
            tickets.ForEach(t => mainViewModel.items.Add(t));
            var appointmentsString = new WebRequestHandler().Get("http://localhost:33059/Appointment").Result;
            if(appointmentsString != null)
                appointments = JsonConvert.DeserializeObject<List<Appointment>>(appointmentsString);
            appointments.ForEach(a => mainViewModel.items.Add(a));

            DataContext = mainViewModel;
            (DataContext as MainViewModel).RefreshList();
        }

        private async void AddTicket_Click(object sender, RoutedEventArgs e)
        {
            var diag = new TicketDialog((DataContext as MainViewModel).items);
            await diag.ShowAsync();
        }
        private async void AddApp_Click(object sender, RoutedEventArgs e)
        {
            var diag = new AppointmentDialog((DataContext as MainViewModel).items);
            await diag.ShowAsync();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).RemoveAsync();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var diag = new FileDialog();
            await diag.ShowAsync();
            (DataContext as MainViewModel).SaveItems(diag.fileName);
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            var diag = new FileDialog();
            await diag.ShowAsync();
            (DataContext as MainViewModel).LoadItems(diag.fileName);
        }
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).SortByPriority();
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var dc = (DataContext as MainViewModel);
            if(dc.SelectedItem is SupportTicket)
            {
                var diag = new TicketDialog(dc.SelectedItem, dc.items);
                await diag.ShowAsync();
            }
            else
            {
                var diag = new AppointmentDialog(dc.SelectedItem, dc.items);
                await diag.ShowAsync();
            }
            dc.RefreshList();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).Search();
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).Complete_Selected();
        }
    }
}
