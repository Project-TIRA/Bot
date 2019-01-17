// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See <see cref="StateAccessors"/> constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class StateAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        /// <param name="conversationState">The state object that stores the conversation state.</param>
        /// <param name="organizationState">The state object that stores the organization state.</param>
        public StateAccessors(ConversationState conversationState, UserState organizationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            OrganizationState = organizationState ?? throw new ArgumentNullException(nameof(organizationState));
        }

        /// <summary>
        /// Gets the accessor name for the dialog context property.
        /// </summary>
        /// <value>The accessor name for the dialog state property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string DialogContextName { get; } = "DialogContext";

        /// <summary>
        /// Gets the accessor name for the organization profile property accessor.
        /// </summary>
        /// <value>The accessor name for the user profile property accessor.</value>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string OrganizationProfileName { get; } = "OrganizationProfile";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for DialogContext.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog context for the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogContextAccessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for OrganizationProfile.
        /// </summary>
        /// <value>
        /// The accessor stores user data.
        /// </value>
        public IStatePropertyAccessor<OrganizationProfile> OrganizationProfileAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="OrganizationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="OrganizationState"/> object.</value>
        public UserState OrganizationState { get; }

        /// <summary>
        /// Gets the <see cref="OrganizationProfile"/> object for the conversation.
        /// </summary>
        public async Task<OrganizationProfile> GetOrganizationProfile(ITurnContext context, CancellationToken cancellationToken)
        {
            return await this.OrganizationProfileAccessor.GetAsync(context, () => new OrganizationProfile(), cancellationToken);
        }
    }
}