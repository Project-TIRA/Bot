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
        [MemberData(nameof(TestTypes))]
        public async Task UpdateSingle(ServiceData dataType)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { dataType });

        }

        [Fact]
        public async Task UpdateAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetServiceDataTypes());
        }

        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task WaitlistClosedSingle(ServiceData dataType)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { dataType }, testWaitlist: true);
        }

        [Fact]
        public async Task WaitlistClosedAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetServiceDataTypes(), testWaitlist: true);
        }

        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task WaitlistOpenSingle(ServiceData dataType)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, new List<ServiceData>() { dataType }, testWaitlist: true, testWaitlistOpen: true);
        }

        [Fact]
        public async Task WaitlistOpenAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);
            await RunTest(user, organization, Helpers.GetServiceDataTypes(), testWaitlist: true, testWaitlistOpen: true);
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

            // Add each sub-service from the service types to the test flow.
            foreach (var type in types)
            {
                foreach (var subService in type.SubServices())
                {
                    testFlow = testFlow.AssertReply(Phrases.Capacity.GetOpenings(subService.Name));

                    if (testWaitlist)
                    {
                        testFlow = testFlow.Send("0");
                        testFlow = testFlow.AssertReply(StartsWith(Phrases.Capacity.GetWaitlistIsOpen(subService.Name)));
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

                foreach (var subService in type.SubServices())
                {
                    var open = (int)resultData.GetProperty(subService.OpenPropertyName);
                    Assert.Equal(testWaitlist ? 0 : TestHelpers.DefaultOpen, open);

                    var hasWaitlist = (bool)resultData.GetProperty(subService.HasWaitlistPropertyName);
                    Assert.Equal(testWaitlist, hasWaitlist);

                    var waitlistIsOpen = (bool)resultData.GetProperty(subService.WaitlistIsOpenPropertyName);
                    Assert.Equal(testWaitlistOpen, waitlistIsOpen);

                }
            }
        }
    }
}
