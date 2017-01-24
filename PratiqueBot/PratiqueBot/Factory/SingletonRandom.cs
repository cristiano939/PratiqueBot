using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PratiqueBot.Factory
{
    public class SingletonRandom
    {
        public static Random random;

        public static Random GetInstance()
        {
            if (random == null)
            {
                random = new Random();
            }
            return random;
        }

        public static void ResetRandom()
        {
            random = new Random();
        }
    }
}
