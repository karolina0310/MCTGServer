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
    public class PackagesDB
    {
        private PostgresDB db;
        public PackagesDB()
        {
            db= new PostgresDB();
        }
        public  async Task<bool> CreatePackage(PackageModel pack)
        {

            db.Connect();
            string commandText = "INSERT INTO packages (cards) VALUES (@cards)";
            await using (var cmd = new NpgsqlCommand(commandText, db.Connection))
            {
                //bereitet Befehle 
                cmd.Parameters.AddWithValue("cards", pack.Cards);
               
                int affectedRows= await cmd.ExecuteNonQueryAsync(); //führt SQL Anweisung aus
                db.Disconnect();
                return affectedRows == 1;
            }

        }

        public async Task<bool> AddCards(List<CardModel> cards)
        {
            db.Connect();
            var insertQuery = new StringBuilder("INSERT INTO cards (id, name, damage, element, type) VALUES ");

            // speichert Parameter für jede Karte in Liste
            var valueStrings = new List<string>();
            int counter = 0;
            foreach (var card in cards)
            {
                string values = $"(@id{counter}, @name{counter}, @damage{counter}, @element{counter}, @type{counter})";
                valueStrings.Add(values);
                counter++;
            }

            
            insertQuery.Append(string.Join(", ", valueStrings));

            await using (var cmd = new NpgsqlCommand(insertQuery.ToString(), db.Connection))
            {
                
                counter = 0;
                //setzt die Parameter pro Karte 
                foreach (var card in cards)
                {
                    cmd.Parameters.AddWithValue($"@id{counter}", card.Id);
                    cmd.Parameters.AddWithValue($"@name{counter}", card.Name);
                    cmd.Parameters.AddWithValue($"@damage{counter}", card.Damage);
                    cmd.Parameters.AddWithValue($"@element{counter}", card.Element);
                    cmd.Parameters.AddWithValue($"@type{counter}", card.Type);
                    counter++;
                }

                int affectedRows = await cmd.ExecuteNonQueryAsync(); 
                db.Disconnect();

                
                return affectedRows == cards.Count;
            }
        }

       
        



    }
}
