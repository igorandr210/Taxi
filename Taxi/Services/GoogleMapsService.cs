using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Taxi.Models.Location;

namespace Taxi.Services
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private GoogleApiOptions _googleOptions;

        private const string BaseQueryUrl = "https://maps.googleapis.com/maps/api/directions/json";

      
        public GoogleMapsService(IOptions<GoogleApiOptions> googleOptions)
        {
            _googleOptions = googleOptions.Value;
        }

        public async Task<DirectionsData> GetDirections(double latFrom, double lonFrom, double latTo, double lonTo)
        {
            var queryParams = $"?origin={latFrom},{lonFrom}&destination={latTo},{lonTo}&mode=driving";
            if (_googleOptions.ApiKey != null)
            {
                queryParams += $"&key={_googleOptions.ApiKey}";

            }
            DirectionsData responce = null;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(BaseQueryUrl+queryParams))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();

                responce = JsonConvert.DeserializeObject<DirectionsData>(data);
            }
            return responce;
        }
    }
}
