using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using Shared.ApiInterface;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateJobTrainingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<JobTrainingData>(this.api, organization.Id);
            var data = await TestHelpers.CreateJobTrainingData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateJobTrainingDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.JobTraining.ServiceName))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<JobTrainingData>(this.turnContext, true);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.Open);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<JobTrainingData>(this.api, organization.Id);
            var data = await TestHelpers.CreateJobTrainingData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateJobTrainingDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.JobTraining.ServiceName))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.JobTraining.ServiceName)))
                .Send(TestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<JobTrainingData>(this.turnContext, true);
            Assert.Equal(0, resultData.Open);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.WaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<JobTrainingData>(this.api, organization.Id);
            var data = await TestHelpers.CreateJobTrainingData(this.api, user.Id, service.Id, true, false, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateJobTrainingDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.JobTraining.ServiceName))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<JobTrainingData>(this.turnContext, true);
            Assert.Equal(0, resultData.Open);
            Assert.False(resultData.WaitlistIsOpen);
        }
    }
}
