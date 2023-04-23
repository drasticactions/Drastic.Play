// <copyright file="MacAppDispatcher.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Services;

namespace Drastic.Play.Mac.Services
{
    /// <summary>
    /// Mac App Dispatcher.
    /// </summary>
    public class MacAppDispatcher : NSObject, IAppDispatcher
    {
        /// <inheritdoc/>
        public bool Dispatch(Action action)
        {
            this.InvokeOnMainThread(() => action.Invoke());
            return true;
        }
    }
}