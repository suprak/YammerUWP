//---------------------------------------------------------------------
// <copyright file="ViewModelBase.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model base class
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The property event arguments cache
        /// </summary>
        private static readonly Dictionary<string, PropertyChangedEventArgs> PropertyEventArgumentsCache = new Dictionary<string, PropertyChangedEventArgs>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The properties
        /// </summary>
        private readonly Dictionary<string, object> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
            : this(TimeSpan.FromHours(1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="refreshInterval">The refresh interval.</param>
        protected ViewModelBase(TimeSpan refreshInterval)
        {
            this.properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.RefreshCommand = new RefreshCommand(this);
            this.IsBusy = false;
            this.RefreshInterval = refreshInterval;
            this.LastRefresh = DateTimeOffset.MinValue;
        }

        /// <summary>
        /// Occurs when the model has refreshed.
        /// </summary>
        public event EventHandler Refreshed;

        /// <summary>
        /// Occurs when the model encounters an error on refresh.
        /// </summary>
        public event EventHandler<Exception> RefreshError;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return this.GetProperty<bool>(); }
            protected set { this.SetProperty(value); }
        }

        /// <summary>
        /// Gets the refresh interval.
        /// </summary>
        /// <value>
        /// The refresh interval.
        /// </value>
        public TimeSpan RefreshInterval
        {
            get;
        }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        /// <value>
        /// The refresh command.
        /// </value>
        public ICommand RefreshCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the last refresh.
        /// </summary>
        /// <value>
        /// The last refresh.
        /// </value>
        protected DateTimeOffset LastRefresh
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The property value.
        /// </returns>
        public TValue GetProperty<TValue>([CallerMemberName]string propertyName = null)
        {
            TValue result = default(TValue);
            object value;
            if (this.properties.TryGetValue(propertyName, out value))
            {
                result = (TValue)value;
            }

            return result;
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        public void SetProperty<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            this.properties[propertyName] = value;
            this.NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            this.Refresh(false);
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        public async void Refresh(bool force)
        {
            await this.RefreshAsync(force);
        }

        /// <summary>
        /// Refreshes the instance, asynchronously.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        public async Task RefreshAsync(bool force)
        {
            if (force || (DateTimeOffset.UtcNow - this.LastRefresh >= this.RefreshInterval && !this.IsBusy))
            {
                this.IsBusy = true;

                try
                {
                    Task task = this.OnRefreshAsync();
                    if (task != null)
                    {
                        await task;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.LogException(exception);
                    this.NotifyRefreshError(exception);
                }
                finally
                {
                    this.IsBusy = false;
                    this.NotifyRefreshed();
                    this.LastRefresh = DateTimeOffset.UtcNow;
                }
            }
        }

        /// <summary>
        /// Called when this instance needs to refresh, asynchronously.
        /// </summary>
        /// <returns>
        /// A completion task.
        /// </returns>
        protected abstract Task OnRefreshAsync();

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (!string.IsNullOrEmpty(propertyName) && handler != null)
            {
                PropertyChangedEventArgs arguments;
                if (!ViewModelBase.PropertyEventArgumentsCache.TryGetValue(propertyName, out arguments))
                {
                    arguments = new PropertyChangedEventArgs(propertyName);
                    ViewModelBase.PropertyEventArgumentsCache[propertyName] = arguments;
                }

                handler(this, arguments);
            }
        }

        /// <summary>
        /// Notifies that the instance has refreshed.
        /// </summary>
        protected void NotifyRefreshed()
        {
            EventHandler handler = this.Refreshed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Notifies the refresh error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected void NotifyRefreshError(Exception exception)
        {
            EventHandler<Exception> handler = this.RefreshError;
            if (handler != null)
            {
                handler(this, exception);
            }
        }
    }
}
