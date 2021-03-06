﻿using EntityModel;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bot.Connector.Authentication;
using SearchBot.Bot;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using System.Diagnostics;
using Shared.Middleware;
using Microsoft.ApplicationInsights.Extensibility;

namespace SearchBot
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

            var appInsightsConfigString = this.configuration.ApplicationInsightsConfiguration();
            var config = string.IsNullOrEmpty(appInsightsConfigString) ? TelemetryConfiguration.CreateDefault() : TelemetryConfiguration.CreateFromConfiguration(appInsightsConfigString);
            this.telemetry = new TelemetryClient(config);
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
            //services.AddScoped(_ => new CdsInterface(this.configuration));

            // Add the DB interface.
            services.AddScoped(_ => new EfInterface(DbModelFactory.Create(configuration.DbModelConnectionString())));

            // Create and add the state accessors.
            var state = StateAccessors.Create(this.configuration);
            services.AddSingleton(state);

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

                    await context.TraceActivityAsync("Exception", exception);

                    if (!configuration.IsProduction())
                    {
                        await context.SendActivityAsync(exception.Message);
                        await context.SendActivityAsync(exception.StackTrace);
                    }

                    await context.SendActivityAsync(Shared.Phrases.Exceptions.Search);
                };

                // Auto-save the state after each turn.
                // This should be the first middleware called in order to catch state changes by any other middleware or the bot.
                options.Middleware.Add(new AutoSaveStateMiddleware(state.ConversationState));

                // Trim the incoming message.
                options.Middleware.Add(new TrimIncomingMessageMiddleware());
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
