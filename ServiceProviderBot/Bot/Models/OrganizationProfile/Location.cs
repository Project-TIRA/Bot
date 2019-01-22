namespace ServiceProviderBot.Bot.Models.OrganizationProfile
{
    public class Location
    {
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public override string ToString()
        {
            return $"{this.City}, {this.State} {this.Zip}";
        }
    }
}
