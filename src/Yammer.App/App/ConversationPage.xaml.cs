//---------------------------------------------------------------------
// <copyright file="ConversationPage.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Linq;
    using API.Models;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Conversation page class
    /// </summary>
    public sealed partial class ConversationPage : PageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationPage"/> class.
        /// </summary>
        public ConversationPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public ViewModel Model
        {
            get
            {
                return (ViewModel)this.DataContext;
            }

            set
            {
                this.DataContext = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Tries to re-navigate to this page.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// true if successfull, false otherwise
        /// </returns>
        public override bool TryReNavigateTo(object parameter)
        {
            bool result = false;
            Arguments arguments = (Arguments)parameter;
            if (this.Model.ThreadId == arguments.GetThreadId())
            {
                result = true;
                if (arguments.DirectReply)
                {
                    // in cases where we know the message upfront
                    if (arguments.Message != null)
                    {
                        this.Model.ReplyTo = arguments.Message;
                    }

                    this.FocusComposeTextBox();
                }
            }

            return result;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Arguments arguments = (Arguments)e.Parameter;
            this.Model = new ViewModel(arguments);
            await this.Model.RefreshAsync(false);

            if (arguments.DirectReply)
            {
                this.FocusComposeTextBox();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedFrom" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Navigation.NavigationEventArgs" /> instance containing the event data.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.Model?.Dispose();
            this.Model = null;
        }

        /// <summary>
        /// Handles the Click event of the ShowOlderReplies control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ShowOlderReplies_Click(object sender, RoutedEventArgs e)
        {
            Message oldestMessage = this.Model.Messages.First();
            await this.Model.LoadMoreAsync();
            this.RepliesListView.ScrollIntoView(oldestMessage, ScrollIntoViewAlignment.Default);
        }

        /// <summary>
        /// Handles the Click event of the Reply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Reply_Click(object sender, RoutedEventArgs e)
        {
            this.RepliesListView.ScrollIntoView(this.RepliesListView.Footer);
        }

        /// <summary>
        /// Handles the Click event of the Refresh control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.Model.Refresh(true);
        }

        /// <summary>
        /// Handles the Tapped event of the ReplySend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Input.TappedRoutedEventArgs" /> instance containing the event data.</param>
        private async void ReplySend_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await this.Model.PostMessageAsync();
            this.RepliesListView.ScrollIntoView(this.Model.Messages.Last());
        }

        /// <summary>
        /// Handles the SwipedLeft event of the Message control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void Message_SwipedLeft(object sender, EventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.Tag;
            this.Model.ReplyTo = message;
            this.ComposeTextBox.FocusText();
            this.FocusComposeTextBox();
        }

        /// <summary>
        /// Focuses the compose text box.
        /// </summary>
        private void FocusComposeTextBox()
        {
            IAsyncAction focusAction = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ComposeTextBox.FocusText());
        }

        /// <summary>
        /// Handles the SwipedRight event of the Message control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private async void Message_SwipedRight(object sender, EventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.Tag;
            await this.Model.ToggleMessageLikeAsync(message);
        }

        /// <summary>
        /// Arguments class
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Arguments" /> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="directReply">if set to <c>true</c> [direct reply].</param>
            public Arguments(Message message, bool directReply)
            {
                this.Message = message.ThrowIfNull(nameof(message));
                this.DirectReply = directReply;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Arguments" /> class.
            /// </summary>
            /// <param name="messageAttachment">The message attachment.</param>
            /// <param name="directReply">if set to <c>true</c> [direct reply].</param>
            public Arguments(MessageAttachment messageAttachment, bool directReply)
            {
                this.MessageAttachment = messageAttachment;
                this.DirectReply = directReply;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            /// <value>
            /// The message.
            /// </value>
            public Message Message
            {
                get;
            }

            /// <summary>
            /// Gets the message attachment.
            /// </summary>
            /// <value>
            /// The message attachment.
            /// </value>
            public MessageAttachment MessageAttachment
            {
                get;
            }

            /// <summary>
            /// Gets a value indicating whether a direct reply.
            /// </summary>
            /// <value>
            /// return <c>true</c> if a direct reply; otherwise, <c>false</c>.
            /// </value>
            public bool DirectReply
            {
                get;
            }

            /// <summary>
            /// Gets the thread identifier.
            /// </summary>
            /// <returns>
            /// The thread id.
            /// </returns>
            public int GetThreadId()
            {
                int id;
                if (this.Message != null)
                {
                    id = this.Message.ThreadId;
                }
                else
                {
                    id = this.MessageAttachment.ThreadId;
                }

                return id;
            }
        }
    }
}
