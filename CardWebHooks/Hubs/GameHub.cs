using CardsAgaisntNet;
using CardWebSocks.Cards;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class GameHub : Hub
    {
        readonly GameManager GameManager;
        Game game;
        public GameHub(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("UUIDHandler");
            await base.OnConnectedAsync();
            
        }

        public void GetGameFromID(string guid)
        {
            game = GameManager.FindGameByID(new Guid(guid));
        }

        public async Task HandleUUID(string uuid, string gameID)
        {
            GetGameFromID(gameID);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameID);

            var player = game.FindPlayerById(uuid) ?? new Player(uuid, Context.ConnectionId);
            if (!game.ConnectPlayer(player))
            {
                await Clients.Caller.SendAsync("ReturnToLobby");
                return;
            }

            if (Context.ConnectionId != player.ConnectionID)
            {
                player.ConnectionID = Context.ConnectionId;
            }



            if (game.PlayerCount == 1)
            {
                game.AssignAsCardCzar(player);
                await Clients.Caller.SendAsync("CardCzar");
                player.IsGameStarter = true;
                await Clients.Caller.SendAsync("GameStarter");
                await Clients.Caller.SendAsync("RecieveDeckConfig", game.GetAvailableDecks());
            }
            if (game.HasGameStarted)
            {
                await Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
                await Clients.Caller.SendAsync("ReceiveHand", player.Hand.ToArray());
                await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            }


        }
        public async Task SetName(string name, string gameID)
        {
            GetGameFromID(gameID);
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            player.Name = name;

            await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
        }

        public async Task StartGame(string[] decks, string gameId)
        {
            GetGameFromID(gameId);
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            if (!game.HasGameStarted && player.IsGameStarter)
            {
                game.StartGame(decks);
                await Clients.Group(gameId).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
                await Clients.Caller.SendAsync("ReceiveHand", player.Hand.ToArray());
                await Clients.Group(gameId).SendAsync("RefreshHand");
            }
        }

        public async Task RefreshHand(string id, string gameID)
        {
            GetGameFromID(gameID);
            var player = game.FindPlayerById(id);
            await Clients.Caller.SendAsync("ReceiveHand", player.Hand.ToArray());
            if (player == game.CardCzar)
            {
                await Clients.Caller.SendAsync("CardCzar");
            }
        }

        public async Task ClickCard(string id, string card, string gameID)
        {
            GetGameFromID(gameID);
            var cardHand = game.PlayerPlayCard(id, card);
            if (cardHand != null)
            {
                await Clients.Caller.SendAsync("RecieveSelWCard", cardHand.Item1);
                await Clients.Caller.SendAsync("ReceiveHand", cardHand.Item2);
            }
            RevealWhiteCards(gameID);
        }
        private async void RevealWhiteCards(string gameID)
        {
            if (game.PlayedCards >= (game.CurrentBlackCard.Pick * (game.PlayerCount - 1)))
            {
                await Clients.Group(gameID).SendAsync("ShowWhiteCards", game.PlayedCardsToArray());
            }
        }

        public async Task SelectWinningCard(string id, string card, string gameID)
        {
            GetGameFromID(gameID);
            var player = game.FindPlayerById(id);
            if (player == game.CardCzar)
            {
                game.SelectCard(card);
                await GetNewBlackCard(gameID);
                await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            }
        }

        private async Task GetNewBlackCard(string gameID)
        {
            game.NewBlackCard();
            game.NewCardCzar();
            await Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
            await Clients.Group(gameID).SendAsync("RefreshHand");
            await Clients.Group(gameID).SendAsync("ClearSelectedCards");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            game = GameManager.FindGameByPlayerConnectionId(Context.ConnectionId);
            if(game == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            
            game.DisconnectPlayer(player);
            await Clients.Group(game.Id.ToString()).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            Console.WriteLine(game.Id.ToString());
            if (player.IsGameStarter && game.PlayerCount > 0)
            {
                var newPlayer = game.GetNextPlayer();
                newPlayer.IsGameStarter = true;

                await Clients.Client(newPlayer.ConnectionID).SendAsync("GameStarter");
                await Clients.Client(newPlayer.ConnectionID).SendAsync("RecieveDeckConfig", game.GetAvailableDecks());
            }
            if (game.HasGameStarted)
            {
                
                if (game.PlayerCount > 0)
                {
                    if (player == game.CardCzar)
                    {
                        await GetNewBlackCard(game.Id.ToString());
                    }
                    RevealWhiteCards(game.Id.ToString());

                }
            }

            GameManager.RemoveGameIfEmpty(game.Id);
            await base.OnDisconnectedAsync(exception);
        }

        

        

        

        

        

        
        

        


    }
}