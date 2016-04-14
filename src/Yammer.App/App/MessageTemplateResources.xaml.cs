//---------------------------------------------------------------------
// <copyright file="MessageTemplateResources.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Threading.Tasks;
    using API.Models;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Message template resources class
    /// </summary>
    public partial class MessageTemplateResources
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTemplateResources"/> class.
        /// </summary>
        public MessageTemplateResources()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the LoadMoreItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void LoadMoreItems_Click(object sender, RoutedEventArgs e)
        {
            Task task = ((MessageCollection)((e.OriginalSource as Button)?.Tag as MessagesViewModel)?.Messages).LoadMoreItemsAsync(true);
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

            this.ReplyToMessage(message);
        }

        /// <summary>
        /// Handles the SwipedRight event of the Message control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void Message_SwipedRight(object sender, EventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.Tag;
            Task task = message.ToggleLikeAsync();
        }

        /// <summary>
        /// Handles the Click event of the ReplyToMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void ReplyToMessage_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.Tag;

            this.ReplyToMessage(message);
        }

        /// <summary>
        /// Replies to message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ReplyToMessage(Message message)
        {
            Frame frame = (Frame)Window.Current.Content;
            MainPage page = (MainPage)frame.Content;
            page.NavigateTo(typeof(ConversationPage), new ConversationPage.Arguments(message, true), false);
        }

        /// <summary>
        /// Handles the Click event of the LikeMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void LikeMessage_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.Tag;
            Task task = message.ToggleLikeAsync();
        }

        /// <summary>
        /// Handles the Click event of the ImageAttachment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Controls.ItemClickEventArgs" /> instance containing the event data.</param>
        private async void ImageAttachment_Click(object sender, ItemClickEventArgs e)
        {
            Frame frame = (Frame)Window.Current.Content;
            MainPage page = (MainPage)frame.Content;
            ImageAttachment attachment = (ImageAttachment)e.ClickedItem;

            if (!attachment.IsBusy)
            {
                await page.ShowImageAttachmentAsync(attachment);
            }
        }

        /// <summary>
        /// Handles the Click event of the AttachedConversationLink control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void AttachedConversationLink_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Frame frame = (Frame)Window.Current.Content;
            MainPage page = (MainPage)frame.Content;

            MessageAttachment attachment = (MessageAttachment)element.Tag;

            // Force navigation to ensure that there is a conversation history maintained
            page.NavigateTo(typeof(ConversationPage), new ConversationPage.Arguments(attachment, false), true);
        }
    }
}
