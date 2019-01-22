using System;

namespace ServiceProviderBot.Bot.Models.OrganizationProfile
{
    public class OrganizationProfile
    {
        public string Name { get; set; }

        public Location Location { get; set; }

        public Demographic Demographic { get; set; }

        public Capacity Capacity { get; set; }

        public OrganizationProfile()
        {
            this.Location = new Location();
            this.Demographic = new Demographic();
            this.Capacity = new Capacity();
        }

        public override string ToString()
        {
            return $"{nameof(OrganizationProfile)}:" + Environment.NewLine +
                $"  {nameof(Name)}: [ {this.Name} ]" + Environment.NewLine +
                $"  {nameof(Location)}: [ {this.Location} ]" + Environment.NewLine +
                $"  {nameof(Demographic)}: [ {this.Demographic} ]" + Environment.NewLine +
                $"  {nameof(Capacity)}: [ {this.Capacity} ]";
        }
    }
}