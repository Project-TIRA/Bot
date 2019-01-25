// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot
{
    public class StateAccessors
    {
        /// <summary>
        /// Gets the accessor name for the dialog context property.
        /// </summary>
        /// <value>The accessor name for the dialog state property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string DialogContextName { get; } = "DialogContext";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for DialogContext.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog context for the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogContextAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="DbInterface"/> for accessing the database.
        /// </summary>
        /// <value>The <see cref="DbInterface"/> object.</value>
        public DbInterface Database { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        /// <param name="conversationState">The state object that stores the conversation state.</param>
        /// <param name="conversationState">The database object for accessing the database.</param>
        public StateAccessors(ConversationState conversationState, DbModel dbContext)
        {
            this.ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            this.DialogContextAccessor = conversationState.CreateProperty<DialogState>(DialogContextName);

            this.Database = new DbInterface(dbContext ?? throw new ArgumentNullException(nameof(dbContext)));
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

            // Create the state management.
            ConversationState conversationState = new ConversationState(storage);

            // Create the database either from the connection string or in-memory.
            DbModel dbContext = configuration == null ?
                DbModelFactory.CreateInMemory() : DbModelFactory.Create(configuration.DbModelConnectionString());
                
            // Create the state accessors.
            return new StateAccessors(conversationState, dbContext);
        }
    }
}