using MonsterCardServer.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonsterCardServer.Database
{
    public class CardsDB
    {
        private PostgresDB db;
        public CardsDB()
        {
            db = new PostgresDB();
        }

        public async Task<List<CardModel>> GetCards(UserModel user)
        {
            db.Connect();
            string commandText = "SELECT id,name,element,damage,type from cards WHERE username=@user";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                cmd.Parameters.AddWithValue("user", user.Username);


                NpgsqlDataReader r = await cmd.ExecuteReaderAsync();
                List<CardModel> cards = new List<CardModel>();
                while (await r.ReadAsync())
                {
                    var card = new CardModel();
                    card.Id = r.GetString(0);
                    card.Name = r.GetString(1);
                    card.Element = r.GetString(2);
                    card.Damage = r.GetDouble(3);
                    card.Type = r.GetString(4);
                    cards.Add(card);

                }

                db.Disconnect();
                return cards;
            }
        }

        public async Task<List<CardModel>> GetDeck(UserModel user)
        {
            db.Connect();
            //Deck ist gespeichert im user als Array, es wird jede Karte selected, die beim User im Deck ist
            string commandText = "SELECT c.id, c.name, c.element, c.damage, c.type FROM cards c JOIN users u ON c.id = ANY(u.userdeck) WHERE u.username=@user";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                cmd.Parameters.AddWithValue("user", user.Username);


                NpgsqlDataReader r = await cmd.ExecuteReaderAsync();
                List<CardModel> cards = new List<CardModel>();
                while (await r.ReadAsync())
                {
                    var card = new CardModel();
                    card.Id = r.GetString(0);
                    card.Name = r.GetString(1);
                    card.Element = r.GetString(2);
                    card.Damage = r.GetDouble(3);
                    card.Type = r.GetString(4);
                    cards.Add(card);

                }

                db.Disconnect();
                return cards;
            }

        }

        public async Task<bool> SetDeck(List<string> deck, string token)
        {
            string deckString = "{" + String.Join(",", deck) + "}";

            db.Connect();
            string commandText = "UPDATE users SET userdeck=@deck WHERE token=@token";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                cmd.Parameters.AddWithValue("token", token);
                cmd.Parameters.AddWithValue("deck", deck);


                int affectedRows = await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
                db.Disconnect();
                return affectedRows == 1;
            }

        }
    }
}
