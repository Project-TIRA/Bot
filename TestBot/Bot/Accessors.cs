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
        public Accessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets the accessor name for the dialog state property.
        /// </summary>
        /// <value>The accessor name for the dialog state property.</value>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string DialogStateName { get; } = "DialogState";

        /// <summary>
        /// Gets the accessor name for the user profile property accessor.
        /// </summary>
        /// <value>The accessor name for the user profile property accessor.</value>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string UserProfileName { get; } = "UserProfile";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for ConversationDialogState.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog state for the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for CounterState.
        /// </summary>
        /// <value>
        /// The accessor stores user data.
        /// </value>
        public IStatePropertyAccessor<UserProfile> UserProfile { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="UserState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="UserState"/> object.</value>
        public UserState UserState { get; }
    }
}