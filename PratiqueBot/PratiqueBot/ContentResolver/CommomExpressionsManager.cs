using PratiqueBot.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PratiqueBot.ContentResolver
{
    public class CommomExpressionsManager
    {
        public bool IsFirstMessage(string message)
        {
            if (IsGreeting(message) || IsHello(message))
            {
                return true;
            }
            return false;
        }

        public bool IsByeMessage(string message)
        {
            Regex regex = new Regex(@"(!?)(tchau|Tchau|flw|flws|até breve|até logo|ate breve|ate logo|xau|bye)");
            if (regex.IsMatch(message))
            {
                return true;
            }
            return false;
        }

        public bool IsThanksMessage(string message)
        {
            Regex regex = new Regex(@"(!?)(obrigado|Obrigado|vlw|valeu)");
            if (regex.IsMatch(message))
            {
                return true;
            }
            return false;
        }

        public bool IsGreeting(string message)
        {
            Regex regex = new Regex(@"(!?)(bom dia|Bom dia|boa tarde|Boa tarde|boa noite|Boa noite)");
            if (regex.IsMatch(message))
            {
                return true;
            }
            return false;
        }


        public bool IsHello(string message)
        {
            Regex regex = new Regex(@"(!?)(ola|oi|opa|kole|e ai|koe)");
            if (regex.IsMatch(message))
            {
                return true;
            }
            return false;
        }

        public string ReturnGreetingByDateTime()
        {
            DateTime date = DateTime.Now;
            if (date.Hour > 5 && date.Hour < 12)
            {
                return "Bom dia, {0}! 😎 ☀️️";
            }
            else if (date.Hour > 12 && date.Hour < 18)
            {
                return "Boa tarde, {0}! 🌇";
            }
            else if (date.Hour > 18 )
            {
                return "Boa noite, {0}! 🌙";
            }
            else if (date.Hour > 0 && date.Hour < 5)
            {
                return "Boa noite, {0}! 🌙";
            }
            else
            {
                return "Ola, {0}! 😁";
            }
        }

        public string ReturnRandomHi()
        {
            string[] hellos = new string[] { "Ola, {0}!\n Tudo Bem?\n\n", "Oi Oi, {0}!\n Como vai?\n\n", "E ai , {0}! 😀\n\n" };
            return hellos[SingletonRandom.GetInstance().Next(0, 2)];
        }

        public string ReturnThanksMSG()
        {
            string[] hellos = new string[] { "Por nada!\nEspero ter ajudado.😉", "É sempre um prazer poder ajudar 😊", "Pode contar sempre comigo para tirar suas dúvidas sobre a Pratique 😎" };
            return hellos[SingletonRandom.GetInstance().Next(0, 2)];
        }

        public string ReturnByeMSG()
        {
            string[] hellos = new string[] { "Tchau Tchau!😀", "Até Breve.", "Até logo.\nEspero ter ajudado." };
            return hellos[SingletonRandom.GetInstance().Next(0, 2)];
        }
    }
}
