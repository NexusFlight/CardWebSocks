//CARD CAST NOW OFFLINE

//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;

//namespace CardWebSocks.Cards
//{
//    class CardCastHandler
//    {
//        string api = "https://api.cardcastgame.com/v1/decks/";
//        string cards = "/cards";
//        CardCast CDeck;

//        public CardCastHandler()
//        {

//        }

//        public Deck GetCardCastCards(string code)
//        {
//            string json = "";
//            using(WebClient client = new WebClient())
//            {
//                json = client.DownloadString(string.Concat(api, code, cards));
//            }
//            CDeck = JsonConvert.DeserializeObject<CardCast>(json);
//            return ConvertCardCastToBase(code);
//        }

//        public Deck ConvertCardCastToBase(string code)
//        {
//            List<BlackCard> blackCards = new List<BlackCard>();
//            List<string> whiteCards = new List<string>();
//            for (int i = 0; i < CDeck.Calls.Count; i++)
//            {
//                var card = CDeck.Calls[i].Text;
//                var pick = card.Count - 1;
//                var text = string.Join("____", card.ToArray());
//                blackCards.Add(new BlackCard(text, pick));
//            }
//            for (int i = 0; i < CDeck.Responses.Count; i++)
//            {
//                var card = CDeck.Responses[i].Text;
//                var text = string.Join(',', card.ToArray());
//                whiteCards.Add(text);
//            }
//            return new Deck(code,blackCards, whiteCards);
//        }

//    }
//}
