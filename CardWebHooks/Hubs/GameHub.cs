using CardsAgaisntNet;
using CardWebSocks.Cards;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class GameHub : Hub
    {
        GameManager GameManager;
        Game game;
        public GameHub(GameManager gameManager)
        {
            GameManager = gameManager;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public void GetGameID(string guid)
        {
            game = GameManager.FindGameByID(new Guid(guid));
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameID(gameID);
            }
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            if (player == game.CardCzar)
            {
                GetNewBlackCard();
            }
            game.DisconnectPlayer(player);
            if (game.PlayerCount > 0)
            {
                RevealWhiteCards();
            }
            Clients.Group(gameID).SendAsync("ReceivePlayers", game.AllPlayersDetails());


            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public void GetNewBlackCard()
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameID(gameID);
            }
            game.NewBlackCard();
            Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
            Clients.Group(gameID).SendAsync("RefreshHand");
            Clients.Group(gameID).SendAsync("ClearSelectedCards");
        }

        public void HandleUUID(string uuid, string name, string gameID)
        {
            GetGameID(gameID);
            Groups.AddToGroupAsync(Context.ConnectionId,gameID);
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                Context.Items.Remove(Context.ConnectionId);
             
            }
            Context.Items.Add(Context.ConnectionId, gameID);
            var player = game.FindPlayerById(uuid) ?? new Player(game.Decks.Deck, uuid, name, Context.ConnectionId);
            game.ConnectPlayer(player);
            
            if (name != player.Name)
            {
                player.Name = name;
            }
            if (Context.ConnectionId != player.ConnectionID)
            {
                player.ConnectionID = Context.ConnectionId;
            }
            
            Clients.Caller.SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
            Clients.Caller.SendAsync("ReceiveHand", player.hand.ToArray());

            if (game.PlayerCount == 1)
            {
                game.AssignAsCardCzar(player);
                Clients.Caller.SendAsync("CardCzar");
            }

            Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
        }

        private async void RevealWhiteCards()
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameID(gameID);
            }
            if (game.PlayedCards >= (game.CurrentBlackCard.Pick * (game.PlayerCount - 1)))
            {
                await Clients.Group(gameID).SendAsync("ShowWhiteCards", game.PlayedCardsToArray());
            }
        }

        public async Task ClickCard(string id, string card)
        {
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                GetGameID((string)Context.Items[Context.ConnectionId]);
            }
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
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                GetGameID((string)Context.Items[Context.ConnectionId]);
            }
            var player = game.FindPlayerById(id);
            Clients.Caller.SendAsync("ReceiveHand", player.hand.ToArray());
            if (player == game.CardCzar)
            {
                Clients.Caller.SendAsync("CardCzar");
            }
        }

        public void SelectCard(string id, string card)
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameID(gameID);
            }
            var player = game.FindPlayerById(id);
            if (player == game.CardCzar)
            {
                game.SelectCard(card);
                GetNewBlackCard();
                Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            }
        }


    }
}