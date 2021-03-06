﻿using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Shared;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class LocationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NoLocation()
        {
            await CreateTestFlow(LocationDialog.Name)
                .Test("test", SearchBot.Phrases.Search.GetLocation)
                .Send(TestHelpers.DefaultLocation)
                .StartTestAsync();

            var expectedContext = new ConversationContext();
            expectedContext.TEST_SetLocation(TestHelpers.DefaultLocation, TestHelpers.DefaultLocationPosition);

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(expectedContext, actualContext);
        }       
    }
}
