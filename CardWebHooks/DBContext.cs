using MongoDB.Driver;
namespace CardWebSocks
{
    public class DBContext
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public DBContext()
        {
            Client = new MongoClient("mongodb://tower:27017/admin");
            Database = Client.GetDatabase("Cards");

        }


    }
}
