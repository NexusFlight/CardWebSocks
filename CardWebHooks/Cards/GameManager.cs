using CardsAgaisntNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWebSocks.Cards
{
    public class GameManager
    {
        private readonly IList<Game> games;
        public IEnumerable<Game> Games => games;
        public GameManager()
        {
            games = new List<Game>
            {
                new Game("bob"),
                new Game("Paul")
            };

        }

        public Guid AddGame(string name)
        {
            var game = new Game(name);
            games.Add(game);
            return game.Id;
        }

        public Game FindGameByID(Guid id)
        {
            var game = games.FirstOrDefault(x => x.Id == id);
            if(game == null)
            {
                throw new Exception("No Game Found");
            }
            return game;
        }

        public Game FindGameByPlayerConnectionId(string connectionId)
        {
            foreach (var game in games)
            {
                var player = game.FindPlayerByConnectionId(connectionId);
                if (player != null)
                {
                    return game;
                }
            }
            return null;
        }

        internal void RemoveGame(Game game)
        {
            games.Remove(game);
        }

        internal void RemoveGameIfEmpty(Guid id)
        {
            var game = games.Where(game => game.Id == id).FirstOrDefault();
            if (game.PlayerCount == 0)
            {
                RemoveGame(game);
            }
        }
    }
}
