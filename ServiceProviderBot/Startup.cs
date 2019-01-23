﻿using System.Diagnostics;
using EntityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceProviderBot.Bot;

namespace ServiceProviderBot
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the DB context.
            services.AddScoped(_ => new DbModel());

            // Add the configuration.
            services.AddSingleton(configuration);

            // Create and add the state accessors.
            StateAccessors state = StateAccessors.CreateFromMemoryStorage();
            services.AddSingleton(state);

            // Configure the bot.
            services.AddBot<TheBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(configuration);

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    Debug.WriteLine(exception.Message);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // Auto-save the state after each turn.
                options.Middleware.Add(new AutoSaveStateMiddleware(state.ConversationState));
                options.Middleware.Add(new AutoSaveStateMiddleware(state.OrganizationState));
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