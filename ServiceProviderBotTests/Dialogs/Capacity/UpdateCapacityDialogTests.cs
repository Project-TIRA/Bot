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
        [Theory]
        [MemberData(nameof(TestServiceTypes))]
        public async Task UpdateSingle(ServiceData type)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { type });

        }

        [Fact]
        public async Task UpdateAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetSubtypes<ServiceData>());
        }

        [Theory]
        [MemberData(nameof(TestServiceTypes))]
        public async Task WaitlistClosedSingle(ServiceData type)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { type }, testWaitlist: true);
        }

        [Fact]
        public async Task WaitlistClosedAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetSubtypes<ServiceData>(), testWaitlist: true);
        }

        [Theory]
        [MemberData(nameof(TestServiceTypes))]
        public async Task WaitlistOpenSingle(ServiceData type)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { type }, testWaitlist: true, testWaitlistOpen: true);
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
                testFlow = testFlow.AssertReply(StartsWith(Phrases.Update.Options));
                testFlow = testFlow.Send(Phrases.Services.All);
            }

            // Add each step from the service types to the test flow.
            foreach (var type in types)
            {
                foreach (var step in type.UpdateSteps())
                {
                    testFlow = testFlow.AssertReply(Phrases.Capacity.GetOpenings(step.Name));

                    if (testWaitlist)
                    {
                        testFlow = testFlow.Send("0");
                        testFlow = testFlow.AssertReply(StartsWith(Phrases.Capacity.GetWaitlistIsOpen(step.Name)));
                        testFlow = testFlow.Send(testWaitlistOpen.ToString());
                    }
                    else
                    {
                        testFlow = testFlow.Send(TestHelpers.DefaultOpen.ToString());
                    }
                }
            }

            testFlow = testFlow.AssertReply(Phrases.Update.Closing);

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

        public static IEnumerable<object[]> TestServiceTypes =>
            Helpers.GetSubtypes<ServiceData>().Select(t => new object[] { t });
    }
}
