// <copyright file="IFileHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Play.Interfaces
{
    /// <summary>
    /// File Handler.
    /// </summary>
    public interface IFileHandler
    {
        /// <summary>
        /// Gets stream from file path.
        /// </summary>
        /// <param name="path">File Path.</param>
        /// <returns>Stream.</returns>
        Task<Stream> GetFileStreamFromPathAsync(string path);

        /// <summary>
        /// Opens File handler for video, gets path.
        /// </summary>
        /// <returns>Stream.</returns>
        Task<string> PickVideo();
    }
}
