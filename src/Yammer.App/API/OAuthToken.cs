//---------------------------------------------------------------------
// <copyright file="OAuthToken.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API
{
    using System;
    using System.Linq;
    using Windows.Security.Credentials;

    /// <summary>
    /// OAuth token class
    /// </summary>
    public class OAuthToken
    {
        /// <summary>
        /// The vault resource name
        /// </summary>
        private const string VaultResourceName = nameof(OAuthToken);

        /// <summary>
        /// The vault username
        /// </summary>
        private const string VaultUsername = "not-used";

        /// <summary>
        /// Prevents a default instance of the <see cref="OAuthToken" /> class from being created.
        /// </summary>
        /// <param name="value">The value.</param>
        private OAuthToken(string value)
        {
            this.Value = value.ThrowIfEmpty(nameof(value));
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get;
        }

        /// <summary>
        /// Tries to get the token value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if successful, false otherwise
        /// </returns>
        public static bool TryGetValue(out OAuthToken value)
        {
            bool result = false;
            value = default(OAuthToken);

            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = vault.RetrieveAll().Where(_ => _.Resource == OAuthToken.VaultResourceName).FirstOrDefault();
            if (credential != null)
            {
                credential.RetrievePassword();
                value = new OAuthToken(credential.Password);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Saves the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static OAuthToken Save(string value)
        {
            value.ThrowIfEmpty(nameof(value));
            PasswordVault vault = new PasswordVault();
            vault.Add(new PasswordCredential(OAuthToken.VaultResourceName, OAuthToken.VaultUsername, value));
            return new OAuthToken(value);
        }

        /// <summary>
        /// Deletes the token.
        /// </summary>
        public static void Delete()
        {
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = vault.Retrieve(OAuthToken.VaultResourceName, OAuthToken.VaultUsername);
            vault.Remove(credential);
        }
    }
}
