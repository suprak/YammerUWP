//---------------------------------------------------------------------
// <copyright file="Network.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Network class
    /// </summary>
    public class Network
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("community")]
        public bool Community { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("show_upgrade_banner")]
        public bool ShowUpgradeBanner { get; set; }

        [JsonProperty("header_background_color")]
        public string HeaderBackgroundColor { get; set; }

        [JsonProperty("header_text_color")]
        public string HeaderTextColor { get; set; }

        [JsonProperty("navigation_background_color")]
        public string NavigationBackgroundColor { get; set; }

        [JsonProperty("navigation_text_color")]
        public string NavigationTextColor { get; set; }

        [JsonProperty("paid")]
        public bool Paid { get; set; }

        [JsonProperty("moderated")]
        public bool Moderated { get; set; }

        [JsonProperty("is_org_chart_enabled")]
        public bool IsOrgChartEnabled { get; set; }

        [JsonProperty("is_group_enabled")]
        public bool IsGroupEnabled { get; set; }

        [JsonProperty("is_chat_enabled")]
        public bool IsChatEnabled { get; set; }

        [JsonProperty("is_translation_enabled")]
        public bool IsTranslationEnabled { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("browser_deprecation_url")]
        public object BrowserDeprecationUrl { get; set; }

        [JsonProperty("external_messaging_state")]
        public string ExternalMessagingState { get; set; }

        [JsonProperty("unseen_message_count")]
        public int UnseenMessageCount { get; set; }

        [JsonProperty("preferred_unseen_message_count")]
        public int PreferredUnseenMessageCount { get; set; }

        [JsonProperty("private_unseen_thread_count")]
        public int PrivateUnseenThreadCount { get; set; }

        [JsonProperty("inbox_unseen_thread_count")]
        public int InboxUnseenThreadCount { get; set; }

        [JsonProperty("group_counts")]
        public GroupCounts GroupCounts { get; set; }

        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("unseen_notification_count")]
        public int UnseenNotificationCount { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public NetworkToken Token { get; set; }
    }

    public class GroupCounts
    {
        [JsonProperty("unseen_group_thread_counts")]
        public JObject UnseenGroupThreadCounts { get; set; }

        public IDictionary<int, Tuple<int, int>> UnseenGroupThreadCountsLookup { get; set; }

        [JsonProperty("unseen_general_thread_count")]
        public int UnseenGeneralThreadCount { get; set; }
    }
}
