using CardWebSocks.Cards;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWebSocks.Hubs
{
    public class LobbyHub : Hub
    {
        GameManager gameManager;
        public LobbyHub(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public override Task OnConnectedAsync()
        {
            ShowAllGames();
            return base.OnConnectedAsync();
        }
        public void ShowAllGames()
        {
            Clients.All.SendAsync("ReceiveAllGames", gameManager.Games.ToArray());
        }

        public void AddGame(string gameName)
        {
            var gameID = gameManager.AddGame(gameName);
            ShowAllGames();
            Clients.Caller.SendAsync("SetGameID", gameID);
        }




    }
}
