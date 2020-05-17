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
            Clients.Caller.SendAsync("UUIDHandler");
            return base.OnConnectedAsync();
        }

        public void GetGameFromID(string guid)
        {
            game = GameManager.FindGameByID(new Guid(guid));
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameFromID(gameID);
            }
            else
            {
                return base.OnDisconnectedAsync(exception);
            }
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);

            game.DisconnectPlayer(player);
            if (player.IsGameStarter && game.PlayerCount > 0)
            {
                var newPlayer = game.GetNextPlayer();
                newPlayer.IsGameStarter = true;

                Clients.Client(newPlayer.ConnectionID).SendAsync("GameStarter");
                Clients.Client(newPlayer.ConnectionID).SendAsync("RecieveDeckConfig", game.GetAvailableDecks());
            }
            if (game.HasGameStarted)
            {
                if (player == game.CardCzar)
                {
                    GetNewBlackCard();
                }
                if (game.PlayerCount > 0)
                {
                    
                    RevealWhiteCards();
                }
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
                GetGameFromID(gameID);
            }
            game.NewBlackCard();
            game.NewCardCzar();
            Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
            Clients.Group(gameID).SendAsync("RefreshHand");
            Clients.Group(gameID).SendAsync("ClearSelectedCards");
        }

        public void SetName(string name)
        {
            var gameID = GetGameFromConnection();
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            player.Name = name;
            Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
        }

        public void HandleUUID(string uuid, string gameID)
        {
            GetGameFromID(gameID);
            Groups.AddToGroupAsync(Context.ConnectionId,gameID);
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                Context.Items.Remove(Context.ConnectionId);
             
            }
            Context.Items.Add(Context.ConnectionId, gameID);
            var player = game.FindPlayerById(uuid) ?? new Player(uuid, Context.ConnectionId);
            game.ConnectPlayer(player);
            
            if (Context.ConnectionId != player.ConnectionID)
            {
                player.ConnectionID = Context.ConnectionId;
            }
            
            

            if (game.PlayerCount == 1)
            {
                game.AssignAsCardCzar(player);
                Clients.Caller.SendAsync("CardCzar");
                player.IsGameStarter = true;
                Clients.Caller.SendAsync("GameStarter");
               Clients.Caller.SendAsync("RecieveDeckConfig", game.GetAvailableDecks());
            }
            if (game.HasGameStarted)
            {
                Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
                Clients.Caller.SendAsync("ReceiveHand", player.Hand.ToArray());
                Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            }

            
        }

        public string GetGameFromConnection()
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameFromID(gameID);
            }
            return gameID;
        }

        public void StartGame(string[] decks)
        {
            var gameID = GetGameFromConnection();
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            if (!game.HasGameStarted && player.IsGameStarter)
            {
                game.StartGame(decks);
                Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
                Clients.Caller.SendAsync("ReceiveHand", player.Hand.ToArray());
                Clients.Group(gameID).SendAsync("RefreshHand");
            }
        }

        private async void RevealWhiteCards()
        {
            var gameID = "";
            if (Context.Items.ContainsKey(Context.ConnectionId))
            {
                gameID = (string)Context.Items[Context.ConnectionId];
                GetGameFromID(gameID);
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
                GetGameFromID((string)Context.Items[Context.ConnectionId]);
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
                GetGameFromID((string)Context.Items[Context.ConnectionId]);
            }
            var player = game.FindPlayerById(id);
            Clients.Caller.SendAsync("ReceiveHand", player.Hand.ToArray());
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
                GetGameFromID(gameID);
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