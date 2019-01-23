﻿namespace ServiceProviderBot.Bot.Models.OrganizationProfile
{
    public class AgeRange
    {
        public const int Default = -1;

        public int Start { get; set; }
        public int End { get; set; }

        public AgeRange()
        {
            SetToAll();
        }

        public override string ToString()
        {
            if (this.Start == Default && this.End == Default)
            {
                return "All";
            }

            return $"{this.Start} to {this.End}";
        }

        public void SetToAll()
        {
            this.Start = Default;
            this.End = Default;
        }
    }
}