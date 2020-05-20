using CardWebSocks.Cards;
using MongoDB.Driver;
using System.Threading.Tasks;

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

        public void CreateCollection(string collection)
        {
            Database.CreateCollection(collection);
        }

        public async Task CreateDeck(Deck deck)
        {
            await Database.GetCollection<Deck>("Decks").InsertOneAsync(deck);
        }

        public async void UpdateDeck(Deck deck)
        {
            var filter = Builders<Deck>.Filter.Eq("DBID", deck.DBID);
            await Database.GetCollection<Deck>("Decks").ReplaceOneAsync(filter, deck);
        }
        
        public Deck GetDeckByName(string name)
        {
            return Database.GetCollection<Deck>("Decks").Find(x => x.Name == name).First();
        }

        public async Task<Deck> GetDeckByID(string iD)
        {
            return await Database.GetCollection<Deck>("Decks").FindAsync(x => x.DBID == iD).Result.FirstAsync();
        }

    }
}
