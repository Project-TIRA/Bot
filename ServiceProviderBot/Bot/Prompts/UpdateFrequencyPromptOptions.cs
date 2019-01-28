using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using ServiceProviderBot.Bot.Utils;
using System;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Prompts
{
    public class UpdateFrequencyPromptOptions : PromptOptions
    {
        public UpdateFrequencyPromptOptions()
        {
            this.Choices = new List<Choice>();

            foreach (var frequency in Enum.GetValues(typeof(Frequency)))
            {
                this.Choices.Add(new Choice { Value = frequency.ToString() });
            }

            this.Prompt = Phrases.Capacity.GetFrequency;
        }
    }
}
