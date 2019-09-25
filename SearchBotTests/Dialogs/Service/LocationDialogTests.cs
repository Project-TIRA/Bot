﻿using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.State;
using Shared;
using Xunit;

namespace SearchBotTests.Dialogs
{
    public class LocationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NoLocation()
        {
            await CreateTestFlow(LocationDialog.Name)
                .Test("test", Phrases.Search.GetLocation(string.Empty))
                .Send(SearchBotTestHelpers.DefaultLocation)
                .StartTestAsync();

            var expectedContext = new ConversationContext();
            expectedContext.Location = SearchBotTestHelpers.DefaultLocation;

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(expectedContext, actualContext);
        }       
    }
}