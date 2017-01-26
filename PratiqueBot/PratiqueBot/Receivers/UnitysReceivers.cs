using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using Takenet.MessagingHub.Client.Extensions.Directory;
using Lime.Messaging.Resources;
using PratiqueBot.ContentResolver;
using Lime.Messaging.Contents;
using PratiqueBot.Factory;
using PratiqueBot.Models;
using System.Collections.Generic;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using System.Linq;

namespace PratiqueBot.Receivers
{
    class UnitysReceivers : BaseReceiver, IMessageReceiver
    {


        public UnitysReceivers(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings) : base(sender, directory, bucket, settings)
        {
         

        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            string input = message.Content.ToString();


            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (await IsBotActive(message.From))
            {
                Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);

                if (input.Contains("#unidades#"))
                {
                    await _sender.SendMessageAsync(Start(account, _settings.Gyms), message.From, cancellationToken);

                }
                else if (input.Contains("#vertodas#"))
                {
                    await ShowAllGyms(account, message.From, _settings.Gyms, cancellationToken);
                }
                else if (input.Contains("#unidmodalids#"))
                {
                    await ShowAllModalities(account, message.From, _settings.Gyms, input, cancellationToken);
                }
                else if (input.Contains("#searchnearest#"))
                {
                    await SearchNearest(account, message.From, cancellationToken);
                }
                else
                {
                    await _sender.SendMessageAsync(Start(account, _settings.Gyms), message.From, cancellationToken);
                }
            }
        }



        public Document Start(Account account, List<Gym> gyms)
        {
            string firstMessage = string.Format("Possuimos um total de {0} unidades.\nPor aqui posso te ajudar a\n\n " +
                                                "✅Encontrar a Pratique mais perto de você.\n\n" +
                                                "✅Descobrir quais modalidades estão em cada unidade\n\n" +
                                                "✅Contato e rotas de todas as academias\n\n\nO que vai ser? ⬇️️",
                                                gyms.Count);


            Select select = new Select
            {
                Text = firstMessage,
                Scope = SelectScope.Immediate,
                Options = new SelectOption[] {
                    new SelectOption {Text="Ver Todas",Value="#vertodas#" },
                    new SelectOption {Text="Ver Modalidades", Value= "#unidmodalids#" },
                    new SelectOption {Text="Ver a mais perto",Value="#searchnearest#" }
                }
            };

            return select;
        }

        public async Task SearchNearest(Account account, Node node, CancellationToken cancellationToken)
        {
            string text = string.Format("{0}, envie para mim seu endereço.\nSiga o exemplo:\n\nAv. Afonso Pena 1000 Centro BH", account.FullName.Split(' ')[0]);
            PlainText simpleMsg = new PlainText { Text = text };
            await _sender.SendMessageAsync(simpleMsg, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync("Você pode tambem enviar a localização pelo seu celular, veja a imagem de como fazer 😉", node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            MediaLink tutorialMedia = new MediaLink { Type = new MediaType("image", "jpeg"), Uri = new Uri("https://s23.postimg.org/u7vrcpshn/usar_localiza_o.jpg") };
            await _sender.SendMessageAsync(tutorialMedia, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            PlainText finalMsg = new PlainText { Text = "Você pode fazer isso a qualquer momento da nossa conversa, que lhe ajudarei a encontrar a unidade mais proxima." };
            await _sender.SendMessageAsync(finalMsg, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);
        }

        public async Task ShowAllModalities(Account account, Node node, List<Gym> gyms, string input, CancellationToken cancellationToken)
        {
            input = input.Replace("#unidmodalids#", "");
            if (input.Length > 3)
            {
                await ShowGymModalities(account, node, gyms, input, cancellationToken);
            }
            else
            {
                await ShowAllGymsSelect(account, node, gyms, cancellationToken);
            }

        }

        public async Task ShowGymModalities(Account account, Node node, List<Gym> gyms, string input, CancellationToken cancellationToken)
        {
            Gym gym = (from gymm in gyms where gymm.Name == input select gymm).FirstOrDefault();
            string simpleMsg = string.Format("As modalidades da unidade {0} são:\n", gym.Name);
            foreach (string modalities in gym.Modalities)
            {
                simpleMsg = simpleMsg + "\n✅" + modalities;
            }
            await _sender.SendMessageAsync(simpleMsg, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);
        }

        public async Task ShowAllGymsSelect(Account account, Node node, List<Gym> gyms, CancellationToken cancellationToken)
        {
            SelectOption[] options = new SelectOption[gyms.Count];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = new SelectOption { Text = gyms[i].Name, Value = "#unidmodalids#" + gyms[i].Name };
            }
            Select select = new Select { Scope = SelectScope.Immediate, Text = "Escolha de qual unidade você deseja saber as modalidades: ⬇️️", Options = options };
            await _sender.SendMessageAsync(select, node, cancellationToken);
        }




        public async Task ShowAllGyms(Account account, Node node, List<Gym> gyms, CancellationToken cancellationToken)
        {
            List<CarrosselCard> cards = new List<CarrosselCard>();
            foreach (Gym gym in gyms)
            {
                cards.Add(
                 new CarrosselCard
                 {
                     CardContent = string.Format("📍:{0} ☎️️: {1}", gym.Address, gym.Phone),
                     CardMediaHeader = new MediaLink
                     {
                         Text = string.Format("📍:{0} ☎️️: {1}", gym.Address, gym.Phone),
                         Uri = new Uri(string.Format("https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&markers=color:red%7Clabel:C%7C{0},{1}&zoom=15&size=600x300&maptype=roadmap&key=AIzaSyAj0zH0MFBnL5oBpUt-SXeSgyCuoLi2caw", gym.latitude, gym.longitude)),
                         Title = gym.Name,
                         Type = new MediaType("image", "jpeg")
                     },
                     options = new List<CarrosselOptions>() {
                        new CarrosselOptions {
                            label = new WebLink {
                                Title = "Ligar Agora",
                                Uri = new Uri(string.Format("tel:+55{0}", gym.Phone).Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))
                            },
                            value = "" },
                        new CarrosselOptions {
                         label= new WebLink
                            {
                                Title = "Traçar Rota",
                                Uri = new Uri(string.Format("https://www.google.com.br/maps/dir/{0},{1}",gym.latitude,gym.longitude))
                            }
                         ,value = ""
                        }
                     }
                 });
            }
            var carrossel = _service.CreateCarrossel(cards);
            await _sender.SendMessageAsync(carrossel, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);
        }
    }
}