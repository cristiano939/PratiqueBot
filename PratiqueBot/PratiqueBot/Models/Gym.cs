using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PratiqueBot.Models
{
    public class Gym
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<string> Modalities { get; set; }
        public string Phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

    }
}
