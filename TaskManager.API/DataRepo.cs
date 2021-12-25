using Library.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Persistence
{
    public class DataRepo
    {
        private IMongoDatabase _database;
        private static DataRepo instance;
        public static DataRepo Current
        {
            get
            {
                if (instance == null)
                {
                    var settings = MongoClientSettings.FromConnectionString("mongodb+srv://gjanderso:m9Atxs3tPVAi5vE@cluster0.sij92.mongodb.net/Cluster0?retryWrites=true&w=majority");
                    var client = new MongoClient(settings);
                    var database = client.GetDatabase("test");
                     instance = new DataRepo(database);
                }
                return instance;
            }
        }
        public void AddOrUpdate(Item item)
        {
            if (string.IsNullOrEmpty(item._id))
            {
                item._id = ObjectId.GenerateNewId().ToString();
            }

            //mapping for collections
            IMongoCollection<BsonDocument> collection;
            Item itemToPersist;
            if (item is SupportTicket)
            {
                collection = _database.GetCollection<BsonDocument>("tickets");
                itemToPersist = item as SupportTicket;
            }
            else if (item is Appointment)
            {
                collection = _database.GetCollection<BsonDocument>("appointments");
                itemToPersist = item as Appointment;
            }
            else
            {
                throw new TypeNotSupportedException(item.GetType().ToString());
            }


            collection.DeleteOne(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(itemToPersist._id)));
            collection.InsertOne(itemToPersist.ToBsonDocument());
            return;
        }
        public void Delete(Item item)
        {
            if (string.IsNullOrEmpty(item._id))
            {
                return;
            }

            //mapping for collections
            IMongoCollection<BsonDocument> collection;
            Item itemToPersist;
            if (item is SupportTicket)
            {
                collection = _database.GetCollection<BsonDocument>("tickets");
                itemToPersist = item as SupportTicket;
            }
            else if (item is Appointment)
            {
                collection = _database.GetCollection<BsonDocument>("appointments");
                itemToPersist = item as Appointment;
            }
            else
            {
                throw new TypeNotSupportedException(item.GetType().ToString());
            }

            collection.DeleteOne(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(itemToPersist._id)));
            return;
        }
        public List<SupportTicket> Tickets
        {
            get
            {
                var ticketsBson = _database.GetCollection<BsonDocument>("tickets");
                var data = ticketsBson.Find(_ => true).ToList();
                var _tickets = new List<SupportTicket>();
                foreach(var item in data)
                {
                    var json = item.ToJson();
                    var obj = BsonSerializer.Deserialize<SupportTicket>(item);
                    _tickets.Add(obj);
                }
                return _tickets;
            }
        }
        public List<Appointment> Appointments
        {
            get
            {
                var appBson = _database.GetCollection<BsonDocument>("appointments");
                var data = appBson.Find(_ => true).ToList();
                var _appointments = new List<Appointment>();
                foreach (var item in data)
                {
                    var json = item.ToJson();
                    var obj = BsonSerializer.Deserialize<Appointment>(item);
                    _appointments.Add(obj);
                }
                return _appointments;
            }
        }
        private DataRepo(IMongoDatabase db)
        {
            _database = db;
        }
        public class TypeNotSupportedException : Exception
        {
            private string _type;
            public TypeNotSupportedException(string type)
            {
                _type = type;
            }
            public override string Message => $"Attempt was made to persist an unsupported type: {_type}";
        }
    }
}