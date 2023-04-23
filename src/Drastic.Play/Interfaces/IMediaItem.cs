// <copyright file="IMediaItem.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace Drastic.Play.Interfaces
{
    /// <summary>
    /// Media Item.
    /// </summary>
    public interface IMediaItem
    {
        /// <summary>
        /// Gets or sets the ID of the item.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the path to the item.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of media location this file is located at.
        /// </summary>
        FromType FromType { get; set; }

        /// <summary>
        /// Gets or sets the token of the item.
        /// </summary>
        string Token { get; set; }
    }
}
