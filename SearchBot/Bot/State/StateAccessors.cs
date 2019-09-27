using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;

namespace SearchBot.Bot.State
{
    public class StateAccessors
    {
        /// <summary>
        /// Gets the accessor name for the dialog context property.
        /// </summary>
        /// <value>The accessor name for the dialog context property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string DialogContextName { get; } = "DialogContext";

        /// <summary>
        /// Gets the accessor name for the conversation context property.
        /// </summary>
        /// <value>The accessor name for the conversation context property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string ConversationContextName { get; } = "ConversationContext";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for DialogContext.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog context for the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogContextAccessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for ConversationContext.
        /// </summary>
        /// <value>
        /// The accessor stores the conversation context for the conversation.
        /// </value>
        public IStatePropertyAccessor<ConversationContext> ConversationContextAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        /// <param name="conversationState">The state object that stores the conversation state.</param>
        public StateAccessors(ConversationState conversationState)
        {
            this.ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            this.DialogContextAccessor = conversationState.CreateProperty<DialogState>(DialogContextName);
            this.ConversationContextAccessor = conversationState.CreateProperty<ConversationContext>(ConversationContextName);
        }

        public async Task<ConversationContext> GetConversationContext(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return await this.ConversationContextAccessor.GetAsync(turnContext, () => 
                { return new ConversationContext(); }, cancellationToken).ConfigureAwait(false);
        }

        public async Task ClearConversationContext(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await this.ConversationContextAccessor.DeleteAsync(turnContext, cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        public static StateAccessors Create(IConfiguration configuration = null)
        {
            IStorage storage;

            if (configuration == null || string.IsNullOrEmpty(configuration.CosmosKey()))
            {
                // Use the in-memory storage provider.
                storage = new MemoryStorage();
            }
            else
            {
                // Use the Cosmos storage provider.
                storage = new CosmosDbStorage(new CosmosDbStorageOptions
                {
                    CosmosDBEndpoint = configuration.CosmosEndpoint(),
                    AuthKey = configuration.CosmosKey(),
                    DatabaseId = configuration.CosmosDatabase(),
                    CollectionId = configuration.CosmosCollection(),
                });
            }

            // Create the state management and accessors.
            return new StateAccessors(new ConversationState(storage));
        }
    }
}