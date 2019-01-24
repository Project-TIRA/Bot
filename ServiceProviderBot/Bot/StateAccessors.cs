// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        /// Gets the <see cref="DbModel"/> for accessing the database.
        /// </summary>
        /// <value>The <see cref="DbModel"/> object.</value>
        public DbModel DbContext { get; }

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

            this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        public static StateAccessors Create(DbModel dbContext)
        {
            // Create the state management with in-memory storage provider.
            // TODO: Use CosmosDB if not in-memory.
            IStorage storage = new MemoryStorage();
            ConversationState conversationState = new ConversationState(storage);
                
            // Create the state accessors.
            return new StateAccessors(conversationState, dbContext);
        }

        public async Task SaveDbContext()
        {
            await this.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Creates an organization in the database.
        /// </summary>
        public async Task<Organization> CreateOrganization(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;

            var organization = new Organization();
            organization.PhoneNumber = phoneNumber;
            await this.DbContext.Organizations.AddAsync(organization);
            await this.DbContext.SaveChangesAsync();

            return organization;
        }

        /// <summary>
        /// Gets the current organization from the database.
        /// </summary>
        public async Task<Organization> GetOrganization(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;
            return await this.DbContext.Organizations.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// Creates an shapshot in the database.
        /// </summary>
        public async Task<Snapshot> CreateSnapshot(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;
            var organization = await this.DbContext.Organizations.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber);

            if (organization == null)
            {
                return null;
            }

            var snapshot = new Snapshot(organization.Id);
            await this.DbContext.Snapshots.AddAsync(snapshot);
            await this.DbContext.SaveChangesAsync();

            return snapshot;
        }

        /// <summary>
        /// Gets the latest snapshot from the database.
        /// </summary>
        public async Task<Snapshot> GetSnapshot(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;
            var organization = await this.DbContext.Organizations.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber);

            if (organization == null)
            {
                return null;
            }

            // Get the most recent snapshot.
            return organization.Snapshots.OrderByDescending(s => s.Date).FirstOrDefault();
        }
    }
}