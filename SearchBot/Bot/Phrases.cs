﻿using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using SearchBot.Bot.State;
using Shared;
using Shared.Models;
using System;
using System.Collections.Generic;

namespace SearchBot
{
    public static class Phrases
    {
        public static class Search
        {
            public static Activity GetLocation = MessageFactory.Text("In what city are you looking for services?");
            public static Activity GetLocationRetry = MessageFactory.Text($"Oops, I couldn't find that location. Try entering as City, State");
            public static Activity GetServiceType = MessageFactory.Text($"What type of service are you looking for? I can help with {Helpers.GetServicesString(Helpers.GetServiceDataTypes())} services");

            public static Activity GetSpecificType(ServiceData dataType)
            {
                return MessageFactory.Text($"What type of {dataType.ServiceTypeName().ToLower()} are you looking for?");
            }

            public static Activity MakeRecommendation(List<MatchData> matches)
            {
                string result = string.Empty;

                if (matches.Count > 1)
                {
                    result = $"It looks like {matches.Count} organizations can help!";
                }

                foreach (var match in matches)
                {
                    if (matches.Count > 1)
                    {
                        result += Helpers.NewLine + Helpers.NewLine;
                    }

                    result += $"{match.Organization.Name} has availability for {Helpers.GetServicesString(match.RequestedServiceFlags)} services." +
                        Helpers.NewLine + $"You can reach them at {match.Organization.PhoneNumber} or {match.Organization.Address}";
                }

                return MessageFactory.Text(result);
            }

            public static Activity NoMatch(ConversationContext conversationContext)
            {
                return MessageFactory.Text($"Unfortunately it looks like no organizations within {conversationContext.SearchDistance} miles of {conversationContext.Location} have availability for {Helpers.GetServicesString(conversationContext.RequestedServiceFlags())} services");
            }

            public static Activity NoMatchSearchWider(ConversationContext conversationContext)
            {
                return MessageFactory.Text($"{NoMatch(conversationContext).Text}. Would you like to expand your search to {conversationContext.NextSearchDistance()} miles?");
            }
        }
    }
}
