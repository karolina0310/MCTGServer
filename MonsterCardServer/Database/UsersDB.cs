using MonsterCardServer.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Database
{
     public class UsersDB
    {
        private PostgresDB db;
        public UsersDB()
        {
            db= new PostgresDB();
        }
        public  async Task<bool> AddUser(UserModel user)
        {

            db.Connect();
            string commandText = "INSERT INTO users (username,password,coins,userdeck,wins,loses,elo,gamesplayed,displayname,bio,image) VALUES (@username,@password,@coins,@userdeck,@wins,@loses,@elo,@gamesplayed,'','','')";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                //bereitet Befehle 
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", BCrypt.Net.BCrypt.HashPassword(user.Password));
                cmd.Parameters.AddWithValue("coins", 20);
                cmd.Parameters.AddWithValue("userdeck", new string[] { });
                cmd.Parameters.AddWithValue("wins", 0);
                cmd.Parameters.AddWithValue("loses", 0);
                cmd.Parameters.AddWithValue("elo", 1000);
                cmd.Parameters.AddWithValue("gamesplayed", 0);
               
                int affectedRows= await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
                db.Disconnect();
                return affectedRows == 1;
            }

        }

		public async Task<bool> ChangeUserStats(UserModel user)
		{
			db.Connect();
			string commandText = "UPDATE users SET wins = @wins, loses = @loses, elo = @elo, gamesplayed = @gamesplayed WHERE username = @user";
			await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
			{
				//bereitet Befehle
				cmd.Parameters.AddWithValue("wins", user.Wins);
				cmd.Parameters.AddWithValue("loses", user.Loses);
				cmd.Parameters.AddWithValue("elo", user.Elo);
				cmd.Parameters.AddWithValue("gamesplayed", user.GamesPlayed);
				cmd.Parameters.AddWithValue("user", user.Username);

				int affectedRows = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
				db.Disconnect();
				return affectedRows == 1;
			}
		}

		public async Task<bool> ChangeUserProfile(UserModel oldUser, UserModel newUser)
        {

            db.Connect();
            string commandText = "UPDATE users SET displayname = @name, bio = @bio, image = @image WHERE username = @user";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                //bereitet Befehle
                cmd.Parameters.AddWithValue("user", oldUser.Username);
                cmd.Parameters.AddWithValue("name", newUser.Username);
                cmd.Parameters.AddWithValue("bio", newUser.Bio);
                cmd.Parameters.AddWithValue("image", newUser.Image);

                int affectedRows = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
                db.Disconnect();
                return affectedRows == 1;
            }

        }

        public  async Task<UserModel> GetUserByToken(string token)
        {
            db.Connect();
            string commandText = "SELECT id,username,coins,userdeck,wins,loses,elo,gamesplayed,displayname,bio,image FROM users WHERE token=@token";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                cmd.Parameters.AddWithValue("token", token);


                NpgsqlDataReader r = await cmd.ExecuteReaderAsync();
                await r.ReadAsync();
                var user= new UserModel(r.GetInt32(0), r.GetString(1), r.GetInt32(2), r.GetFieldValue<List<string>>(3), r.GetInt32(4), r.GetInt32(5), r.GetInt32(6), r.GetInt32(7), r.GetString(8), r.GetString(9), r.GetString(10));

                db.Disconnect();
                return user;
            }
        }

        public  async Task<UserModel> GetUserByUsername(string username)
        {
            db.Connect();
            string commandText = "SELECT username,coins,wins,loses,elo,gamesplayed,displayname,bio,image,password FROM users WHERE username=@username";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                cmd.Parameters.AddWithValue("username", username);


                NpgsqlDataReader r = await cmd.ExecuteReaderAsync();
                await r.ReadAsync();
                var user = new UserModel();
                user.Username = r.GetString(0);
                user.Coins = r.GetInt32(1);
                user.Wins = r.GetInt32(2);
                user.Loses = r.GetInt32(3);
                user.Elo = r.GetInt32(4);
                user.GamesPlayed = r.GetInt32(5);
                user.DisplayName = r.GetString(6);
                user.Bio = r.GetString(7);
                user.Image = r.GetString(8);
                user.Password = r.GetString(9);
                db.Disconnect();
                return user;

            }
        }

        public  async Task Login(string token,UserModel user)
        {
            db.Connect();
            string commandText = "UPDATE users SET token=@token WHERE username=@username";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("token", token);

                int affectedRows = await cmd.ExecuteNonQueryAsync();
                db.Disconnect();
                return;
            }

        }

        public async Task<bool> BuyPackage(UserModel user)
        {
            int affectedRows = 0;
            db.Connect();
            string commandText = "UPDATE cards SET username=@user WHERE id IN (SELECT unnest(cards) FROM packages WHERE id = (SELECT id FROM packages ORDER BY id LIMIT 1))";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                //bereitet Befehle 
                cmd.Parameters.AddWithValue("user", user.Username);

                affectedRows = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
                
                
            }
            if(affectedRows ==0)
            {
                db.Disconnect();
                return false;
            }
            commandText = "DELETE FROM packages WHERE id = (SELECT id FROM packages ORDER BY id LIMIT 1)";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                affectedRows = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus


            }
            user.Coins -= 5;
            string commandTextCoins = "UPDATE users SET coins=@coins WHERE username=@username";
            await using (var cmd = new NpgsqlCommand(commandTextCoins, db.Connection))
            {
                //bereitet Befehle 
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("coins", user.Coins);

                affectedRows = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus


            }

            db.Disconnect();
            return affectedRows==1;
        }
        public async Task<List<UserModel>> GetScoreboard()
        {
            db.Connect();
            string commandText = "SELECT username,wins,loses,elo FROM users ORDER BY elo desc";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                NpgsqlDataReader r = await cmd.ExecuteReaderAsync();

                List<UserModel> users = new List<UserModel>();
                while (await r.ReadAsync())
                {
                    var user = new UserModel();
                    user.Username = r.GetString(0);
                    user.Wins = r.GetInt32(1);
                    user.Loses = r.GetInt32(2);
                    user.Elo = r.GetInt32(3);
                    users.Add(user);

                }

                db.Disconnect();
                return users;
            }
        }

        public async Task TruncateAllTables()
        {

            db.Connect();
            await using (var cmd = new NpgsqlCommand("TRUNCATE TABLE users CASCADE", db.Connection))
            {
                _ = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
            }
            await using (var cmd = new NpgsqlCommand("TRUNCATE TABLE cards CASCADE", db.Connection))
            {
                _ = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
            }
            await using (var cmd = new NpgsqlCommand("TRUNCATE TABLE packages CASCADE", db.Connection))
            {
                _ = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
            }

            db.Disconnect();
            return;

        }


    }

}
