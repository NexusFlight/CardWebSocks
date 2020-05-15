using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWebHooks.Cards
{
    public class Games
    {
        public List<Game> games = new List<Game>();

        public Games()
        {
            CreateNewGame();
        }
        public void CreateNewGame()
        {
            games.Add(new Game());
        }
    }
}
