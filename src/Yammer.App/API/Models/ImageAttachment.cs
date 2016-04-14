//---------------------------------------------------------------------
// <copyright file="ImageAttachment.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Windows.Storage;
    /// <summary>
    /// Image attachment class
    /// </summary>
    public class ImageAttachment : IAttachment, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("content_class")]
        public string ContentClass { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty("owner_type")]
        public string OwnerType { get; set; }

        [JsonProperty("official")]
        public bool Official { get; set; }

        [JsonProperty("small_icon_url")]
        public string SmallIconUrl { get; set; }

        [JsonProperty("large_icon_url")]
        public string LargeIconUrl { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("large_preview_url")]
        public string LargePreviewUrl { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("last_uploaded_at")]
        public string LastUploadedAt { get; set; }

        [JsonProperty("last_uploaded_by_id")]
        public int LastUploadedById { get; set; }

        [JsonProperty("last_uploaded_by_type")]
        public string LastUploadedByType { get; set; }

        [JsonProperty("uuid")]
        public object Uuid { get; set; }

        [JsonProperty("transcoded")]
        public object Transcoded { get; set; }

        [JsonProperty("streaming_url")]
        public object StreamingUrl { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("y_id")]
        public int YId { get; set; }

        [JsonProperty("overlay_url")]
        public string OverlayUrl { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("group_id")]
        public int? GroupId { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("scaled_url")]
        public string ScaledUrl { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("latest_version_id")]
        public int LatestVersionId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("real_type")]
        public string RealType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Loads this instance, asynchronously.
        /// </summary>
        /// <returns>
        /// The file task.
        /// </returns>
        public async Task<IStorageFile> LoadAsync()
        {
            this.SetIsBusy(true);
            try
            {
                return await YammerService.Instance.GetImageAttachmentFileAsync(this.PreviewUrl, this);
            }
            finally
            {
                this.SetIsBusy(false);
            }
        }

        /// <summary>
        /// Sets the is busy.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetIsBusy(bool value)
        {
            this.IsBusy = value;
            this.TriggerPropertyChanged(nameof(this.IsBusy));
        }

        /// <summary>
        /// Triggers the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void TriggerPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
