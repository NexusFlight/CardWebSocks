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
        public bool HasGameStarted { get; set; } = false;

        public Game(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
            players = new List<Player>();
            disconnectedPlayers = new List<Player>();
            PlayedCards = 0;
        }

        public void StartGame(string[] decks)
        {
            foreach (var deck in decks)
            {
                Decks.AddDeckFromName(deck);
            }
            NewBlackCard();
            HasGameStarted = true;
        }

        public void FillPlayerHand(Player player)
        {
            while (player.Hand.Count < 7)
                player.GiveCard(Decks.Deck.SelectWhiteCard());
        }

        public void NewBlackCard()
        {
            for (var j = 0; j < PlayerCount; j++)
            {
                players[j].PlayedCards.Clear();
                while (players[j].Hand.Count < 7)
                    players[j].GiveCard(Decks.Deck.SelectWhiteCard());
            }
            CurrentBlackCard = Decks.Deck.ShowBlackCard();

            PlayedCards = 0;
        }

        internal Player GetNextPlayer()
        {
            return players[0];
        }

        public void NewCardCzar()
        {

            CardCzar = players.IndexOf(CardCzar) + 1 == PlayerCount
            ? players[0]
    : players[players.IndexOf(CardCzar) + 1];

        }

        public Tuple<string, Array> PlayerPlayCard(string id, string card)
        {
            var player = FindPlayerById(id);
            var playedCount = player.PlayedCards.Count;
            if (playedCount != CurrentBlackCard.Pick && player != CardCzar)
            {
                player.PlayCard(card);
                PlayedCards++;
                return new Tuple<string, Array>(card, player.Hand.ToArray());
            }
            return null;
        }

        public string[] PlayedCardsToArray()
        {
            var playedArray = new string[(PlayerCount - 1) * CurrentBlackCard.Pick];

            var played = 0;
            var selections = new List<int>();
            for (var i = 0; i < PlayerCount; i++)
            {
                selections.Add(i);
            }
            var rand = new Random();
            selections = selections.OrderBy(x => rand.Next(0, 100)).ToList();

            for (var i = 0; i < PlayerCount; i++)
            {
                var player = players[selections[i]];
                if (IsCardCzar(player)) continue;

                foreach (var playedCard in player.PlayedCards)
                {
                    playedArray[played] = playedCard;
                    played++;
                }
            }

            return playedArray;
        }

        public bool ConnectPlayer(Player player)
        {
            if (!players.Contains(FindPlayerByConnectionId(player.ConnectionID)))
            {
                players.Add(player);
            }
            else
            {
                return false;
            }
            if (IsPlayerDisconnected(player))
                disconnectedPlayers.Remove(player);
            if (HasGameStarted)
                FillPlayerHand(player);
            return true;
        }

        public bool IsPlayerDisconnected(Player player)
        {
            return disconnectedPlayers.Contains(player);
        }

        public void DisconnectPlayer(Player player)
        {
            players.Remove(player);
            if (HasGameStarted)
            {
                disconnectedPlayers.Add(player);
                PlayedCards -= player.PlayedCards.Count;
                player.PlayedCards.Clear();
                player.IsGameStarter = false;
            }
        }

        internal Tuple<string, bool>[] GetAvailableDecks()
        {
            return Decks.AvailableDecks.Select(deck => new Tuple<string, bool>(deck.Id, deck.Base)).ToArray();
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
            Players.Single(x => x.PlayedCards.Contains(card)).points++;
        }

        public string[] AllPlayersDetails()
        {
            return players
                .Select(player => $"{player.Name} points: {player.points} {(IsCardCzar(player) ? "Card Czar" : "")}")
                .ToArray();
        }
    }
}
