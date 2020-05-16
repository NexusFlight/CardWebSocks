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
            games = new List<Game>();
            games.Add(new Game("bob"));
            games.Add(new Game("Paul"));

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
    }
}
