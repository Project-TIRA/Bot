namespace TestBot.Bot
{
    public class OrganizationProfile
    {
        public string Name { get; set; }

        public int Size { get; set; }

        public int ConversationIndex { get; set; }

        public override string ToString()
        {
            return $"Name: {this.Name}, Size: {this.Size}";
        }
    }
}