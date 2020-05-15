//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.RegularExpressions;
//using Newtonsoft.Json;
//namespace CardsAgaisntNet
//{
//    class Program
//    {
//        //static void Main(string[] args)
//        //{
//        //    string json = File.ReadAllText(@"entireList.json");
//        //    Deck3 deck = JsonConvert.DeserializeObject<Deck3>(json);


//        //    foreach (var item in deck.GetType().GetProperties())
//        //    {
//        //        if (item.PropertyType == typeof(Deck2))
//        //        {
//        //            List<BlackCard> blackCards = new List<BlackCard>();
//        //            List<string> whiteCards = new List<string>();
//        //            Deck2 curDeck = ((Deck2)item.GetValue(deck));
//        //            string id = curDeck.Name;
//        //            if (curDeck.Black.Count > 0)
//        //            {
//        //                int startLoc = Convert.ToInt32(curDeck.Black[0]);
//        //                int endLoc = Convert.ToInt32(curDeck.Black[curDeck.Black.Count-1]);
//        //                for (int i = startLoc; i <= endLoc; i++)
//        //                {
//        //                    blackCards.Add(deck.BlackCards[i]);
//        //                }
//        //            }
//        //            if (curDeck.White.Count > 0)
//        //            {
//        //                int startLoc = Convert.ToInt32(curDeck.White[0]);
//        //                int endLoc = Convert.ToInt32(curDeck.White[curDeck.White.Count-1]);
//        //                for (int i = startLoc; i <= endLoc; i++)
//        //                {
//        //                    whiteCards.Add(deck.WhiteCards[i]);
//        //                }
//        //            }

//        //            id = Regex.Replace(id, @"[^\w\s\-\+]", "");
//        //            Deck deck1 = new Deck(id, blackCards, whiteCards);
//        //            deck1.OutputToJson();
//        //        }
//        //    }


//        //    Console.ReadLine();
//        //}
//        static void Main(string[] args)
//        {
//            var files = new DirectoryInfo("jsons/");
//            string json;
//            Deck deck = new Deck();
//            foreach (var file in files.GetFiles())
//            {
//                json = File.ReadAllText(file.FullName);
//                deck.AddDeck(JsonConvert.DeserializeObject<Deck>(json));
//            }
//            while (true)
//            {
//                Player player1 = new Player();
//                Random rand = new Random();

//                for (int i = 0; i < 7; i++)
//                {
//                    int cardNum = rand.Next(0, deck.WhiteCards.Count);
//                    player1.GiveCard(deck.WhiteCards[cardNum]);
//                    deck.WhiteCards.RemoveAt(cardNum);
//                }

//                BlackCard blackCard = deck.ShowBlackCard();

//                Console.WriteLine(blackCard.Text);

//                Console.WriteLine($"Pick:{blackCard.Pick}");


//                for (int i = 0; i < blackCard.Pick; i++)
//                {

//                    Console.WriteLine(player1.PlayCard(rand.Next(0, player1.hand.Count)));
//                }

//                for (int i = 0; i < blackCard.Pick; i++)
//                {
//                    int cardNum = rand.Next(0, deck.WhiteCards.Count);
//                    player1.GiveCard(deck.WhiteCards[cardNum]);
//                    deck.WhiteCards.RemoveAt(cardNum);
//                }

//            }
//        }
//    }
//}
