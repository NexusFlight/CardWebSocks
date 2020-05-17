using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CardsAgaisntNet;
using Newtonsoft.Json;
namespace CardWebSocks.Cards
{
    public class Decks
    {
        public Deck Deck { get; private set; }
        public List<Deck> AvailableDecks { get; }
        public Decks()
        {
            Deck = new CardsAgaisntNet.Deck();
            DirectoryInfo dInfo = new DirectoryInfo("Cards/jsons/");
            AvailableDecks = new List<Deck>();
            foreach (var item in dInfo.GetFiles())
            {
                var json = File.ReadAllText(item.FullName);
                AvailableDecks.Add(JsonConvert.DeserializeObject<Deck>(json));
            }
        }

        public void AddDeckFromName(string name)
        {
            Deck.AddDeck(AvailableDecks.Find(x => x.Id == name));
        }
    }
}
