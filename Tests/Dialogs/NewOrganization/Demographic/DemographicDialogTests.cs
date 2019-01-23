﻿using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Demographic
{
    public class DemographicDialogTests : DialogTestBase
    {
        [Fact]
        public async Task MenOnly()
        {
            var expected = new Organization();
            expected.Gender = Gender.Male;

            // Execute the conversation.
            await CreateTestFlow(DemographicDialog.Name)
                .Test("begin", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Send("no")
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task WomenOnly()
        {
            var expected = new Organization();
            expected.Gender = Gender.Female;

            // Execute the conversation.
            await CreateTestFlow(DemographicDialog.Name)
                .Test("begin", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Send("no")
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task GenderUnknown()
        {
            var expected = new Organization();
            expected.Gender = Gender.Unknown;

            // Execute the conversation.
            await CreateTestFlow(DemographicDialog.Name)
                .Test("begin", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Send("no")
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }
    }
}
