using PratiqueBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PratiqueBot
{
    public class Settings
    {
        public string BotIdentifier { get; set; }
        public List<Gym> Gyms { get; set; }
       public List<string> Sports { get; set; }


    }
}
