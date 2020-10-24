using MongoDB.Bson;
using MongoDB.Driver;
using System;
using Zero_Web_GetGameContent.Model;

namespace Zero_Web_GetGameContent.Manager
{
    public class MongoDBManager
    {

        private static protected readonly string user = "zero";
        private static protected readonly string password = "Lands_at_the_sea";
        private static protected readonly string host = "darkwolfcraft.net";
        private static protected readonly string port = "27017";
        private static protected readonly string database = "Zero";
        private static protected readonly string connectionString = "mongodb://" + user + ":" + password + "@" + host + ":" + port + "/" + database;

        public static MongoClient mongoClient;
        private static IMongoDatabase db;

        public static void CreateConnection()
        {
            mongoClient = new MongoClient(connectionString);
            db = mongoClient.GetDatabase(database);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool CreateEntry(string collection, BsonDocument bsonDocument)
        {
            try
            {
                db.GetCollection<BsonDocument>(collection).InsertOne(bsonDocument);
                System.Diagnostics.Debug.WriteLine("MongoDB Entry created successfull!");
                return true;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("MongoDB Entry created Error!");
                return false;
            }
        }

        public static bool DocumentExists(string collection, StoreItem storeItem)
        {
            var filter = Builders<StoreItem>.Filter;
            var count = db.GetCollection<StoreItem>(collection).Find(filter.Eq(x => x.GameName, storeItem.GameName)).CountDocuments();
            return count > 0;
        }

    }
}
