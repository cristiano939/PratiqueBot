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
    class SportsReceivers : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDirectoryExtension _directory;
        private CommomExpressionsManager _expression;
        private Settings _settings;
        private DocumentService _service;
        private readonly IBucketExtension _bucket;


        public SportsReceivers(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings)
        {
            _sender = sender;
            _directory = directory;
            _bucket = bucket;
            _settings = settings;
            _expression = new CommomExpressionsManager();
            _service = new DocumentService();

        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            string input = message.Content.ToString();


            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);

            if (input.Contains("#modalidades#"))
            {
                await _sender.SendMessageAsync("Em construção", message.From, cancellationToken);
                await CanIHelpYou(account, message.From, cancellationToken);

            }
        }


        public async Task CanIHelpYou(Account account, Node node, CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));

            Select select = new Select { Text = "Posso ajudar em mais alguma coisa?", Scope = SelectScope.Immediate, Options = new SelectOption[] { new SelectOption { Text = "Sim", Value = "#comecar#" }, new SelectOption { Text = "Não, obrigado", Value = "#encerrar#" } } };
            await _sender.SendMessageAsync(select, node, cancellationToken);
        }
    }
}