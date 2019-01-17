namespace TestBot.Bot.Models
{
    public class OrganizationProfile
    {
        public string Name { get; set; }

        public Demographic Demographic { get; set; }

        public BedCapacity BedCapacity { get; set; }

        public OrganizationProfile()
        {
            this.Demographic = new Demographic();
            this.BedCapacity = new BedCapacity();
        }

        public override string ToString()
        {
            return $"Name: ({this.Name}), {this.Demographic.ToString()}";
        }
    }
}