using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestBot.Bot;
using TestBot.Bot.Models;

namespace TestBot
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
            services.AddSingleton(configuration);

            // Create the state management with in-memory storage provider.
            IStorage storage = new MemoryStorage();
            ConversationState conversationState = new Microsoft.Bot.Builder.ConversationState(storage);
            UserState organizationProfile = new UserState(storage);

            // Create and register state accessors.
            // Accessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton<StateAccessors>(sp =>
            {
                return new StateAccessors(conversationState, organizationProfile)
                {
                    DialogContextAccessor = conversationState.CreateProperty<DialogState>(StateAccessors.DialogContextName),
                    OrganizationProfileAccessor = organizationProfile.CreateProperty<OrganizationProfile>(StateAccessors.OrganizationProfileName),
                };
            });

            // Configure the bot.
            services.AddBot<MyBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(configuration);

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    Debug.WriteLine(exception.Message);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // Auto-save the state after each turn.
                options.Middleware.Add(new AutoSaveStateMiddleware(conversationState));
                options.Middleware.Add(new AutoSaveStateMiddleware(organizationProfile));
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
