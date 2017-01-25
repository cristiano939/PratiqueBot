using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PratiqueBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PratiqueBot.Clients
{
    public class GMapsClient
    {

        private string GeoCodeBaseAddress = "https://maps.googleapis.com/maps";
        private string GeoCodeApiKey = "AIzaSyCkezlXx4pwBKaPE0ffdadiMgojkn8q7zQ";
        private string GMapsDirApiKey = "AIzaSyBZyyOKYUz-OSQx02ugUskjHTDKRrpDP5U";
        private string GMapsDistMatrixApiKey = "AIzaSyAdigTfmPuj9g5rJizfT-hBoaDSukVPvaE";
        private string GeoCodeAddressPattern = "/api/geocode/json?address={0}";
        private string DistancePattern = "/api/distancematrix/json?origins={0}&destinations={1}&mode={2}&key={3}";


        private static HttpClient GetClient(string apiToken, Uri baseAddress)
        {
            var client = new HttpClient();

            client.BaseAddress = baseAddress;
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Token " + apiToken);

            return client;
        }

        public async Task<JObject> GetAddressData(string address)
        {
            using (var client = GetClient(GeoCodeApiKey, new Uri(GeoCodeBaseAddress)))
            {
                var taskResult = await client.GetAsync(client.BaseAddress + string.Format(GeoCodeAddressPattern, address));
                JObject jobject = JObject.Parse(await taskResult.Content.ReadAsStringAsync());

                return jobject;
            }
        }

        public async Task<GDistanceData> GetDistanceData(string oLat, string oLng,string dLat,string dLng)
        {
            using (var client = GetClient(GeoCodeApiKey, new Uri(GeoCodeBaseAddress)))
            {
                var taskResult = await client.GetAsync(client.BaseAddress + string.Format(DistancePattern, oLat+"," +oLng,dLat+","+dLng, "driving", GMapsDistMatrixApiKey));
                var GDistData = JsonConvert.DeserializeObject<GDistanceData>(taskResult.Content.ReadAsStringAsync().Result);

                return GDistData;
            }



        }
    }
}
