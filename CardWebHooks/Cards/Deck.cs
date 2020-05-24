using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CardWebSocks.Cards
{
    public partial class Deck
    {

        [JsonProperty("blackCards")]
        public List<BlackCard> BlackCards { get; set; }

        [JsonProperty("whiteCards")]
        public List<string> WhiteCards { get; set; }

        [JsonProperty("Base", NullValueHandling = NullValueHandling.Ignore)]
        public bool Base { get; set; }

        [JsonProperty("order")]
        public object Order { get; set; }

        [JsonProperty("ID")]
        public string Name { get; set; }
        [BsonId]
        public string DBID { get; set; }
        public string PlayID { get; set; }
        public Deck()
        {
            BlackCards = new List<BlackCard>();
            WhiteCards = new List<string>();
        }
        public Deck(string DBID)
        {
            this.DBID = DBID;
            BlackCards = new List<BlackCard>();
            WhiteCards = new List<string>();
        }

        //public Deck(string id, List<BlackCard> blackCards, List<string> whiteCards) //DEFUNCT CARDCAST CODE
        //{
        //    Id = id;
        //    BlackCards = blackCards;
        //    WhiteCards = whiteCards;
        //}

        public void OutputToJson()
        {
            File.WriteAllText("jsons/" + Name + ".txt", JsonConvert.SerializeObject(this));
        }
        public void AddDeck(Deck deck)
        {
            BlackCards.AddRange(deck.BlackCards);
            WhiteCards.AddRange(deck.WhiteCards);
        }

        public void RemoveDeck(Deck deck)
        {
            BlackCards.RemoveAll(x => deck.BlackCards.Find(y => x.Text == y.Text).Equals(x));
            WhiteCards.RemoveAll(x => deck.WhiteCards.Find(y => x == y).Equals(x));
        }

        public BlackCard ShowBlackCard()
        {
            Random rand = new Random();
            int cardNum = rand.Next(0, BlackCards.Count);
            BlackCard card = BlackCards[cardNum];
            BlackCards.RemoveAt(cardNum);
            return card;
        }

        public string SelectWhiteCard()
        {
            Random rand = new Random();
            var selected = rand.Next(WhiteCards.Count);
            string card = WhiteCards[selected];
            WhiteCards.RemoveAt(selected);
            return card;
        }

        public void AddWhiteCard(string text)
        {
            WhiteCards.Add(text);
        }

        public void AddBlackCard(BlackCard blackCard)
        {
            BlackCards.Add(blackCard);
        }

        public void RemoveBlackCard(string text)
        {
            BlackCards.Remove(BlackCards.Find(x => x.Text.Equals(text)));
        }

        public void RemoveWhiteCard(string text)
        {
            WhiteCards.Remove(WhiteCards.Find(x => x.Equals(text)));
        }
    }


    public partial class BlackCard
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("pick")]
        public int Pick { get; set; }

        public BlackCard(string text, int pick)
        {
            Text = text;
            Pick = pick;
        }
    }
}
