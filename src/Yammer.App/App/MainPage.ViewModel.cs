//---------------------------------------------------------------------
// <copyright file="MainPage.ViewModel.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using API;
    using API.Models;

    /// <summary>
    /// Main page class
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// View model class
        /// </summary>
        public class ViewModel : ViewModelBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ViewModel" /> class.
            /// </summary>
            /// <param name="currentUser">The current user.</param>
            /// <param name="networks">The networks.</param>
            public ViewModel(User currentUser, IEnumerable<Network> networks)
                : base(TimeSpan.FromSeconds(30))
            {
                this.CurrentUser = currentUser.ThrowIfNull(nameof(currentUser));
                this.Networks = networks.ThrowIfNull(nameof(networks));
            }

            /// <summary>
            /// Gets the current user.
            /// </summary>
            /// <value>
            /// The current user.
            /// </value>
            public User CurrentUser
            {
                get;
            }

            /// <summary>
            /// Gets the networks.
            /// </summary>
            /// <value>
            /// The networks.
            /// </value>
            public IEnumerable<Network> Networks
            {
                get { return this.GetProperty<IEnumerable<Network>>(); }
                private set { this.SetProperty(value); }
            }

            /// <summary>
            /// Gets or sets the network.
            /// </summary>
            /// <value>
            /// The network.
            /// </value>
            public Network Network
            {
                get { return this.GetProperty<Network>(); }
                set { this.SetProperty(value); }
            }

            /// <summary>
            /// Gets my groups.
            /// </summary>
            /// <value>
            /// My groups.
            /// </value>
            public IEnumerable<Group> MyGroups
            {
                get { return this.GetProperty<IEnumerable<Group>>(); }
                private set { this.SetProperty(value); }
            }

            /// <summary>
            /// Creates an instance, asynchronously.
            /// </summary>
            /// <returns>
            /// The model task.
            /// </returns>
            public static async Task<ViewModel> CreateAsync()
            {
                User currentUser = await YammerService.Instance.GetCurrentUserAsync();
                Network[] networks = await YammerService.Instance.GetNetworkAsync();

                ViewModel model = new ViewModel(currentUser, networks);
                await ViewModel.InternalRefreshAsync(model);

                return model;
            }

            /// <summary>
            /// Called when this instance needs to refresh, asynchronously.
            /// </summary>
            /// <returns>
            /// A completion task.
            /// </returns>
            protected override Task OnRefreshAsync()
            {
                return ViewModel.InternalRefreshAsync(this);
            }

            /// <summary>
            /// Internal refresh, asynchronously.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <returns>
            /// The completion task.
            /// </returns>
            private static async Task InternalRefreshAsync(ViewModel instance)
            {
                Network[] networks = await YammerService.Instance.GetNetworkAsync();
                Network network = networks.Where(_ => instance.Network == null ? _.IsPrimary : _.Id == instance.Network.Id).FirstOrDefault();
                Group[] groups = await YammerService.Instance.GetUserGroupsAsync();
                if (network != null &&
                    network.GroupCounts != null &&
                    network.GroupCounts.UnseenGroupThreadCountsLookup != null)
                {
                    foreach (Group group in groups)
                    {
                        Tuple<int, int> groupInfo;
                        if (network.GroupCounts.UnseenGroupThreadCountsLookup.TryGetValue(group.Id, out groupInfo))
                        {
                            group.Index = groupInfo.Item1;
                            group.UnreadCount = groupInfo.Item2;
                        }
                        else
                        {
                            // apparently the unread counts don't come back for all the groups (for some reason)
                            // so throw these to the bottom, as it likely means that user hasn't visited them in a while
                            group.Index = int.MaxValue;
                        }
                    }

                    instance.Network = network;
                }

                instance.MyGroups = groups.OrderBy(_ => _.Index).ToArray();
            }
        }
    }
}
