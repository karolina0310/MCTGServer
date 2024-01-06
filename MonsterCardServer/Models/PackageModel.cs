using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Models
{
    public class PackageModel
    {
        public int Id { get; set; }
        public List<string> Cards { get; set; } = new List<string>();

        public PackageModel(int id, List<string> cards) 
        {
            Id = id;
            Cards = cards;
        }

        public PackageModel()
        {

        }
    }
}
