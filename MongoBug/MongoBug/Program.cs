using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoBug
{
    class Program
    {
        const string databaseName = "MyMongoDB";
        const string collectionName = "MyMongocollection";

        private 
        static void Main(string[] args)
        {
            Guid ID1 = Guid.NewGuid();

            Guid Value1 = Guid.NewGuid();

            MyObject obj1 = new MyObject { ID = ID1, myValue = Value1 };

            Insert<MyObject>(obj1);

            Guid?[] myLookupArray = new Guid?[] { Value1};

            List<MyObject> retrievedObjects = Find<MyObject>(x => myLookupArray.Contains(x.myValue)).Result;
        }

        private async static Task<List<T>> Find<T>(Expression<Func<T, bool>> filter)
        {
            IMongoCollection<T> collection = GetCollection<T>();

            IFindFluent<T, T> query = collection.Find(filter);

            return await query.ToListAsync();
        }

        class MyObject
        {
            [BsonId]
            public Guid ID { get; set; }

            public Guid? myValue { get; set; }
        }

        private static IMongoCollection<T> GetCollection<T>()
        {
            MongoDB.Driver.MongoClient client = new MongoClient("mongodb://xxxx:xxxx@localhost:27017");
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<T> collection = database.GetCollection<T>(collectionName);
            return collection;
        }
        
        static void Insert<T>(MyObject obj)
        {
            GetCollection<MyObject>().InsertOneAsync(obj).Wait();
        }
    }
}
