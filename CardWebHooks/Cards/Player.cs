using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgaisntNet
{
    
    public class Player
    {
        public List<string> hand { get; private set; }
        public int points = 0;
        public string ID { get; private set; }
        public List<string> playedCards { get; set; }
        public string ConnectionID { get; set; }
        public string Name { get;  set; }
        public Player(Deck deck, string ID, string name,string connectionID)
        {
            ConnectionID = connectionID;
            Name = name;
            this.ID = ID;
            hand = new List<string>();
            playedCards = new List<string>();
            for (int i = 0; i < 7; i++)
            {
                Random rand = new Random();
                int cardNum = rand.Next(0, deck.WhiteCards.Count);
                GiveCard(deck.WhiteCards[cardNum]);
                deck.WhiteCards.RemoveAt(cardNum);
            }
        }

        public void GiveCard(string card)
        {
            hand.Add(card);
        }

        public void PlayCard(string card)
        {
            int cardIndex = hand.IndexOf(card);
            playedCards.Add(hand[cardIndex]);
            hand.RemoveAt(cardIndex);
        }



    }
}
