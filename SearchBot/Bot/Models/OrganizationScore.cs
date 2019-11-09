using EntityModel;

namespace SearchBot.Bot.Models
{
    public class OrganizationScore
    {
        private const int REASONABLE_DISTANCE = 25;

        public Organization Organization { get; set; }
        public float ServiceScore { get; set; }
        public float Distance { get; set; }

        public bool AllServicesMatch { get { return this.ServiceScore == 1; } }
        public bool IsWithinDistance {  get { return this.Distance <= REASONABLE_DISTANCE;  } }
    }
}
