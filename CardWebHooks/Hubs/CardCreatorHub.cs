using CardWebSocks.Cards;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWebSocks.Hubs
{
    public class CardCreatorHub : Hub
    {
        DBContext dBContext;
        public CardCreatorHub(DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

       


        public async void SetDataBase(string dbID)
        {
            if(dbID == null ||dBContext.GetDeckByID(dbID).Result == null)
            {
                var dbid = Guid.NewGuid().ToString();
                await Clients.Caller.SendAsync("SetDbId",dbid);
                await dBContext.CreateDeck(new Deck(dbid));
            }
            else
            {
                GetAllCards(dbID);
            }
                
        }

        public Deck GetDeck(string dbID)
        {
            return dBContext.GetDeckByID(dbID).Result;
        }

       public void GetDeckAsJson(string dbID)
        {
            Clients.Caller.SendAsync("DownloadCards", JsonConvert.SerializeObject(GetDeck(dbID)));
        }

        public void UpdateDB(Deck deck)
        {
            dBContext.UpdateDeck(deck);
        }

        public void UpdateDeckName(string dbID, string name)
        {
            var deck = GetDeck(dbID);
            deck.Name = name;
            UpdateDB(deck);
        }

        public void AddWhiteCard(string dbID, string text)
        {
            var deck = GetDeck(dbID);
            deck.AddWhiteCard(text);
            UpdateDB(deck);
            GetAllCards(dbID);
        }

        public void AddBlackCard(string dbID, string text, string pick)
        {
            var deck = GetDeck(dbID);
            deck.AddBlackCard(new BlackCard(text, Convert.ToInt32(pick)));
            UpdateDB(deck);
            GetAllCards(dbID);
        }

        public async Task GetAllCards(string dbID)
        {
            var deck = GetDeck(dbID);
            await Clients.Caller.SendAsync("DeckName", deck.Name);
            await Clients.Caller.SendAsync("RecieveBlackCards", deck.BlackCards.ToArray());
            await Clients.Caller.SendAsync("RecieveWhiteCards",deck.WhiteCards.ToArray());
        }

        public void RemoveCard(string dbID, string card)
        {

            var deck = GetDeck(dbID);
            var index = Convert.ToInt32(card.Substring(1, 1));
            Console.WriteLine(index);
            if (card.Contains('B')) {
                deck.RemoveBlackCard(deck.BlackCards[index].Text);
            }
            else
            {
                deck.RemoveWhiteCard(deck.WhiteCards[index]);
            }
            UpdateDB(deck);
            GetAllCards(dbID);
        }
    }
}
