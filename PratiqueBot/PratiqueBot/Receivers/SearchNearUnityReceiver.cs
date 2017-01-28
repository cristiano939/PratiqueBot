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
using Lime.Messaging.Contents;
using PratiqueBot.Factory;
using PratiqueBot.Models;
using System.Collections.Generic;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using PratiqueBot.Clients;
using Newtonsoft.Json.Linq;
using System.Linq;
using Takenet.MessagingHub.Client.Extensions.EventTracker;

namespace PratiqueBot.Receivers
{
    class SearchNearUnityReceiver : BaseReceiver, IMessageReceiver
    {
     
        private GMapsClient _gclient;



        public SearchNearUnityReceiver(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings, IEventTrackExtension track) : base(sender, directory, bucket, settings,track)
        {
         
            _gclient = new GMapsClient();

        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {

            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (await IsBotActive(message.From))
            {
                Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);
                string lat, lng;
                if (message.Content.GetType() == typeof(Location))
                {
                    Location location = (Location)message.Content;
                    lat = location.Latitude.Value.ToString().Replace(",", ".");
                    lng = location.Longitude.Value.ToString().Replace(",", ".");
                }
                else
                {
                    string input = message.Content.ToString();
                    JObject result = await _gclient.GetAddressData(input);
                    lat = result["results"][0]["geometry"]["location"]["lat"].Value<string>().Replace(",", ".");
                    lng = result["results"][0]["geometry"]["location"]["lng"].Value<string>().Replace(",", ".");
                }

                Gym gym = await SendNearestGym(lat, lng, _settings.Gyms);

                await _track.AddAsync("Unidades Proximas", gym.Name);
                List<CarrosselCard> cards = new List<CarrosselCard>();
                cards.Add(
                    new CarrosselCard
                    {
                        CardContent = "Encontramos a Pratique mais próxima de você. 😎 Deslize para ⬅️️ esquerda",
                        CardMediaHeader = new MediaLink
                        {
                            Text = "Encontramos a Pratique mais próxima de você. 😎 Deslize para ⬅️️ esquerda",
                            Uri = new Uri("https://s23.postimg.org/ofubvasor/logo.png"),
                            Title = "Pratique Fitness Academia",
                            Type = new MediaType("image", "jpeg")
                        },
                        options = new List<CarrosselOptions>()
                    });

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
                                Uri = new Uri(string.Format("https://www.google.com.br/maps/dir/{0},{1}/{2},{3}/",lat,lng,gym.latitude,gym.longitude))
                            }
                         ,value = ""
                        }
                        }
                    });

                await _sender.SendMessageAsync(_service.CreateCarrossel(cards), message.From, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 10));
                await CanIHelpYou(account, message.From, cancellationToken);

            }
        }




        public async Task<Gym> SendNearestGym(string lat, string lng, List<Gym> gyms)
        {

            List<GDistanceData> distances = new List<GDistanceData>();
            foreach (Gym gym in gyms)
            {
                var gDistData = await _gclient.GetDistanceData(lat, lng, gym.latitude, gym.longitude);
                distances.Add(gDistData);
            }
            var nearGym = gyms[GetNearestIndex(distances)];
            nearGym.distance = (from dist in distances orderby dist.rows[0].elements[0].distance.value ascending select dist.rows[0].elements[0].distance.text).FirstOrDefault();
            return nearGym;
        }

        public int GetNearestIndex(List<GDistanceData> distances)
        {
            var min = distances.Min(a => a.rows[0].elements[0].distance.value);
            var distance = from dist in distances where dist.rows[0].elements[0].distance.value == min select dist;
            return distances.IndexOf(distance.FirstOrDefault());
        }


     
    }
}