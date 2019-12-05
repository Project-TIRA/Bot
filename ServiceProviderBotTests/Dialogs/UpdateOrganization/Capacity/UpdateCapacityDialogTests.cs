using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.State;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateSingle()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            foreach (var type in Helpers.GetSubtypes<ServiceData>())
            {
                await RunTest(user, organization, new List<ServiceData>() { type });
            }
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
            // Create the user context.
            var typesToUpdate = types.Select(t => t.ServiceType()).ToList();

            // Prepare the test flow.
            var testFlow = CreateTestFlow(UpdateCapacityDialog.Name, user, typesToUpdate)
                .Send("test");

            foreach (var type in types)
            {
                // Create the service and data.
                var service = await TestHelpers.CreateService(this.api, organization.Id, type.ServiceType());
                var data = await TestHelpers.CreateServiceData(this.api, user.Id, service.Id, type, testWaitlist);

                // Add each step for the service type to the test flow.
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

            // Run the test.
            await testFlow.StartTestAsync();

            foreach (var type in types)
            {
                // Get the result data.
                var resultData = await this.api.GetLatestServiceData(organization.Id, type, this.turnContext);
                Assert.NotNull(resultData);
                Assert.True(resultData.IsComplete);


                // Validate the results.
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
