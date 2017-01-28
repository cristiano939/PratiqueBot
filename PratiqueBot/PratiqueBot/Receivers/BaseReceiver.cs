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
using Takenet.MessagingHub.Client.Extensions.EventTracker;

namespace PratiqueBot.Receivers
{
    public abstract class BaseReceiver
    {
        public readonly IMessagingHubSender _sender;
        public readonly IDirectoryExtension _directory;
        public CommomExpressionsManager _expression;
        public readonly IBucketExtension _bucket;
        public readonly IEventTrackExtension _track;
        public Settings _settings;
        public DocumentService _service;
        


        public BaseReceiver(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings, IEventTrackExtension track)
        {
            _sender = sender;
            _directory = directory;
            _bucket = bucket;
            _settings = settings;
            _track = track;
            _expression = new CommomExpressionsManager();
            _service = new DocumentService();

        }


        public async Task CanIHelpYou(Account account, Node node, CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));

            Select select = new Select { Text = "Posso ajudar em mais alguma coisa?", Scope = SelectScope.Immediate, Options = new SelectOption[] { new SelectOption { Text = "Sim", Value = "#comecar#" }, new SelectOption { Text = "Não, obrigado", Value = "#encerrar#" } } };
            await _sender.SendMessageAsync(select, node, cancellationToken);
        }

        public async Task<bool> IsBotActive(Node from)
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
