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
    class PackagesPricesReceivers : BaseReceiver, IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDirectoryExtension _directory;
        private CommomExpressionsManager _expression;
        private Settings _settings;
        private DocumentService _service;
        private readonly IBucketExtension _bucket;


        public PackagesPricesReceivers(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings) : base(sender, directory, bucket, settings)
        {
         

        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            string input = message.Content.ToString();


            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (await IsBotActive(message.From))
            {
                Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);

                if (input.Contains("#gold#"))
                {
                    await ShowGoldPackage(account, message.From, cancellationToken);

                }
                else if (input.Contains("#silver#"))
                {
                    await ShowSilverPackage(account, message.From, cancellationToken);

                }
                else if (input.Contains("#bronze#"))
                {
                    await ShowBronzePackage(account, message.From, cancellationToken);

                }
                else if (input.Contains("#discount#"))
                {
                    await ShowDiscountInfo(account, message.From, cancellationToken);
                }
                else
                {
                    await _sender.SendMessageAsync(Start(account), message.From, cancellationToken);
                }

            }
        }




        public Document Start(Account account)
        {
            string initialMessage = string.Format("{0},\nNossos planos e pacotes seguem as modalidades: Ouro , Prata e Bronze.\n\nQual deseja conhecer? ⬇️️", account.FullName.Split(' ')[0]);
            Select select = new Select
            {
                Text = initialMessage,
                Scope = SelectScope.Immediate,
                Options = new SelectOption[] {
                new SelectOption { Text = "Ouro", Value = "#gold#" },
                new SelectOption { Text = "Prata", Value = "#silver#" },
                new SelectOption { Text = "Bronze", Value = "#bronze#" },
                new SelectOption { Text = "Promoções", Value = "#discount#" }}
            };
            return select;
        }

        public async Task ShowBronzePackage(Account account, Node node, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync(new PlainText { Text = "O pacote bronze é uma boa escolha para quem não tem muito tempo para aproveitar todas as oportunidades que a Pratique oferece." }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "O Pacote Bronze da direito a:\n\n✅ Uma modalidade a sua escolha \n✅ Aulas de abdominal \n✅ Acesso ilimitados no dia e horários livres \n✅Treine com um amigo uma vez por semana" }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "Pelo preço de R$49,90 mensal" }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "Todos os nossos pacotes oferecem :\nAvaliação + Biometria + Manutenção\n\n por R$99,90" }, node, cancellationToken);
            await ShowBuyInfo(account, node, cancellationToken);
        }

        public async Task ShowSilverPackage(Account account, Node node, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync(new PlainText { Text = "O pacote prata te permite fazer mais atividades na academia, sendo mais interessante para quem quer entrar em forma e aprender alguma coisa nova. " }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "O Pacote Prata da direito a:\n\n✅ Duas modalidades a sua escolha \n✅ Aulas de abdominal \n✅ Acesso ilimitados no dia e horários livres \n✅Treine com um amigo uma vez por semana" }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "Pelo preço de R$89,90 mensal" }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "Todos os nossos pacotes oferecem :\nAvaliação + Biometria + Manutenção\n\n por R$99,90" }, node, cancellationToken);
            await ShowBuyInfo(account, node, cancellationToken);
        }

        public async Task ShowGoldPackage(Account account, Node node, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync(new PlainText { Text = "O pacote ouro é uma boa escolha para quem tem mais tempo para além de entrar em forma aprender alguma luta, reforçar a natação e varias outras opções. " }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "O Pacote Ouro da direito a:\n\n✅ Três modalidades a sua escolha \n✅ Aulas de abdominal \n✅ Acesso ilimitados no dia e horários livres \n✅Treine com um amigo uma vez por semana\n✅ Camiseta da academia" }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "Pelo preço de R$99,90 mensal" }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync(new PlainText { Text = "Todos os nossos pacotes oferecem :\nAvaliação + Biometria + Manutenção\n\n por R$99,90" }, node, cancellationToken);
            await ShowBuyInfo(account, node, cancellationToken);
        }


        public async Task ShowBuyInfo(Account account, Node node, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync(new PlainText { Text = "Faça a compra pelo nosso site, onde você pode escolher qual a unidade Pratique desejada." }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));

            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard
            {
                CardContent = "Vem entrar em forma com a gente!",
                CardMediaHeader =
                new MediaLink
                {
                    Text = "Vem entrar em forma com a gente!",
                    Uri = new Uri("https://s23.postimg.org/ofubvasor/logo.png"),
                    Title = "Seja nosso aluno!",
                    Type = new MediaType("image", "jpeg")
                },
                options = new List<CarrosselOptions>() {
                    new CarrosselOptions {
                        label = new WebLink {
                            Text ="",
                            Title ="Comprar",
                            Uri =new Uri("http://www.pratiquefitness.com.br/comprar.php") },
                        value = "" } }
            });

            cards.Add(new CarrosselCard
            {
                CardContent = "Já sabe qual a unidade mais próxima de voce?",
                CardMediaHeader =
          new MediaLink
          {
              Text = "Já sabe qual a unidade mais próxima de voce?",
              Uri = new Uri("https://maps.googleapis.com/maps/api/staticmap?center=-19.8838096,-43.9409045&markers=color:red%7Clabel:C%7C-19.8838096,-43.9409045&zoom=15&size=600x300&maptype=roadmap&key=AIzaSyAj0zH0MFBnL5oBpUt-SXeSgyCuoLi2caw"),
              Title = "Unidades",
              Type = new MediaType("image", "jpeg")
          },
                options = new List<CarrosselOptions>()
                {
                    new CarrosselOptions {
                        label = "Encontrar Unidade",
                        value = "#search#" }
                }
            });

            var carrossel = _service.CreateCarrossel(cards);
            await _sender.SendMessageAsync(carrossel, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);


        }


        public async Task ShowDiscountInfo(Account account, Node node, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync(new PlainText { Text = "As promoções podem variar de unidade para unidade, então é preferível que você confira as mesmas em nosso site." }, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));

            List<CarrosselCard> cards = new List<CarrosselCard>();
            cards.Add(new CarrosselCard
            {
                CardContent = "Podem ser o que faltava para sua vontade de estar aqui.",
                CardMediaHeader =
                new MediaLink
                {
                    Text = "Podem ser o que faltava para sua vontade de estar aqui.",
                    Uri = new Uri("https://s23.postimg.org/ofubvasor/logo.png"),
                    Title = "Fique de olho nas nossas promoções",
                    Type = new MediaType("image", "jpeg")
                },
                options = new List<CarrosselOptions>() {
                    new CarrosselOptions {
                        label = new WebLink {Text="",Title="Promoções",Uri=new Uri("http://www.pratiquefitness.com.br/promocoes/") },value = "" } }
            });

            var carrossel = _service.CreateCarrossel(cards);
            await _sender.SendMessageAsync(carrossel, node, cancellationToken);
            await CanIHelpYou(account, node, cancellationToken);
        }

      
    }
}