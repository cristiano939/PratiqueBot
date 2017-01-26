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
        private readonly IMessagingHubSender _sender;
        private readonly IDirectoryExtension _directory;
        private CommomExpressionsManager _expression;
        private Settings _settings;
        private DocumentService _service;
        private readonly IBucketExtension _bucket;


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
                    await _sender.SendMessageAsync("Em construção", message.From, cancellationToken);
                    await CanIHelpYou(account, message.From, cancellationToken);

                } 
            }
        }

        private async Task<bool> IsBotActive(Node from)
        {
            try
            {
                var data = await _bucket.GetAsync<JsonDocument>(from.ToString() + _settings.BotIdentifier + "_suspended");

                if (data != null)
                {
                    return false;
                }
                else return true;

            }
            catch
            {
                return true;
            }

        }


       
    }
}