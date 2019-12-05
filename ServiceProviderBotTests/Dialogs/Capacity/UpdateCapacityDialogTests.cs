using EntityModel;
using ServiceProviderBot.Bot.Dialogs.Capacity;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ServiceProviderBotTests.Dialogs.Capacity
{
    public class UpdateCapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateSingle()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { new EmploymentData() });
            
        }

        [Fact]
        public async Task UpdateAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetSubtypes<ServiceData>());
        }

        [Fact]
        public async Task WaitlistClosedSingle()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            foreach (var type in Helpers.GetSubtypes<ServiceData>())
            {
                await RunTest(user, organization, new List<ServiceData>() { type }, testWaitlist: true);
            }
        }

        [Fact]
        public async Task WaitlistClosedAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetSubtypes<ServiceData>(), testWaitlist: true);
        }

        [Fact]
        public async Task WaitlistOpenSingle()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            foreach (var type in Helpers.GetSubtypes<ServiceData>())
            {
                await RunTest(user, organization, new List<ServiceData>() { type }, testWaitlist: true, testWaitlistOpen: true);
            }
        }

        [Fact]
        public async Task WaitlistOpenAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetSubtypes<ServiceData>(), testWaitlist: true, testWaitlistOpen: true);
        }

        private async Task RunTest(User user, Organization organization, List<ServiceData> types, bool testWaitlist = false, bool testWaitlistOpen = false)
        {
            // Create the services and data.
            // Must be done before starting the test flow.
            foreach (var type in types)
            {
                var service = await TestHelpers.CreateService(this.api, organization.Id, type.ServiceType());
                var data = await TestHelpers.CreateServiceData(this.api, user.Id, service.Id, type, testWaitlist);
            }

            // Prepare the test flow.
            var testFlow = CreateTestFlow(UpdateCapacityDialog.Name, user)
                .Send("test");

            if (types.Count > 1)
            {
                testFlow.AssertReply(StartsWith(Phrases.Update.Options));
                testFlow.Send(Phrases.Services.All);
            }

            // Add each step for the service types to the test flow.
            foreach (var type in types)
            {
                foreach (var step in type.UpdateSteps())
                {
                    testFlow.AssertReply(Phrases.Capacity.GetOpenings(step.Name));

                    if (testWaitlist)
                    {
                        testFlow.Send("0");

                        if (testWaitlistOpen)
                        {
                            testFlow.AssertReply(StartsWith(Phrases.Capacity.GetWaitlistIsOpen(step.Name)));
                            testFlow.Send(TestHelpers.DefaultWaitlistIsOpen.ToString());
                        }
                    }
                    else
                    {
                        testFlow.Send(TestHelpers.DefaultOpen.ToString());
                    }
                }
            }

            testFlow.AssertReply(Phrases.Update.Closing);

            // Run the test.
            await testFlow.StartTestAsync();

            // Validate the results.
            foreach (var type in types)
            {
                var resultData = await this.api.GetLatestServiceData(organization.Id, type, this.turnContext);
                Assert.NotNull(resultData);
                Assert.True(resultData.IsComplete);

                foreach (var step in type.UpdateSteps())
                {
                    var open = (int)resultData.GetProperty(step.OpenPropertyName);
                    Assert.Equal(testWaitlist ? 0 : TestHelpers.DefaultOpen, open);

                    var hasWaitlist = (bool)resultData.GetProperty(step.HasWaitlistPropertyName);
                    Assert.Equal(testWaitlist, hasWaitlist);

                    var waitlistIsOpen = (bool)resultData.GetProperty(step.WaitlistIsOpenPropertyName);
                    Assert.Equal(testWaitlistOpen, waitlistIsOpen);

                }
            }
        }
    }
}
