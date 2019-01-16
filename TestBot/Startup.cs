using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestBot.Bot;

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
            services.AddBot<MyBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(configuration);

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };
            });

            // Create conversation and user state with in-memory storage provider.
            IStorage storage = new MemoryStorage();
            ConversationState conversationState = new ConversationState(storage);
            UserState userState = new UserState(storage);

            // Create and register state accessors.
            // Accessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton<Accessors>(sp =>
            {
                // Create the custom state accessor.
                return new Accessors(conversationState, userState)
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>(Accessors.DialogStateName),
                    UserProfile = userState.CreateProperty<UserProfile>(Accessors.UserProfileName),
                };
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
