using System.Linq;

namespace TestBot.Bot.Models.LocationApi
{
    public class LocationApiResponse
    {
        public LocationApiResult[] Results { get; set; }

        public LocationApiAddress GetTopStreetResult()
        {
            return this.Results.FirstOrDefault(r => r.Type == LocationApiResult.StreetType).Address;
        }
    }
}
