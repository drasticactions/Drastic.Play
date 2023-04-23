// <copyright file="INavigationHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Play.Interfaces
{
    /// <summary>
    /// Navigation Handler.
    /// </summary>
    public interface INavigationHandler
    {
        /// <summary>
        /// Display Alert to User.
        /// </summary>
        /// <param name="title">Title of message.</param>
        /// <param name="message">Message to user.</param>
        /// <param name="closeButton">Close button.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task DisplayAlertAsync(string title, string message, string closeButton);
    }
}
