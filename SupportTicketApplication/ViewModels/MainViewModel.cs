using Library.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Library.Communication;
using System.Threading.Tasks;

namespace SupportTicketApplication.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        internal static string PersistencePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\";
        internal static JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        public Item SelectedItem { get; set; }
        public ObservableCollection<Item> items { get; set; }
        public ObservableCollection<Item> filteredItems
        {
            get
            {
                if (string.IsNullOrEmpty(Query))
                    return items;
                var q = Query.ToUpper();
                return new ObservableCollection<Item>(items.Where(t => t.Name.ToUpper().Contains(q)
                || t.Description.ToUpper().Contains(q) ||
                (t is Appointment) && (t as Appointment).Attendees != null &&
                (t as Appointment).Attendees.Contains(Query)));
            }
        }
        public string Query
        {
            get; set;
        }
        public void Complete_Selected()
        {
            if (SelectedItem == null) return;
            SelectedItem.IsCompleted = !SelectedItem.IsCompleted;
        }

        public MainViewModel()
        {
            items = new ObservableCollection<Item>();
        }
        public async Task RemoveAsync()
        {
            if (SelectedItem is SupportTicket)
                await new WebRequestHandler().Post($"http://localhost:33059/Ticket/Delete", SelectedItem);
            else
                await new WebRequestHandler().Post($"http://localhost:33059/Appointment/Delete", SelectedItem);
            items.Remove(SelectedItem);
        }
        public void SaveItems(string file)
        {
            string fileName = PersistencePath + file;
            File.WriteAllText(fileName, JsonConvert.SerializeObject(items, Settings));
        }
        public void LoadItems(string file)
        {
            string fileName = PersistencePath + file;
            if (File.Exists(fileName))
            {
                items.Clear();
                var loaded = JsonConvert.DeserializeObject<ObservableCollection<Item>>(File.ReadAllText(fileName), Settings);
                foreach (var item in loaded) items.Add(item);
            }
        }
        public void SortByPriority()
        {
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < items.Count - 1; ++i)
                {
                    if (items[i + 1].Priority < items[i].Priority)
                    {
                        items.Move(i + 1, i);
                        sorted = false;
                    }
                }
            }
        }
        public void Search()
        {
            NotifyPropertyChanged("filteredItems");
        }
        public void RefreshList()
        {
            NotifyPropertyChanged("FilteredItemList");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
