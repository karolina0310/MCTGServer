using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Database
{
    public class PostgresDB
    {
        public NpgsqlConnection? Connection;
        public void Connect()
        {
           var connectionString = "Host=localhost;Username=karo;Password=karo;Database=mctgdb;Port=5432";
           Connection=new NpgsqlConnection(connectionString);
           Connection.Open();
           Console.WriteLine("DB connected");
        }

        public void Disconnect()
        {
            Connection.Close();
        }
    }
}
