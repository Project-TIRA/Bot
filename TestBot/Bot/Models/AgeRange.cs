namespace TestBot.Bot.Models
{
    public struct AgeRange
    {
        public int Start;
        public int End;

        public override string ToString()
        {
            if (this.Start == -1 && this.End == -1)
            {
                return "All";
            }

            return $"{this.Start} to {this.End}";
        }
    }
}
