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

namespace PratiqueBot
{
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDirectoryExtension _directory;
        private CommomExpressionsManager _expression;
        private DocumentService _service;


        public PlainTextMessageReceiver(IMessagingHubSender sender, IDirectoryExtension directory)
        {
            _sender = sender;
            _directory = directory;
            _expression = new CommomExpressionsManager();
            _service = new DocumentService();

        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            string input = message.Content.ToString();


            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);

            if (_expression.IsFirstMessage(input))
            {
                await _sender.SendMessageAsync(GreetingsFirstMessage(account, input), message.From, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 4));
                await _sender.SendMessageAsync(Start(account), message.From, cancellationToken);

            }
            else if (input.Contains("#comofunciona#"))
            {
                await _sender.SendMessageAsync(CreateDoubtsCarrossel(), message.From, cancellationToken);
            }
            else if (input.Contains("#comecar#"))
            {
                await _sender.SendMessageAsync(FirstMenu(account), message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(Start(account), message.From, cancellationToken);
            }


        }

        public string GreetingsFirstMessage(Account account, string message)
        {

            if (_expression.IsGreeting(message))
            {
                return string.Format(_expression.ReturnGreetingByDateTime(), account.FullName);
            }
            else if (_expression.IsHello(message))
            {
                return string.Format(_expression.ReturnRandomHi(), account.FullName);
            }
            return string.Format("Ola,{0}!!!", account.FullName);
        }


        public Document Start(Account account)
        {
            string initialMessage = string.Format("{0},\n\nsou o assistente virtual da academia Pratique Fitness!\nEstou aqui para lhe mostrar nossos serviços, planos e encontrar a unidade mais proxima!", account.FullName.Split(' ')[0]);
            Select select = new Select
            {
                Text = initialMessage,
                Scope = SelectScope.Immediate,
                Options = new SelectOption[] {
                new SelectOption { Text = "Começar", Value = "#comecar#" },
                new SelectOption { Text = "Como funciona?", Value = "#comofunciona#" } }
            };
            return select;
        }

        public Document FirstMenu(Account account)
        {
            string initialMessage = string.Format("Sobre o que deseja saber?", account.FullName.Split(' ')[0]);
            Select select = new Select
            {
                Text = initialMessage,
                Scope = SelectScope.Immediate,
                Options = new SelectOption[] {
                new SelectOption { Text = "Modalidades", Value = "#modalidades#" },
                new SelectOption { Text = "Unidades", Value = "#unidades#" },
                new SelectOption { Text = "Rede Saude", Value = "#redesaude#" },
                new SelectOption { Text = "Planos e Pacotes", Value = "#planospacotes#" },
                new SelectOption { Text = "Atendimento", Value = "#atendente#" }}
            };
            return select;
        }

        public Document CreateDoubtsCarrossel()
        {
            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard { CardContent = "Conheça a melhor rede de academias de BH. Deslize para ⬅️️ esquerda", CardMediaHeader = new MediaLink { Text = "Conheça a melhor rede de academias de BH. Deslize para ⬅️️ esquerda", Uri = new Uri("https://s23.postimg.org/ofubvasor/logo.png"), Title = "Pratique Fitness Academia", Type = new MediaType("image", "jpeg") }, options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardContent = "Todas as unidades possuem um grupo de esportes e modalidades diferenciadas.", CardMediaHeader = new MediaLink { Text = "Todas as unidades possuem um grupo de esportes e modalidades diferenciadas.", Uri = new Uri("https://s27.postimg.org/n6zs51qub/modalidades.png"), Title = "Modalidades", Type = new MediaType("image", "jpeg") }, options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardContent = "Encontre a unidade mais proxima de onde estiver. Basta enviar sua localização", CardMediaHeader = new MediaLink { Text = "Encontre a unidade mais proxima de onde estiver. Basta enviar sua localização", Uri = new Uri("https://maps.googleapis.com/maps/api/staticmap?center=-19.8838096,-43.9409045&markers=color:red%7Clabel:C%7C-19.8838096,-43.9409045&zoom=15&size=600x300&maptype=roadmap&key=AIzaSyAj0zH0MFBnL5oBpUt-SXeSgyCuoLi2caw"), Title = "Encontre a unidade mais próxima", Type = new MediaType("image", "jpeg") }, options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardContent = "Indique amigos e familiares e aumente sua Rede Saúde. Ganhe créditos e dinheiro", CardMediaHeader = new MediaLink { Text = "Indique amigos e familiares e aumente sua Rede Saúde. Ganhe créditos e dinheiro", Uri = new Uri("https://s23.postimg.org/p67gvh9wr/rede_saude.png"), Title = "Rede Saúde", Type = new MediaType("image", "jpeg") }, options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardContent = "Veja nossos planos e escolha o melhor para seu tempo, corpo e bolso.", CardMediaHeader = new MediaLink { Text = "Veja nossos planos e escolha o melhor para seu tempo, corpo e bolso.", Uri = new Uri("https://s23.postimg.org/8nuvygvy3/pacotes.png"), Title = "Pacotes e Preços", Type = new MediaType("image", "jpeg") }, options = new List<CarrosselOptions>() });
            cards.Add(new CarrosselCard { CardContent = "Podemos começar, ou quer falar com um atendente?", CardMediaHeader = new MediaLink { Text = "Podemos começar, ou quer falar com um atendente?", Uri = new Uri("https://s23.postimg.org/qetna6huz/pratique.jpg"), Title = "Começar", Type = new MediaType("image", "jpeg") }, options = new List<CarrosselOptions>() { new CarrosselOptions { label = "Começar", value = "#comecar#" }, new CarrosselOptions { label = "Atendente", value = "#atendente#" } } });
            Document carrossel = _service.CreateCarrossel(cards);
            return carrossel;
        }
    }
}
