namespace ServiceProviderBot.Bot.Models.OrganizationProfile
{
    public class Beds
    {
        public int Total { get; set; }
        public int Open { get; set; }

        public override string ToString()
        {
            if (this.Total == 0)
            {
                return "Not offered";
            }

            return $"{this.Open} of {this.Total} available";
        }

        public void SetToNone()
        {
            this.Total = 0;
            this.Open = 0;
        }
    }
}
