namespace TestBot.Bot.Models.LocationApi
{
    public class LocationApiResult
    {
        public const string StreetType = "Street";

        public string Type { get; set; }
        public LocationApiAddress Address { get; set; }
    }
}
