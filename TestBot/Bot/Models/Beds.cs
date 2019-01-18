namespace TestBot.Bot.Models
{
    public class Beds
    {
        public int Total;
        public int Open;

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
