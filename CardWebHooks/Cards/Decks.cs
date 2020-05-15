using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace CardWebSocks.Cards
{
    public class Decks
    {
        public CardsAgaisntNet.Deck Deck { get; private set; }
        public Decks()
        {
            Deck = new CardsAgaisntNet.Deck();
            DirectoryInfo dInfo = new DirectoryInfo("Cards/jsons/");
            foreach (var item in dInfo.GetFiles())
            {
                var json = File.ReadAllText(item.FullName);
                Deck.AddDeck(JsonConvert.DeserializeObject<CardsAgaisntNet.Deck>(json));
            }
        }
    }
}
