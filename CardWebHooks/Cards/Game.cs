using CardsAgaisntNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardWebSocks.Cards
{
    public class Game
    {
        private readonly IList<Player> disconnectedPlayers;
        private readonly IList<Player> players;
        
        public string Name { get; }
        public Guid Id { get; }
        public IEnumerable<Player> Players => players;
        public Decks Decks { get; } = new Decks();
        public BlackCard CurrentBlackCard { get; private set; }
        public Player CardCzar { get; private set; }
        public int PlayedCards { get; private set; }
        public int PlayerCount => players.Count;

        public Game(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
            players = new List<Player>();
            disconnectedPlayers = new List<Player>();
            PlayedCards = 0;

            CurrentBlackCard = Decks.Deck.ShowBlackCard();
        }

        public void NewBlackCard()
        {
            for (var i = 0; i < CurrentBlackCard.Pick; i++)
            {
                for (var j = 0; j < PlayerCount; j++)
                {
                    players[j].playedCards.Clear();
                    if(players[j].hand.Count < 7)
                        players[j].GiveCard(Decks.Deck.SelectWhiteCard());
                }
            }
            CurrentBlackCard = Decks.Deck.ShowBlackCard();
            CardCzar = players.IndexOf(CardCzar) + 1 == PlayerCount 
                ? players[0] 
                : players[players.IndexOf(CardCzar) + 1];
            
            PlayedCards = 0;
        }

        public Tuple<string,Array> PlayerPlayCard(string id, string card)
        {
            var player = FindPlayerById(id);
            var playedCount = player.playedCards.Count;
            if (playedCount != CurrentBlackCard.Pick && player != CardCzar)
            {
                player.PlayCard(card);
                PlayedCards++;
                return new Tuple<string, Array>(card,player.hand.ToArray());
            }
            return null;
        }

        public string[] PlayedCardsToArray()
        {
            var playedArray = new string[(PlayerCount-1)*CurrentBlackCard.Pick];
            
            var played = 0;
            var selections = new List<int>();
            for (var i = 0; i < PlayerCount; i++)
            {
                selections.Add(i);
            }
            var rand = new Random();
            selections = selections.OrderBy(x => rand.Next(0,100)).ToList();

            for (var i = 0; i < PlayerCount; i++)
            {
                var player = players[selections[i]];
                if (IsCardCzar(player)) continue;
                
                foreach (var playedCard in player.playedCards)
                {
                    playedArray[played] = playedCard;
                    played++;
                }
            }

            return playedArray;
        }

        public void ConnectPlayer(Player player)
        {
            players.Add(player);
            
            if (IsPlayerDisconnected(player))
                disconnectedPlayers.Remove(player);
        }

        public bool IsPlayerDisconnected(Player player)
        {
            return disconnectedPlayers.Contains(player);
        }

        public void DisconnectPlayer(Player player)
        {
            players.Remove(player);
            disconnectedPlayers.Add(player);
            PlayedCards -= player.playedCards.Count;
            player.playedCards.Clear();
        }

        public Player FindPlayerById(string id)
        {
            return players.SingleOrDefault(x => x.ID == id) ?? disconnectedPlayers.SingleOrDefault(x => x.ID == id);
        }
        
        public Player FindPlayerByConnectionId(string connectionId)
        {
            return players.SingleOrDefault(x => x.ConnectionID == connectionId);
        }

        public bool IsCardCzar(Player player)
        {
            return CardCzar.ID == player.ID;
        }

        public void AssignAsCardCzar(Player player)
        {
            CardCzar = player;
        }

        internal void SelectCard(string card)
        {
            Players.Single(x => x.playedCards.Contains(card)).points++;
        }

        public string[] AllPlayersDetails()
        {
            return Players
                .Select(player => $"${player.Name} points: {player.points} {(IsCardCzar(player) ? "Card Czar" : "")}")
                .ToArray();
        }
    }
}
