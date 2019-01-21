using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using TestBot.Bot.Dialogs;
using TestBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NewOrganization()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("begin", StartsWith(Phrases.Greeting.GetAction))
                .Test("new", Phrases.NewOrganization.GetName)
                .StartTestAsync();
        }

        [Fact]
        public async Task UpdateOrganization()
        {
            /*
            await CreateTestFlow(MasterDialog.Name)
                .Test("hello", StartsWith(Phrases.Greeting.GetAction))
                .Test("update", "TODO")
                .StartTestAsync();
            */               
        }

        [Fact]
        public async Task NewOrganizationNoToAll()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("begin", StartsWith(Phrases.Greeting.GetAction))
                .Test("new", Phrases.NewOrganization.GetName)
                .Test("test org", StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.Greeting.GetClosing)
                .StartTestAsync();
        }

        /*
        [Fact]
        public async Task UpdateOrganizationNoToAll()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("hello", StartsWith(Phrases.Greeting.GetAction))
                .Test("update", "TODO")
                .StartTestAsync();
        }
        */
    }
}
