// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See <see cref="Accessors"/> constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class Accessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Accessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        /// <param name="conversationState">The state object that stores the conversation state.</param>
        /// <param name="userState">The state object that stores the user state.</param>
        public Accessors(ConversationState conversationState, UserState organizationProfile)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            OrganizationState = organizationProfile ?? throw new ArgumentNullException(nameof(organizationProfile));
        }

        /// <summary>
        /// Gets the accessor name for the dialog context property.
        /// </summary>
        /// <value>The accessor name for the dialog state property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string DialogContextName { get; } = "DialogContext";

        /// <summary>
        /// Gets the accessor name for the conversation flow index property.
        /// </summary>
        /// <value>The accessor name for the conversation flow property.</value>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string ConversationFlowIndexName { get; } = "ConversationFlowIndex";

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
        public IStatePropertyAccessor<DialogState> DialogContext { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for ConversationFlowIndex.
        /// </summary>
        /// <value>
        /// The accessor stores the conversation flow for the conversation.
        /// </value>
        public IStatePropertyAccessor<int> ConversationFlowIndex { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for OrganizationProfile.
        /// </summary>
        /// <value>
        /// The accessor stores user data.
        /// </value>
        public IStatePropertyAccessor<OrganizationProfile> OrganizationProfile { get; set; }

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
    }
}