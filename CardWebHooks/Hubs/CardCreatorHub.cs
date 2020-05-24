using CardWebSocks.Cards;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            if (dbID == null)
            {
                var dbid = Guid.NewGuid().ToString();
                dBContext.CreateDeck(new Deck(dbid));
                await Clients.Caller.SendAsync("SetDbId", dbid);
            }
            else
            {

                await Clients.Caller.SendAsync("DeckPlayID", GetDeck(dbID).PlayID);
                await GetAllCards(dbID);
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

        public async void AddWhiteCard(string dbID, string text)
        {
            var deck = GetDeck(dbID);
            deck.AddWhiteCard(text);
            UpdateDB(deck);
            await GetAllCards(dbID);
        }

        public async void AddBlackCard(string dbID, string text, string pick)
        {
            var deck = GetDeck(dbID);
            deck.AddBlackCard(new BlackCard(text, Convert.ToInt32(pick)));
            UpdateDB(deck);
            await GetAllCards(dbID);
        }

        public async Task GetAllCards(string dbID)
        {
            var deck = GetDeck(dbID);
            await Clients.Caller.SendAsync("DeckName", deck.Name);
            await Clients.Caller.SendAsync("DeckPlayID", deck.PlayID);
            await Clients.Caller.SendAsync("RecieveBlackCards", deck.BlackCards.ToArray());
            await Clients.Caller.SendAsync("RecieveWhiteCards", deck.WhiteCards.ToArray());
        }

        public async void RemoveCard(string dbID, string card)
        {

            var deck = GetDeck(dbID);
            var index = Convert.ToInt32(card.Substring(1, card.Length-1));
            Console.WriteLine(index);
            if (card.Contains('B'))
            {
                deck.RemoveBlackCard(deck.BlackCards[index].Text);
            }
            else
            {
                deck.RemoveWhiteCard(deck.WhiteCards[index]);
            }
            UpdateDB(deck);
            await GetAllCards(dbID);
        }
    }
}
