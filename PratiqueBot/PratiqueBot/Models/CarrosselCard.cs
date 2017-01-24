using Lime.Messaging.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PratiqueBot.Models
{
    public class CarrosselCard
    {
        public MediaLink CardMediaHeader { get; set; }
        public string CardContent { get; set; }
        public List<CarrosselOptions> options { get; set; }
    }
}
