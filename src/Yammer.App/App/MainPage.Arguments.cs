//---------------------------------------------------------------------
// <copyright file="MainPage.Arguments.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    /// <summary>
    /// Main page class
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// Arguments class
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Arguments" /> class.
            /// </summary>
            /// <param name="launchArguments">The launch arguments.</param>
            public Arguments(string launchArguments)
            {
                this.LaunchArguments = launchArguments.ThrowIfNull(nameof(launchArguments));
            }

            /// <summary>
            /// Gets the launch arguments.
            /// </summary>
            /// <value>
            /// The launch arguments.
            /// </value>
            public string LaunchArguments
            {
                get;
            }
        }
    }
}
