namespace TestBot.Bot
{
    public class OrganizationProfile
    {
        public string Name { get; set; }

        public int Size { get; set; }

        public int AgeRangeStart { get; set; }

        public int AgeRangeEnd { get; set; }

        public override string ToString()
        {
            return $"Name: {this.Name}," +
            	$"Size: {this.Size}" +
                $"Age Range: {this.AgeRangeStart}-{this.AgeRangeEnd}";
        }
    }
}