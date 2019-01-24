﻿using System.Linq;

namespace ServiceProviderBot.Bot.Models.LocationApi
{
    public class LocationApiResponse
    {
        public LocationApiResult[] Results { get; set; }

        public LocationApiAddress GetTopStreetResult(string expectedZip)
        {
            return this.Results.FirstOrDefault(r => r.Type == LocationApiResult.StreetType &&
                                                    r.Address.PostalCode == expectedZip).Address;
        }
    }
}
