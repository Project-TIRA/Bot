using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceProviderBot.Bot;
using ServiceProviderBot.Bot.Utils;
using Microsoft.Bot.Connector.Authentication;
using ServiceProviderBot.Bot.Middleware;
using Shared;
using Newtonsoft.Json;
using Shared.ApiInterface;

namespace ServiceProviderBot
{
    public class Startup
    {
        private IConfiguration configuration;
        private TelemetryClient telemetry;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            this.configuration = builder.Build();

            this.telemetry = new TelemetryClient();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        // AddSingleton - Same for every object and every request.
        // AddScoped - Same within a request but different across different requests.
        // AddTransient - Always different. New instance for every controller and service.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the configuration.
            services.AddSingleton(this.configuration);

            // Add the Common Data Service interface.
            services.AddScoped(_ => new CdsInterface());

            // Create and add the state accessors.
            var state = StateAccessors.Create(this.configuration);
            services.AddSingleton(state);

            // Ignore null json values. Will be set to default values.
            services.AddMvcCore().AddJsonOptions(options => 
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            // Configure the bot.
            services.AddBot<TheBot>(options =>
            {
                // Load the configuration settings.
                options.CredentialProvider = new SimpleCredentialProvider(
                   this.configuration.MicrosoftAppId(),
                   this.configuration.MicrosoftAppPassword());

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    Debug.WriteLine(exception.Message);
                    this.telemetry.TrackException(exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");

                    if (!configuration.IsProduction())
                    {
                        await context.SendActivityAsync(exception.Message);
                        await context.SendActivityAsync(exception.StackTrace);
                    }
                };

                // Trim the incoming message.
                options.Middleware.Add(new TrimIncomingMessageMiddleware());

                // Auto-save the state after each turn.
                options.Middleware.Add(new AutoSaveStateMiddleware(state.ConversationState));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
