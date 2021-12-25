using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Library.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Library.Models
{
    [JsonConverter(typeof(ItemJsonConverter))]
    public class Item : INotifyPropertyChanged
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("priority")]
        public int Priority { get; set; }
        [BsonIgnore]
        public virtual string PrimaryText { get; }
        [BsonIgnore]
        public virtual string SecondaryText { get; }
        [BsonIgnore]
        public virtual bool IsCompleted { get; set; }
        public override string ToString()
        {
            return $"{Name} - {Priority} - {Description}";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [JsonConverter(typeof(ItemJsonConverter))]
    public class SupportTicket : Item
    {
        public SupportTicket()
        {
            Deadline = DateTime.Now;
        }
        [BsonElement("deadline")]
        public DateTimeOffset Deadline { get; set; }
        [BsonElement("isCompleted"), BsonRequired]
        private bool isCompleted;
        public override bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
            set
            {
                isCompleted = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("SecondaryText");
            }
        }
        public override string PrimaryText => $"Ticket: {Name} - {Description}";
        public override string SecondaryText => $"P:{Priority} {Deadline.ToString("MM'-'dd'-'yyyy")} - is {CompletedToString()}completed";
        private string CompletedToString() { return IsCompleted ? "" : "not "; }

        public override string ToString()
        {
            return $"{Name} - {Priority} - {Description} - {Deadline.ToString("MM'-'dd'-'yyyy")} - is completed: {IsCompleted}";
        }
    }
    [JsonConverter(typeof(ItemJsonConverter))]
    public class Appointment : Item
    {
        public Appointment()
        {
            Start = DateTime.Now;
            Stop = DateTime.Now;
        }
        [BsonElement("start")]
        public DateTimeOffset Start { get; set; }
        [BsonElement("stop")]
        public DateTimeOffset Stop { get; set; }
        [BsonElement("attendees")]
        public string Attendees { get; set; }
        public override string PrimaryText => $"Appointment: {Name} - {Description} P:{Priority} ";
        public override string SecondaryText => $"{Start.ToString("MM'-'dd'-'yyyy")} - {Stop.ToString("MM'-'dd'-'yyyy")} - Attendees: {Attendees}";
        public override string ToString()
        {
            return $"{Name} - {Priority} - {Description} - {Start.ToString("MM'-'dd'-'yyyy")} - {Stop.ToString("MM'-'dd'-'yyyy")} - {Attendees}";
        }
    }
}
