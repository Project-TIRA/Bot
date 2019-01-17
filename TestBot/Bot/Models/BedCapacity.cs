namespace TestBot.Bot.Models
{
    public struct BedCapacity
    {
        public int BedsCount;
        public int BedsOpenCount;

        public override string ToString()
        {
            return $"{this.BedsOpenCount} of {this.BedsCount} beds available";
        }
    }
}
