//---------------------------------------------------------------------
// <copyright file="RefreshCommand.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    /// <summary>
    /// Refresh command class
    /// </summary>
    public class RefreshCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshCommand"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public RefreshCommand(ViewModelBase model)
        {
            this.Model = model.ThrowIfNull("model");
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public ViewModelBase Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            this.Model.Refresh(true);
        }
    }
}
