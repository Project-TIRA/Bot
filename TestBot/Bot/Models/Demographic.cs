using System;
namespace TestBot.Bot.Models
{
    public class Demographic
    {
        public Gender Gender { get; set; }
        public AgeRange AgeRange { get; set; }

        public Demographic()
        {
            this.Gender = Gender.Unknown;
            this.AgeRange = new AgeRange();
        }

        public void SetToAll()
        {
            this.Gender = Gender.All;
            this.AgeRange.SetToAll();
        }

        public override string ToString()
        {
            return $"Gender: {this.Gender}, Age Range: {this.AgeRange}";
        }
    }
}
