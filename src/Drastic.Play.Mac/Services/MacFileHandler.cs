// <copyright file="MacFileHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.Play.Interfaces;

namespace Drastic.Play.Mac.Services
{
    /// <summary>
    /// Mac File Handler.
    /// </summary>
    public class MacFileHandler : IFileHandler
    {
        /// <inheritdoc/>
        public Task<Stream> GetFileStreamFromPathAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<string> PickVideo()
        {
            throw new NotImplementedException();
        }
    }
}