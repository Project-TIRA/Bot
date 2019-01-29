using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceProviderBot.Bot;
using ServiceProviderBot.Bot.Utils;
using System;
using EntityModel;

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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            this.configuration = builder.Build();

            this.telemetry = new TelemetryClient();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the configuration.
            services.AddSingleton(this.configuration);

            // Configure the SnapshotCollector.
            services.Configure<SnapshotCollectorConfiguration>(configuration.SnapshotCollectorConfiguration());

            // Add the SnapshotCollector telemetry processor.
            services.AddSingleton<ITelemetryProcessorFactory>(s => new SnapshotCollectorTelemetryProcessorFactory(s));

            // Add the database interface.
            services.AddScoped(_ => new DbInterface(DbModelFactory.Create(configuration.DbModelConnectionString())));

            // Create and add the state accessors.
            var state = StateAccessors.Create(this.configuration);
            services.AddSingleton(state);

            // Configure the bot.
            services.AddBot<TheBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(this.configuration);

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    Debug.WriteLine(exception.Message);
                    this.telemetry.TrackException(exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

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

    public class SnapshotCollectorTelemetryProcessorFactory : ITelemetryProcessorFactory
    {
        private readonly IServiceProvider serviceProvider;

        public SnapshotCollectorTelemetryProcessorFactory(IServiceProvider serviceProvider) =>
            this.serviceProvider = serviceProvider;

        public ITelemetryProcessor Create(ITelemetryProcessor next)
        {
            var snapshotConfigurationOptions = serviceProvider.GetService<IOptions<SnapshotCollectorConfiguration>>();
            return new SnapshotCollectorTelemetryProcessor(next, snapshotConfigurationOptions.Value);
        }
    }
}
