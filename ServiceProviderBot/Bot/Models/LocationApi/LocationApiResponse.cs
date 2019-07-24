using System.Linq;

namespace ServiceProviderBot.Bot.Models.LocationApi
{
    public class LocationApiResponse
    {
        public LocationApiResult[] Results { get; set; }

        public LocationApiAddress GetTopResult()
        {
            return this.Results
                .Where(r => r.Type == LocationApiResult.PointAddressType)
                .OrderByDescending(r => r.Score)
                .FirstOrDefault()
                .Address;
        }
    }
}
