using CardWebSocks.Cards;
using MongoDB.Driver;
using System;
using System.Text;
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

        public string GetPlayID()
        {
            var count = Database.GetCollection<Deck>("Decks").CountDocumentsAsync(FilterDefinition<Deck>.Empty).Result.ToString();
            StringBuilder sB = new StringBuilder();
            for (int i = count.Length-1; i >= 0; i--)
            {
                var number = Convert.ToInt32(count[i].ToString());
                var charCode = 65 + number;
                sB.Insert(0, (char)charCode);
            }
            while(sB.Length < 5)
            {
                sB.Insert(0, "A");
            }
            return sB.ToString();
        }
        public void CreateDeck(Deck deck)
        {
            deck.PlayID = GetPlayID();
            Database.GetCollection<Deck>("Decks").InsertOneAsync(deck);
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

        public async Task<Deck> GetDeckByPlayID(string playiD)
        {
            return await Database.GetCollection<Deck>("Decks").FindAsync(x => x.PlayID == playiD).Result.FirstAsync();
        }
    }
}
