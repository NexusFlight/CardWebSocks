using CardsAgaisntNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWebSocks.Cards
{
    public class Game
    {
        public List<Player> players = new List<Player>();
        public List<Player> disconnectedPlayers = new List<Player>();
        public Decks Decks = new Decks();
        public BlackCard currentBlackCard;
        public Player cardCzar;
        public int playedCards = 0;
        public Game()
        {
            currentBlackCard = Decks.Deck.ShowBlackCard();
        }

        public void NewBlackCard()
        {
            for (int i = 0; i < currentBlackCard.Pick; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    players[j].playedCards.Clear();
                    if(players[j].hand.Count < 7)
                        players[j].GiveCard(Decks.Deck.SelectWhiteCard());
                }
            }
            currentBlackCard = Decks.Deck.ShowBlackCard();
            if (players.IndexOf(cardCzar) + 1 == players.Count)
            {
                cardCzar = players[0];
            }
            else
            {
                cardCzar = players[players.IndexOf(cardCzar) + 1];
            }
            playedCards = 0;
        }

        public Tuple<string,Array> PlayerPlayCard(string id, string card)
        {
            var player = players.Find(x => x.ID == id);
            var playedCount = player.playedCards.Count;
            if (playedCount != currentBlackCard.Pick && player != cardCzar)
            {
                player.PlayCard(card);
                playedCards++;
                return new Tuple<string, Array>(card,player.hand.ToArray());
            }
            return null;
        }

        

        public string[] PlayedCardsToArray()
        {
            var playedArray = new string[(players.Count-1)*currentBlackCard.Pick];
            
            int played = 0;
            List<int> selections = new List<int>();
            for (int i = 0; i < players.Count; i++)
            {
                selections.Add(i);
            }
            Random rand = new Random();
            selections = selections.OrderBy(x => rand.Next(0,100)).ToList();

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[selections[i]];
                if (player != cardCzar)
                {
                    for (int j = 0; j < player.playedCards.Count; j++)
                    {
                        playedArray[played] = player.playedCards[j];
                        played++;
                    }
                }
            }

            return playedArray;
        }

        internal void SelectCard(string card)
        {
            players.Find(x => x.playedCards.Contains(card)).points++;
        }

        public string[] AllPlayersDetails()
        {
            List<string> names = new List<string>();
            players.ForEach(x => names.Add(string.Concat(x.Name," points: ",x.points, (x == cardCzar ? " Card Czar" : "" ))));
            return names.ToArray();    
        }
    }
}
