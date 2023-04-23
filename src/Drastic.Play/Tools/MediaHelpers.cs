// <copyright file="MediaHelpers.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drastic.Play.Interfaces;
using Drastic.Play.Models;
using LibVLCSharp.Shared;

namespace Kibou.Tools
{
    /// <summary>
    /// Media Helpers.
    /// </summary>
    public static class MediaHelpers
    {
        /// <summary>
        /// Generate Video Item.
        /// </summary>
        /// <param name="type"><see cref="FromType"/>.</param>
        /// <param name="path">Path of the file.</param>
        /// <param name="token">UWP Token for file.</param>
        /// <returns><see cref="VideoItem"/>.</returns>
        public static VideoItem GenerateVideoItem(FromType type, string path, string token)
        {
            var videoItem = new VideoItem
            {
                FromType = type,
                Path = path,
                Name = System.IO.Path.GetFileName(path),
                Token = token,
            };
            return videoItem;
        }
    }
}
