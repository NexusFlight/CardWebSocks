using System.Collections.Generic;

namespace CardsAgaisntNet
{

    public class Player
    {
        public List<string> Hand { get; private set; }
        public int points = 0;
        public string ID { get; private set; }
        public List<string> PlayedCards { get; set; }
        public string ConnectionID { get; set; }
        public string Name { get; set; }
        public bool IsGameStarter { get; set; }
        public Player(string ID, string connectionID)
        {
            ConnectionID = connectionID;
            this.ID = ID;
            Hand = new List<string>();
            PlayedCards = new List<string>();
        }

        public void GiveCard(string card)
        {
            Hand.Add(card);
        }

        public void PlayCard(string card)
        {
            int cardIndex = Hand.IndexOf(card);
            PlayedCards.Add(Hand[cardIndex]);
            Hand.RemoveAt(cardIndex);
        }




    }
}
