using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Models
{
    public class CardModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public string Element { get; set; }
        public string Type { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }


        public CardModel(string id, string name, int damage, string element, string type, int userId, string userName)
        {
            Id = id;
            Name = name;
            Damage = damage;
            Element = element;
            Type = type;
            UserId = userId;
            UserName = userName;
        }

        public CardModel() //für Serilization
        {

        }
    }
}
