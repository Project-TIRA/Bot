using EntityModel;

namespace SearchBot.Bot.Models
{
    public class OrganizationScore
    {
        private const int REASONABLE_DISTANCE = 25;

        public Organization Organization { get; set; }
        public double ServiceScore { get; set; }
        public double Distance { get; set; }

        public bool AllServicesMatch { get { return this.ServiceScore == 1; } }
        public bool SomeServicesMatch { get { return this.ServiceScore > 0 && this.ServiceScore < 1; } }

        public static bool IsWithinDistance(double distance)
        {
            return distance <= REASONABLE_DISTANCE;
        }
    }
}
