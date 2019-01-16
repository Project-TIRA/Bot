using System;
namespace TestBot.Bot.Models
{
    [Flags]
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        // Todo
    }

    public class Demographic
    {
        public Gender Gender { get; set; }
        public AgeRange AgeRange { get; set; }

        public Demographic()
        {
            this.Gender = Gender.Unknown;
            this.AgeRange = new AgeRange { Start = -1, End = -1 };
        }
    }
}
