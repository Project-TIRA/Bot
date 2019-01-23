﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using ServiceProviderBot.Bot.Models.LocationApi;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Location
{
    public class LocationDialog : DialogBase
    {
        public static string Name = typeof(LocationDialog).FullName;

        // TODO: Store this outside of the repo.
        private const string SubscriptionKey = "VHtwi2RwWsjW_xn2M-3Wrn0MPVSWx7aQqseh2HwmNQc";
        private const string MapsApiUriFormat = "https://atlas.microsoft.com/search/fuzzy/json?" +
            "api-version=1.0&countrySet=US&subscription-key={0}&query={1}";

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the location.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.LocationTextPrompt,
                        new PromptOptions { Prompt = Phrases.Location.GetLocation },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var zipcode = (string)stepContext.Result;

                    if (string.IsNullOrEmpty(zipcode))
                    {
                        return await NotifyErrorAndRepeat(stepContext, cancellationToken);
                    }

                    // Validate the location with the Azure Maps API.
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var queryString = string.Format(MapsApiUriFormat, SubscriptionKey, zipcode);
                            HttpResponseMessage responseMessage = await client.GetAsync(queryString);

                            if (!responseMessage.IsSuccessStatusCode)
                            {
                                return await NotifyErrorAndRepeat(stepContext, cancellationToken);
                            }

                            var response = await responseMessage.Content.ReadAsStringAsync();
                            LocationApiResponse result = JsonConvert.DeserializeObject<LocationApiResponse>(response);
                            LocationApiAddress address = result.GetTopStreetResult();

                            if (address == null || address.PostalCode != zipcode)
                            {
                                return await NotifyErrorAndRepeat(stepContext, cancellationToken);
                            }
                                
                            // Update the profile with the location.
                            var organization = await state.GetOrganization(stepContext.Context);
                            organization.City = address.Municipality;
                            organization.State = address.CountrySubdivision;
                            organization.Zip = address.PostalCode;
                            await state.SaveDbContext();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return await NotifyErrorAndRepeat(stepContext, cancellationToken);
                    }
                     
                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        private async Task<DialogTurnResult> NotifyErrorAndRepeat(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Notify about the error.
            await Messages.SendAsync(Phrases.Location.GetLocationError, stepContext.Context, cancellationToken);

            // Repeat the dialog.
            return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
        }
    }
}
