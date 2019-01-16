namespace TestBot.Bot.Models
{
    public class OrganizationProfile
    {
        public string Name { get; set; }

        public Demographic Demographic { get; set; }

        public override string ToString()
        {
            return $"Name: {this.Name}, " +
            	$"Demographic: {this.Demographic}";
        }
    }
}