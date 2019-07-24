namespace ServiceProviderBot.Bot.Models.LocationApi
{
    public class LocationApiResult
    {
        public const string PointAddressType = "Point Address";

        public string Type { get; set; }
        public float Score { get; set; }
        public LocationApiAddress Address { get; set; }
    }
}
