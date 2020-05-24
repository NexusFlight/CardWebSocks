using CardWebSocks.Cards;
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CardWebSocks.Hubs
{
    public class GameHub : Hub
    {
        DBContext dBContext;
        readonly GameManager GameManager;
        Game game;
        public GameHub(GameManager gameManager, DBContext dBContext)
        {
            GameManager = gameManager;
            this.dBContext = dBContext;
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
            }

            await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());

        }
        public async Task SetName(string name, string gameID)
        {
            GetGameFromID(gameID);
            var player = game.FindPlayerByConnectionId(Context.ConnectionId);
            player.Name = name;

            await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
        }

        public void ImportFromDB(string gameID,string playID)
        {
            var deck = dBContext.GetDeckByPlayID(playID);
            GetGameFromID(gameID);
            game.AddDeck(deck.Result);
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

        public async Task ClickCard(string id, int card, string gameID)
        {
            GetGameFromID(gameID);
            var cardHand = game.PlayerPlayCard(id, card);
            await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
            if (cardHand != null)
            {
                await Clients.Caller.SendAsync("RecieveSelWCard", cardHand.Item1);
                await Clients.Caller.SendAsync("ReceiveHand", cardHand.Item2);
            }
            RevealWhiteCards(gameID);
        }
        private async void RevealWhiteCards(string gameID)
        {
            if (game.PlayedCards >= (game.CurrentBlackCard.Pick * (game.PlayerCount - 1)) && !game.WhiteCardsAreShown)
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
                var winner = game.SelectCard(card);
                await GetNewBlackCard(gameID);
                await Clients.Group(gameID).SendAsync("ReceivePlayerDetails", game.AllPlayersDetails());
                await Clients.Group(gameID).SendAsync("RecieveWinner", winner.Name);
            }
        }

        private async Task GetNewBlackCard(string gameID)
        {
            var isGameOver = game.NewBlackCard();
            if (!isGameOver)
            {
                game.NewCardCzar();
                await Clients.Group(gameID).SendAsync("RecieveBlackCard", game.CurrentBlackCard.Text, "Pick " + game.CurrentBlackCard.Pick);
                await Clients.Group(gameID).SendAsync("RefreshHand");
                await Clients.Group(gameID).SendAsync("ClearSelectedCards");
            }
            else
            {
                await Clients.Group(gameID).SendAsync("GameOver",game.GetWinningPlayer());
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            game = GameManager.FindGameByPlayerConnectionId(Context.ConnectionId);
            if (game == null)
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

                if (game.PlayerCount > 0 && player.PlayedCards != null)
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