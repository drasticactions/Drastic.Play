// <copyright file="VideoItem.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drastic.Play.Interfaces;
using LibVLCSharp.Shared;

namespace Drastic.Play.Models
{
    /// <summary>
    /// Video Item.
    /// </summary>
    public class VideoItem : IMediaItem
    {
        /// <summary>
        /// Gets or sets the ID of the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the path to the item.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Duration of the item, in ms.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the token of the item.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of media location this file is located at.
        /// </summary>
        public FromType FromType { get; set; }
    }
}
