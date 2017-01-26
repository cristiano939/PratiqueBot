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

namespace PratiqueBot.Receivers
{
    class HealthNetworkReceiver : BaseReceiver, IMessageReceiver
    {


        public HealthNetworkReceiver(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings) : base(sender, directory, bucket, settings)
        {


        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            string input = message.Content.ToString();


            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (await IsBotActive(message.From))
            {
                Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);

                if (input.Contains("#redesaude#"))
                {
                    await _sender.SendMessageAsync(Start(account), message.From, cancellationToken);
                }
                else if (input.Contains("#saudefunciona#"))
                {
                    await HowHealthWorks(account, message.From, cancellationToken);

                }
                else if (input.Contains("#saudesite#"))
                {
                    await ShowWebSite(account, message.From, cancellationToken);
                }
                else
                {
                    await _sender.SendMessageAsync(Start(account), message.From, cancellationToken);
                }
            }
        }

        public async Task ShowWebSite(Account account, Node node, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync("Em nosso site você consegue se cadastrar e ver mais informações e regulamentações!", node, cancellationToken);
            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard
            {
                CardContent = "Indique amigos e familiares e aumente sua Rede Saúde. Ganhe créditos e dinheiro",
                CardMediaHeader = new MediaLink
                {
                    Text = "Indique amigos e familiares e aumente sua Rede Saúde. Ganhe créditos e dinheiro",
                    Uri = new Uri("https://s23.postimg.org/p67gvh9wr/rede_saude.png"),
                    Title = "Rede Saúde",
                    Type = new MediaType("image", "jpeg")
                },
                options = new List<CarrosselOptions> {
                                new CarrosselOptions {
                                    label = new WebLink {Title="Ver o Site",
                                            Uri = new Uri("http://www.pratiquefitness.com.br/redesaude/") },value="" } }
            });
            var carrossel = _service.CreateCarrossel(cards);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(carrossel, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);
        }

        public Document Start(Account account)
        {
            string text = "Praticar exercícios é muito bom para o corpo e mente, mas melhor ainda é praticar exercícios com amigos, não é mesmo? \n\nMelhor ainda é ganhar dinheiro por ter seus amigos com você.\nIsso é a Rede Saúde. Você convida seus amigos e ganha créditos na academia Pratique.";
            SelectOption[] options = new SelectOption[]
            {
                new SelectOption {
                    Text = "Como funciona?",
                    Value = "#saudefunciona#"
                },

                new SelectOption {
                     Text = "Ver o Site",
                    Value = "#saudesite#"

                },

                new SelectOption {
                    Text = "Chamar Atendente",
                    Value = "#atendente#"
                }
            };
            Select select = new Select { Text = text, Options = options, Scope = SelectScope.Immediate };
            return select;
        }

        public async Task HowHealthWorks(Account account, Node node, CancellationToken cancellationToken)
        {
            List<CarrosselCard> cards = new List<CarrosselCard>();

            await _sender.SendMessageAsync("Melhor do que te contar, vou te mostrar um vídeo que explica melhor, pode ser?", node, cancellationToken);
            MediaLink media = new MediaLink { Type = new MediaType("video", "mp4"), Uri = new Uri("http://www.pratiquefitness.com.br/wp-content/uploads/2017/01/Rede-saude-para-Zap.mp4?_=1") };
            await _sender.SendMessageAsync(media, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 15));
            await _sender.SendMessageAsync("E ai? Interessou? Então veja nosso site e entre nessa! \n\n\nQuer uma dica? Use sua vantagem de poder levar um amigo diferente por semana, para apresentar a academia e aumentar sua rede! \nVantagem disponível nos pacotes Ouro, Prata e Bronze. 😎", node, cancellationToken);

            cards.Add(new CarrosselCard
            {
                CardContent = "Indique amigos e familiares e aumente sua Rede Saúde. Ganhe créditos e dinheiro",
                CardMediaHeader = new MediaLink
                {
                    Text = "Indique amigos e familiares e aumente sua Rede Saúde. Ganhe créditos e dinheiro",
                    Uri = new Uri("https://s23.postimg.org/p67gvh9wr/rede_saude.png"),
                    Title = "Rede Saúde",
                    Type = new MediaType("image", "jpeg")
                },
                options = new List<CarrosselOptions> {
                                new CarrosselOptions {
                                    label = new WebLink {Title="Ver o Site",
                                            Uri = new Uri("http://www.pratiquefitness.com.br/redesaude/") },value=""
                                }
                }
            });
            var carrossel = _service.CreateCarrossel(cards);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(carrossel, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);


        }


    }
}