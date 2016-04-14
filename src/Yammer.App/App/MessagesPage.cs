//---------------------------------------------------------------------
// <copyright file="MessagesPage.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System.Threading.Tasks;
    using API.Models;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Messages page class
    /// </summary>
    public abstract class MessagesPage : PageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesPage"/> class.
        /// </summary>
        public MessagesPage()
        {
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public MessagesViewModel Model
        {
            get
            {
                return (MessagesViewModel)this.DataContext;
            }

            set
            {
                this.DataContext = value;
                this.NotifyPropertyChanged();
                this.OnModelUpdated();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int id = this.GetPageId();
            await this.OnNavigatedToAsync(id);  
        }

        /// <summary>
        /// Called when page is navigated to, asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        protected async Task OnNavigatedToAsync(int id)
        {
            if (this.Model == null || this.Model.Id != id)
            {
                this.Model = await this.CreateModelAsync(id);
            }
            else
            {
                this.Model.Refresh();
            }
        }

        /// <summary>
        /// Gets the page identifier.
        /// </summary>
        /// <returns>
        /// The page identifier.
        /// </returns>
        protected virtual int GetPageId()
        {
            return 1;
        }

        /// <summary>
        /// Creates the model, asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The created model task.
        /// </returns>
        protected abstract Task<MessagesViewModel> CreateModelAsync(int id);

        /// <summary>
        /// Called when the model is updated.
        /// </summary>
        protected abstract void OnModelUpdated();

        /// <summary>
        /// Handles the Click event of the Refresh control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.Model.Refresh(true);
        }

        /// <summary>
        /// Handles the Click event of the Message control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance containing the event data.</param>
        protected void Message_Click(object sender, ItemClickEventArgs e)
        {
            Message message = (Message)e.ClickedItem;
            if (message != null)
            {
                this.Frame.Navigate(typeof(ConversationPage), new ConversationPage.Arguments(message, false));
            }
        }
    }
}
