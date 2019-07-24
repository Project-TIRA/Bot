using System;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected const string TestOrgName = "Test Org";
        protected const string TestOrgPartialAddress = "4215 Rainier Ave Seattle";
        protected const string TestOrgFullAddress = "4215 Rainier Avenue South, Seattle, WA 98118";
        protected const string TestOrgCity = "Seattle";
        protected const string TestOrgState = "WA";
        protected const string TestOrgZip = "98052";

        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly DbInterface database;
        protected readonly TestAdapter adapter;
        private readonly IConfiguration configuration;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        protected DialogTestBase()
        {
            this.state = StateAccessors.Create();
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.database = new DbInterface(DbModelFactory.CreateInMemory());
            this.adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(state.ConversationState));

            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true).Build();

            // Register prompts.
            Prompts.Register(this.state, this.dialogs, this.database);
        }

        protected TestFlow CreateTestFlow(string dialogName, Organization initialOrganization = null, Snapshot initialSnapshot = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                // Initialize the dialog context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.database, this.configuration);

                // Attempt to continue any existing conversation.
                DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);
                var startNewConversation = turnContext.Activity.Type == ActivityTypes.Message && results.Status == DialogTurnStatus.Empty;

                if (startNewConversation)
                {
                    // Init at the start of the conversation. Need to do before checking for expired data.
                    await InitDatabase(turnContext, initialOrganization, initialSnapshot);
                }

                // Check if the conversation is expired.
                var forceExpire = Phrases.TriggerReset(turnContext);
                var expired = await this.database.CheckExpiredConversation(turnContext, forceExpire);

                if (expired)
                {
                    await dialogContext.CancelAllDialogsAsync(cancellationToken);
                    await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
                }
                else if (startNewConversation)
                {
                    // Difference for tests here is starting the given dialog instead of master so that individual dialog flows can be tested.
                    await masterDialog.BeginDialogAsync(dialogContext, dialogName, null, cancellationToken);
                }
            });
        }

        protected Organization CreateDefaultTestOrganization()
        {
            var organization = new Organization();
            organization.Name = TestOrgName;
            organization.Gender = Gender.All;
            organization.City = TestOrgCity;
            organization.State = TestOrgState;
            organization.Zip = TestOrgZip;
            return organization;
        }

        protected async Task ValidateProfile(Organization expectedOrganization = null, Snapshot expectedSnapshot = null)
        {
            if (expectedOrganization != null)
            {
                var actualOrganization = await this.database.GetOrganization(this.turnContext);
                Assert.Equal(actualOrganization.Name, expectedOrganization.Name);
                Assert.Equal(actualOrganization.Gender, expectedOrganization.Gender);
                Assert.Equal(actualOrganization.AgeRangeStart, expectedOrganization.AgeRangeStart);
                Assert.Equal(actualOrganization.AgeRangeEnd, expectedOrganization.AgeRangeEnd);

                //Case Management
                Assert.Equal(actualOrganization.CaseManagementTotal, expectedOrganization.CaseManagementTotal);
                Assert.Equal(actualOrganization.CaseManagementHasWaitlist, expectedOrganization.CaseManagementHasWaitlist);
                Assert.Equal(actualOrganization.CaseManagementGender, expectedOrganization.CaseManagementGender);
                Assert.Equal(actualOrganization.CaseManagementAgeRangeStart, expectedOrganization.CaseManagementAgeRangeStart);
                Assert.Equal(actualOrganization.CaseManagementAgeRangeEnd, expectedOrganization.CaseManagementAgeRangeEnd);
                Assert.Equal(actualOrganization.CaseManagementSobriety, expectedOrganization.CaseManagementSobriety);
                
                Assert.Equal(actualOrganization.HousingEmergencyPrivateTotal, expectedOrganization.HousingEmergencyPrivateTotal);
                Assert.Equal(actualOrganization.HousingEmergencySharedTotal, expectedOrganization.HousingEmergencySharedTotal);
                Assert.Equal(actualOrganization.HousingLongtermPrivateTotal, expectedOrganization.HousingLongtermPrivateTotal);
                Assert.Equal(actualOrganization.HousingLongtermSharedTotal, expectedOrganization.HousingLongtermSharedTotal);
                Assert.Equal(actualOrganization.HousingHasWaitlist, expectedOrganization.HousingHasWaitlist);
                Assert.Equal(actualOrganization.HousingAgeRangeStart, expectedOrganization.HousingAgeRangeStart);
                Assert.Equal(actualOrganization.HousingAgeRangeEnd, expectedOrganization.HousingAgeRangeEnd);
                Assert.Equal(actualOrganization.HousingGender, expectedOrganization.HousingGender);
                Assert.Equal(actualOrganization.HousingFamilyStatus, expectedOrganization.HousingFamilyStatus);
            }

            if (expectedSnapshot != null)
            {
                var actualSnapshot = await this.database.GetSnapshot(this.turnContext);
                Assert.Equal(actualSnapshot.BedsEmergencyPrivateOpen, expectedSnapshot.BedsEmergencyPrivateOpen);
                Assert.Equal(actualSnapshot.BedsEmergencySharedOpen, expectedSnapshot.BedsEmergencySharedOpen);
                Assert.Equal(actualSnapshot.BedsLongtermPrivateOpen, expectedSnapshot.BedsLongtermPrivateOpen);
                Assert.Equal(actualSnapshot.BedsLongtermSharedOpen, expectedSnapshot.BedsLongtermSharedOpen);
                Assert.Equal(actualSnapshot.BedsEmergencyPrivateWaitlistLength, expectedSnapshot.BedsEmergencyPrivateWaitlistLength);
                Assert.Equal(actualSnapshot.BedsEmergencySharedWaitlistLength, expectedSnapshot.BedsEmergencySharedWaitlistLength);
                Assert.Equal(actualSnapshot.BedsLongtermPrivateWaitlistLength, expectedSnapshot.BedsLongtermPrivateWaitlistLength);
                Assert.Equal(actualSnapshot.BedsLongtermSharedWaitlistLength, expectedSnapshot.BedsLongtermSharedWaitlistLength);
            }
        }

        protected async Task ValidateProfileMentalHealth(Organization expectedOrganization = null, Snapshot expectedSnapshot = null)
        {
            if (expectedOrganization != null)
            {
                var actualOrganization = await this.database.GetOrganization(this.turnContext);
                Assert.Equal(actualOrganization.Name, expectedOrganization.Name);
                Assert.Equal(actualOrganization.Gender, expectedOrganization.Gender);
                Assert.Equal(actualOrganization.AgeRangeStart, expectedOrganization.AgeRangeStart);
                Assert.Equal(actualOrganization.AgeRangeEnd, expectedOrganization.AgeRangeEnd);
                Assert.Equal(actualOrganization.TotalBeds, expectedOrganization.TotalBeds);
                Assert.Equal(actualOrganization.MentalHealth_InPatientTotal, expectedOrganization.MentalHealth_InPatientTotal);
                Assert.Equal(actualOrganization.MentalHealth_OutPatientTotal, expectedOrganization.MentalHealth_OutPatientTotal);
                Assert.Equal(actualOrganization.MentalHealth_HasWaitlist, expectedOrganization.MentalHealth_HasWaitlist);
            }

            if (expectedSnapshot != null)
            {
                var actualSnapshot = await this.database.GetSnapshot(this.turnContext);
                Assert.Equal(actualSnapshot.OpenBeds, expectedSnapshot.OpenBeds);
                Assert.Equal(actualSnapshot.MentalHealth_InPatientOpen, expectedSnapshot.MentalHealth_InPatientOpen);
                Assert.Equal(actualSnapshot.MentalHealth_OutPatientOpen, expectedSnapshot.MentalHealth_OutPatientOpen);
                Assert.Equal(actualSnapshot.MentalHealth_InPatientWaitlistLength, expectedSnapshot.MentalHealth_InPatientWaitlistLength);
                Assert.Equal(actualSnapshot.MentalHealth_OutPatientWaitlistLength, expectedSnapshot.MentalHealth_OutPatientWaitlistLength);
            }
        }

        protected Action<IActivity> StartsWith(IMessageActivity expected)
        {
            return receivedActivity =>
            {
                // Validate the received activity.
                var received = receivedActivity.AsMessageActivity();
                Assert.NotNull(received);
                Assert.StartsWith(expected.Text, received.Text);
            };
        }

        private async Task InitDatabase(ITurnContext turnContext, Organization initialOrganization = null, Snapshot initialSnapshot = null)
        {
            Assert.True(initialSnapshot == null || initialOrganization != null, "Cannot initialize a snapshot without an organization");

            // Create the organization and snapshot if provided.
            if (initialOrganization != null)
            {
                var organization = await this.database.CreateOrganization(turnContext);
                organization.DateCreated = initialOrganization.DateCreated;
                organization.IsVerified = initialOrganization.IsVerified;
                organization.Name = initialOrganization.Name;
                organization.City = initialOrganization.City;
                organization.State = initialOrganization.State;
                organization.Zip = initialOrganization.Zip;
                organization.Gender = initialOrganization.Gender;
                organization.AgeRangeStart = initialOrganization.AgeRangeStart;
                organization.AgeRangeEnd = initialOrganization.AgeRangeEnd;
                organization.HasJobTrainingServices = initialOrganization.HasJobTrainingServices;
                organization.HasJobTrainingWaitlist = initialOrganization.HasJobTrainingWaitlist;
                organization.TotalJobTrainingPositions = initialOrganization.TotalJobTrainingPositions;
                organization.OpenJobTrainingPositions = initialOrganization.OpenJobTrainingPositions;
                organization.JobTrainingWaitlistPositions = initialOrganization.JobTrainingWaitlistPositions;

                //Case Management
                organization.CaseManagementTotal = initialOrganization.CaseManagementTotal;
                organization.CaseManagementHasWaitlist = initialOrganization.CaseManagementHasWaitlist;
                organization.CaseManagementGender = initialOrganization.CaseManagementGender;
                organization.HousingEmergencyPrivateTotal = initialOrganization.HousingEmergencyPrivateTotal;
                organization.HousingEmergencySharedTotal = initialOrganization.HousingEmergencySharedTotal;
                organization.HousingLongtermPrivateTotal = initialOrganization.HousingLongtermPrivateTotal;
                organization.HousingLongtermSharedTotal = initialOrganization.HousingLongtermSharedTotal;
                organization.HousingHasWaitlist = initialOrganization.HousingHasWaitlist;
                organization.HousingAgeRangeStart = initialOrganization.HousingAgeRangeStart;
                organization.HousingAgeRangeEnd = initialOrganization.HousingAgeRangeEnd;
                organization.HousingGender = initialOrganization.HousingGender;
                organization.HousingFamilyStatus = initialOrganization.HousingFamilyStatus;
                organization.HousingServiceAnimal = initialOrganization.HousingServiceAnimal;
                organization.HousingSobriety = initialOrganization.HousingSobriety;
                organization.CaseManagementAgeRangeStart = initialOrganization.CaseManagementAgeRangeStart;
                organization.CaseManagementAgeRangeEnd = initialOrganization.CaseManagementAgeRangeEnd;
                organization.CaseManagementSobriety = initialOrganization.CaseManagementSobriety;
                
                

                if (initialSnapshot != null)
                {
                    var snapshot = await this.database.CreateSnapshot(turnContext);
                    snapshot.Date = initialSnapshot.Date;
                    snapshot.OpenBeds = initialSnapshot.OpenBeds;
                    snapshot.MentalHealth_InPatientOpen = initialSnapshot.MentalHealth_InPatientOpen;
                    snapshot.MentalHealth_OutPatientOpen = initialSnapshot.MentalHealth_OutPatientOpen;
                    snapshot.MentalHealth_InPatientWaitlistLength = initialSnapshot.MentalHealth_InPatientWaitlistLength;
                    snapshot.MentalHealth_OutPatientWaitlistLength = initialSnapshot.MentalHealth_OutPatientWaitlistLength;

                    //Case Management
                    snapshot.CaseManagementOpenSlots = initialSnapshot.CaseManagementOpenSlots;
                    snapshot.CaseManagementWaitlistLength = initialSnapshot.CaseManagementWaitlistLength;
                    snapshot.BedsEmergencyPrivateOpen = initialSnapshot.BedsEmergencyPrivateOpen;
                    snapshot.BedsEmergencySharedOpen = initialSnapshot.BedsEmergencySharedOpen;
                    snapshot.BedsLongtermPrivateOpen = initialSnapshot.BedsLongtermPrivateOpen;
                    snapshot.BedsLongtermSharedOpen = initialSnapshot.BedsLongtermSharedOpen;
                    snapshot.BedsEmergencyPrivateWaitlistLength = initialSnapshot.BedsEmergencyPrivateWaitlistLength;
                    snapshot.BedsEmergencySharedWaitlistLength = initialSnapshot.BedsEmergencySharedWaitlistLength;
                    snapshot.BedsLongtermPrivateWaitlistLength = initialSnapshot.BedsLongtermPrivateWaitlistLength;
                    snapshot.BedsLongtermSharedWaitlistLength = initialSnapshot.BedsLongtermSharedWaitlistLength;
                }

                await this.database.Save();
            }
        }
    }
}
