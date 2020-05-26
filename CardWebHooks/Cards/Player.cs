using System.Collections.Generic;
using System.Net;

namespace CardWebSocks.Cards
{

    public class Player
    {
        public List<string> Hand { get; private set; }
        public int Points { get; set; }
        public string ID { get; private set; }
        public List<string> PlayedCards { get; set; }
        public string ConnectionID { get; set; }
        public string Name { get; set; }
        public bool IsGameStarter { get; set; }
        public bool LastRoundWinner { get; set; }
        public Player(string ID, string connectionID)
        {
            ConnectionID = connectionID;
            this.ID = ID;
            Hand = new List<string>();
            PlayedCards = new List<string>();
        }

        public Player(string name, int points)
        {
            Name = name;
            Points = points;
        }

        public void GiveCard(string card)
        {
            Hand.Add(card);
        }

        public string PlayCard(int cardIndex)
        {
            var card = Hand[cardIndex];
            PlayedCards.Add(WebUtility.HtmlEncode(Hand[cardIndex]));
            Hand.RemoveAt(cardIndex);
            return card;
        }




    }
}
