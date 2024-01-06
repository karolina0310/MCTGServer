using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Coins { get; set; }
        public List<string> UserDeck { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Elo { get; set; }
        public int GamesPlayed { get; set; }
        public string Image { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }

        public UserModel(int id, string username, string password, int coins, List<string> userDeck, int wins, int loses, int elo, int gamesPlayed)
        {
            Id = id;
            Username = username;
            Password = password;
            Coins = coins;
            UserDeck = userDeck;
            Wins = wins;
            Loses = loses;
            Elo = elo;
            GamesPlayed = gamesPlayed;
        }

        public UserModel(int id, string username, int coins, List<string> userDeck, int wins, int loses, int elo, int gamesPlayed, string displayname,string bio, string image )
        {
            Id = id;
            Username = username;
            Coins = coins;
            UserDeck = userDeck;
            Wins = wins;
            Loses = loses;
            Elo = elo;
            GamesPlayed = gamesPlayed;
            DisplayName = displayname;
            Bio = bio;
            Image = image;
            Password = "";

        }

        public UserModel() 
        { 
        }
    }
}
