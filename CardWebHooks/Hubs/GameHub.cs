using CardsAgaisntNet;
using CardWebHooks.Cards;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class GameHub : Hub
    {
        Game game;

        public GameHub(Game game)
        {
            this.game = game;
        }
        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("ReceivePlayerDetails",game.AllPlayersDetails());
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var player = game.players.Find(x => x.ConnectionID == Context.ConnectionId);
            if (player == game.cardCzar)
            {
                getNewBlackCard();
            }
            game.disconnectedPlayers.Add(player);
            game.players.Remove(player);
            game.playedCards -= player.playedCards.Count;
            player.playedCards.Clear();
            if (game.players.Count > 0)
            {
                RevealWhiteCards();
            }
            Clients.All.SendAsync("ReceivePlayers", game.AllPlayersDetails());


            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public void getNewBlackCard()
        {
            game.NewBlackCard();
            Clients.All.SendAsync("RecieveBlackCard", game.currentBlackCard.Text, "Pick " + game.currentBlackCard.Pick);
            Clients.All.SendAsync("RefreshHand");
            Clients.All.SendAsync("ClearSelectedCards");
        }

        public void handleUUID(string uuid, string name)
        {
            var player = game.players.Find(x => x.ID == uuid) ?? game.disconnectedPlayers.Find(x => x.ID == uuid);
            if (game.disconnectedPlayers.Contains(player))
            {
                game.players.Add(player);
                game.disconnectedPlayers.Remove(player);
            }
            if (player == null)
            {

                player = new Player(game.Decks.Deck, uuid, name, Context.ConnectionId);
                game.players.Add(player);
            }
            else
            {
                foreach (var item in player.playedCards)
                {
                    Clients.Caller.SendAsync("RecieveSelWCard", item);
                }
            }
            if (name != player.Name)
            {
                player.Name = name;
            }
            if (Context.ConnectionId != player.ConnectionID)
            {
                player.ConnectionID = Context.ConnectionId;
            }
            Clients.Caller.SendAsync("RecieveBlackCard", game.currentBlackCard.Text, "Pick " + game.currentBlackCard.Pick);
            Clients.Caller.SendAsync("ReceiveHand", player.hand.ToArray());

            if (game.players.Count == 1 || player == game.cardCzar)
            {
                game.cardCzar = player;
                Clients.Caller.SendAsync("CardCzar");
            }

            Clients.All.SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());

        }

        private async void RevealWhiteCards()
        {

            if (game.playedCards >= (game.currentBlackCard.Pick * (game.players.Count - 1)))
            {
                await Clients.All.SendAsync("ShowWhiteCards", game.PlayedCardsToArray());
            }
        }

        public async Task ClickCard(string id, string card)
        {
            var cardHand = game.PlayerPlayCard(id, card);
            if (cardHand != null)
            {
                await Clients.Caller.SendAsync("RecieveSelWCard", cardHand.Item1);
                await Clients.Caller.SendAsync("ReceiveHand", cardHand.Item2);
            }
            RevealWhiteCards();
        }

        public void RefreshHand(string id)
        {
            var player = game.players.Find(x => x.ID == id);
            Clients.Caller.SendAsync("ReceiveHand", player.hand.ToArray());
            if (player == game.cardCzar)
            {
                Clients.Caller.SendAsync("CardCzar");
            }
        }

        public void SelectCard(string id, string card)
        {
            var player = game.players.Find(x => x.ID == id);
            if (player == game.cardCzar)
            {
                game.SelectCard(card);
                getNewBlackCard();
                Clients.All.SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            }
        }


    }
}